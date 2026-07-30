using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SeniorCrud.Application.Abstractions.DateTime;
using SeniorCrud.Infrastructure.Authentication;
using SeniorCrud.Infrastructure.Authentication.Options;

namespace SeniorCrud.UnitTests.Infrastructure;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateAccessToken_Should_Include_Claims_And_Expiration()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var options = Options.Create(new JwtOptions
        {
            Issuer = "SeniorCrud",
            Audience = "SeniorCrud.Client",
            SecretKey = "THIS_IS_A_LONG_TEST_SECRET_KEY_1234567890",
            AccessTokenMinutes = 30
        });

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.SetupGet(provider => provider.UtcNow).Returns(now);

        var generator = new JwtTokenGenerator(options, dateTimeProvider.Object);

        var token = generator.GenerateAccessToken(Guid.Parse("11111111-1111-1111-1111-111111111111"), "User Test", [new Claim("custom", "value")]);

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        parsed.Issuer.Should().Be("SeniorCrud");
        parsed.Audiences.Should().Contain("SeniorCrud.Client");
        parsed.Claims.Should().Contain(claim => claim.Type == ClaimTypes.Name && claim.Value == "User Test");
        parsed.Claims.Should().Contain(claim => claim.Type == "custom" && claim.Value == "value");
        parsed.ValidTo.Should().Be(now.AddMinutes(30).UtcDateTime);
    }
}
