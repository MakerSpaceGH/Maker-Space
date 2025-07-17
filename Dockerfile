# ---- Build-Phase ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiere die Projektdatei und restore NuGet-Abhängigkeiten
COPY ["My_own_website.csproj", "./"]
RUN dotnet restore "My_own_website.csproj"

# Kopiere den Rest des Codes und veröffentliche die App
COPY . .
RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish

# ---- Runtime-Phase ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Kopiere veröffentlichte Dateien von der Build-Phase
COPY --from=build /app/publish .

# Öffne Port 80 (Standard-Webport)
EXPOSE 80

# Startbefehl für die Anwendung
ENTRYPOINT ["dotnet", "My_own_website.dll"]
