FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY src/EcoColeta.Api/EcoColeta.Api.csproj src/EcoColeta.Api/
RUN dotnet restore src/EcoColeta.Api/EcoColeta.Api.csproj
COPY src/ src/
RUN dotnet publish src/EcoColeta.Api/EcoColeta.Api.csproj --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ARG APP_VERSION=local
ENV APP_VERSION=$APP_VERSION ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
HEALTHCHECK --interval=10s --timeout=5s --start-period=60s --retries=12 CMD curl --fail --silent http://localhost:8080/health/ready || exit 1
ENTRYPOINT ["dotnet", "EcoColeta.Api.dll"]
