# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
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

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LBPAMS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage (used in production or when running from VS in regular mode)
FROM base AS final
WORKDIR /app

# 🔑 Add environment argument for config
ARG ENVIRONMENT=Staging
ENV DOTNET_ENVIRONMENT=$ENVIRONMENT

# Copy published binaries
COPY --from=publish /app/publish .

# 🔑 Copy environment-specific config as default appsettings.json
# Make sure appsettings.Staging.json and appsettings.Production.json exist in your project output
COPY ./EAMS/appsettings.${ENVIRONMENT}.json ./appsettings.json

ENTRYPOINT ["dotnet", "LBPAMS.dll"]
