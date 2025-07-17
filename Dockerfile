# Basis-Image mit .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# SDK Image zum Bauen
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Kopiere die Projektdatei und stelle Abhängigkeiten wieder her
COPY ["My_own_website.csproj", "./"]
RUN dotnet restore "./My_own_website.csproj"

# Kopiere den Rest des Codes und baue das Projekt
COPY . .
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish

# Erstelle das finale Runtime-Image
FROM base AS final
WORKDIR /app

# Kopiere die gebauten Dateien vom Build-Image
COPY --from=build /app/publish .

# Starte die Anwendung
ENTRYPOINT ["dotnet", "My_own_website.dll"]
