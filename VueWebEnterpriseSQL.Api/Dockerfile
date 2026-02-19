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
COPY ["VueWebEnterpriseSQL.Api/VueWebEnterpriseSQL.Api.csproj", "VueWebEnterpriseSQL.Api/"]
COPY ["VueWebEnterpriseSQL.Application/VueWebEnterpriseSQL.Application.csproj", "VueWebEnterpriseSQL.Application/"]
COPY ["VueWebEnterpriseSQL.Domain/VueWebEnterpriseSQL.Domain.csproj", "VueWebEnterpriseSQL.Domain/"]
COPY ["VueWebEnterpriseSQL.Infrastructure/VueWebEnterpriseSQL.Infrastructure.csproj", "VueWebEnterpriseSQL.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "./VueWebEnterpriseSQL.Api/VueWebEnterpriseSQL.Api.csproj"

# --- COPY REST OF CODE ---
COPY . .
WORKDIR "/src/VueWebEnterpriseSQL.Api"
RUN dotnet build "./VueWebEnterpriseSQL.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VueWebEnterpriseSQL.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VueWebEnterpriseSQL.Api.dll"]