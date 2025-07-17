# ---- Build-Phase ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Nur Projektdateien kopieren (für Restore)
COPY *.csproj ./

# NuGet-Pakete wiederherstellen (Caching)
RUN dotnet restore

# Restlichen Quellcode kopieren
COPY . .

# Alte Build-Ordner löschen, um Konflikte zu vermeiden
RUN rm -rf bin obj

# Projekt im Release-Modus publizieren in /app/publish
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish

# ---- Runtime-Phase ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Gepublishten Code aus dem Build-Container kopieren
COPY --from=build /app/publish .

# Startbefehl für den Container
ENTRYPOINT ["dotnet", "My_own_website.dll"]
