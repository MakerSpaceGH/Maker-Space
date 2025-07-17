# ---- Build-Phase ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Nur Projektdateien und NuGet-Konfig kopieren
COPY *.csproj ./
COPY NuGet.config ./

# Restore der NuGet-Pakete (schnellerer Build-Caching)
RUN dotnet restore

# Restlichen Code kopieren (nicht doppelt)
COPY . .

# Projekt publizieren in /app/publish (cleaner Output)
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish

# ---- Runtime-Phase ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Nur die gepublishten Dateien aus dem Build-Container kopieren
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "My_own_website.dll"]
