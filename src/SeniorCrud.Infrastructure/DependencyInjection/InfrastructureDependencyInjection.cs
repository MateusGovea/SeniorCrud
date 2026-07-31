using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using SeniorCrud.Application.Abstractions.Authentication;
using SeniorCrud.Application.Abstractions.Caching;
using SeniorCrud.Application.Abstractions.Csv;
using SeniorCrud.Application.Abstractions.DateTime;
using SeniorCrud.Application.Abstractions.Identity;
using SeniorCrud.Application.Abstractions.ViaCep;
using SeniorCrud.Infrastructure.Authentication;
using SeniorCrud.Infrastructure.Authentication.Options;
using SeniorCrud.Infrastructure.Caching;
using SeniorCrud.Infrastructure.Caching.Options;
using SeniorCrud.Infrastructure.Csv;
using SeniorCrud.Infrastructure.Csv.Options;
using SeniorCrud.Infrastructure.Services;
using SeniorCrud.Infrastructure.ViaCep;
using SeniorCrud.Infrastructure.ViaCep.Options;

namespace SeniorCrud.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    private static readonly HttpStatusCode[] RetryStatusCodes =
    [
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    ];

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey)
                    && Encoding.UTF8.GetByteCount(options.SecretKey) >= 32,
                $"{JwtOptions.SectionName}:SecretKey must be provided and contain at least 32 bytes.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),
                $"{JwtOptions.SectionName}:Issuer must be provided.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                $"{JwtOptions.SectionName}:Audience must be provided.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearerOptions, jwtOptions) =>
            {
                var settings = jwtOptions.Value;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
                };
            });

        services.AddAuthorization();

        services.AddOptions<ViaCepOptions>()
            .BindConfiguration(ViaCepOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<CsvOptions>()
            .BindConfiguration(CsvOptions.SectionName)
            .ValidateOnStart();

        services.AddOptions<CacheOptions>()
            .BindConfiguration(CacheOptions.SectionName)
            .ValidateOnStart();

        services.AddHttpClient<IViaCepClient, ViaCepClient>()
            .AddPolicyHandler(CreateRetryPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5)))
            .AddPolicyHandler(CreateCircuitBreakerPolicy());

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICsvExportService, CsvExportService>();
        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(response => RetryStatusCodes.Contains(response.StatusCode))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(response => RetryStatusCodes.Contains(response.StatusCode))
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}
