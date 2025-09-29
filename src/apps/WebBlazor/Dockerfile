# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# csproj y restore (prefijo src/)
COPY ["src/apps/WebBlazor/WebBlazor.csproj", "src/apps/WebBlazor/"]
RUN dotnet restore "src/apps/WebBlazor/WebBlazor.csproj"

# resto del código
COPY . .

# build
WORKDIR "/src/src/apps/WebBlazor"
RUN dotnet build "WebBlazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebBlazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebBlazor.dll"]
