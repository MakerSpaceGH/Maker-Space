# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Nur die Projektdatei kopieren und Abhängigkeiten wiederherstellen
COPY *.csproj ./
RUN dotnet restore

# Restlichen Code kopieren
COPY . ./

# Projekt veröffentlichen in den Ordner /app/publish
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Veröffentlichten Output aus dem Build-Stadium kopieren
COPY --from=build /app/publish ./

# Container startet deine Anwendung
ENTRYPOINT ["dotnet", "My_own_website.dll"]
