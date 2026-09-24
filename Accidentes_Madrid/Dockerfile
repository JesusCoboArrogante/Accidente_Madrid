FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Accidentes_Madrid/Accidentes_Madrid.csproj", "Accidentes_Madrid/"]
RUN dotnet restore "Accidentes_Madrid/Accidentes_Madrid.csproj"
COPY . .
WORKDIR "/src/Accidentes_Madrid"
RUN dotnet build "./Accidentes_Madrid.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Accidentes_Madrid.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accidentes_Madrid.dll"]
