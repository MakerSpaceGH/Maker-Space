FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Alle Projektdateien kopieren, damit restore funktioniert
COPY . .

RUN dotnet restore

RUN rm -rf bin obj

RUN dotnet publish "My_own_website.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "My_own_website.dll"]
