# ---------------------------------------------------------------
# 1. Base Image (Runtime)
# CHANGE: Updated from 8.0 to 10.0
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ---------------------------------------------------------------
# 2. Build Image (SDK)
# CHANGE: Updated from 8.0 to 10.0 so it can build your code
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# --- COPY CSPROJ FILES ---
COPY ["VueWebEnterprise.Api/VueWebEnterprise.Api.csproj", "VueWebEnterprise.Api/"]
COPY ["VueWebEnterprise.Application/VueWebEnterprise.Application.csproj", "VueWebEnterprise.Application/"]
COPY ["VueWebEnterprise.Domain/VueWebEnterprise.Domain.csproj", "VueWebEnterprise.Domain/"]
COPY ["VueWebEnterprise.Infrastructure/VueWebEnterprise.Infrastructure.csproj", "VueWebEnterprise.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "./VueWebEnterprise.Api/VueWebEnterprise.Api.csproj"

# --- COPY REST OF CODE ---
COPY . .
WORKDIR "/src/VueWebEnterprise.Api"
RUN dotnet build "./VueWebEnterprise.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VueWebEnterprise.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VueWebEnterprise.Api.dll"]