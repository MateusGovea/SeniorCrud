FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY src/SeniorCrud.Domain/*.csproj src/SeniorCrud.Domain/
COPY src/SeniorCrud.Application/*.csproj src/SeniorCrud.Application/
COPY src/SeniorCrud.Infrastructure/*.csproj src/SeniorCrud.Infrastructure/
COPY src/SeniorCrud.Persistence/*.csproj src/SeniorCrud.Persistence/
COPY src/SeniorCrud.Api/*.csproj src/SeniorCrud.Api/

RUN dotnet restore src/SeniorCrud.Api/SeniorCrud.Api.csproj

COPY . .

RUN dotnet publish src/SeniorCrud.Api/SeniorCrud.Api.csproj -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /publish .

ENTRYPOINT ["dotnet", "SeniorCrud.Api.dll"]