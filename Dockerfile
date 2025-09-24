# --------------------------
# Base image
# --------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Run as root to allow writing to mounted volumes
USER root
WORKDIR /app
EXPOSE 8080

# --------------------------
# Build stage
# --------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EAMS/LBPAMS.csproj", "EAMS/"]
COPY ["EAMS-ACore/LBPAMS-ACore.csproj", "EAMS-ACore/"]
COPY ["EAMS-BLL/LBPAMS-BLL.csproj", "EAMS-BLL/"]
COPY ["EAMS-DAL/LBPAMS-DAL.csproj", "EAMS-DAL/"]

RUN dotnet restore "./EAMS/LBPAMS.csproj"
COPY . .
WORKDIR "/src/EAMS"
RUN dotnet build "./LBPAMS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --------------------------
# Publish stage
# --------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LBPAMS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --------------------------
# Final stage
# --------------------------
FROM base AS final
WORKDIR /app
ARG ENVIRONMENT=Staging
ENV DOTNET_ENVIRONMENT=$ENVIRONMENT

# Copy published app
COPY --from=publish /app/publish .
COPY ./EAMS/appsettings.${ENVIRONMENT}.json ./appsettings.json

# Persist DataProtection keys
VOLUME /home/app/.aspnet/DataProtection-Keys

# Use root user to ensure write permissions on hostPath/NFS volumes
USER root

ENTRYPOINT ["dotnet", "LBPAMS.dll"]
