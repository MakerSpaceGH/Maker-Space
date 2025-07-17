FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Alle Dateien kopieren (inklusive csproj und NuGet-Konfig)
COPY . .

# NuGet-Pakete wiederherstellen
RUN dotnet restore

# Projekt veröffentlichen (Publish) ohne Restore (weil schon gemacht)
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Veröffentlichten Build kopieren
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "My_own_website.dll"]
