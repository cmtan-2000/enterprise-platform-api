# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore EnterprisePlatform.Migrator/EnterprisePlatform.Migrator.csproj

RUN dotnet publish EnterprisePlatform.Migrator/EnterprisePlatform.Migrator.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Include AWS RDS CA certificate
COPY EnterprisePlatform.Api/Certificates/global-bundle.pem /app/Certificates/global-bundle.pem

ENTRYPOINT ["dotnet", "EnterprisePlatform.Migrator.dll"]