FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EcoColeta.Api/EcoColeta.Api.csproj", "EcoColeta.Api/"]
RUN dotnet restore "EcoColeta.Api/EcoColeta.Api.csproj"
COPY . .
WORKDIR "/src/EcoColeta.Api"
RUN dotnet build "EcoColeta.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcoColeta.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "EcoColeta.Api.dll"]
