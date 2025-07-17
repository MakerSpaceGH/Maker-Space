FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Nur die Projektdateien kopieren (für Restore)
COPY *.csproj ./

# NuGet-Pakete wiederherstellen
RUN dotnet restore

# Jetzt den gesamten Quellcode kopieren
COPY . .

# Alte Build-Ordner löschen, falls vorhanden
RUN rm -rf bin obj

# Projekt im Release-Modus veröffentlichen, kein erneutes Restore nötig
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "My_own_website.dll"]
