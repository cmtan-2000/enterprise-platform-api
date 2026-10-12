# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution source
COPY . .

# Restore dependencies
RUN dotnet restore EnterprisePlatform.Api/EnterprisePlatform.Api.csproj

# Publish API
RUN dotnet publish EnterprisePlatform.Api/EnterprisePlatform.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Include the AWS RDS CA certificate
COPY EnterprisePlatform.Api/Certificates/global-bundle.pem /app/Certificates/global-bundle.pem

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "EnterprisePlatform.Api.dll"]
