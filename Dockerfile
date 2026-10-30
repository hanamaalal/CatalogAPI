
# Étape 1 : compilation
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copie du fichier projet depuis la racine du dépôt
COPY ["src/ProductCatalog.Api/ProductCatalog.Api.csproj", "ProductCatalog.Api/"]

# Restauration des dépendances
RUN dotnet restore "ProductCatalog.Api/ProductCatalog.Api.csproj"

# Copie du code source
COPY . .

# Compilation et publication
WORKDIR /src/src/ProductCatalog.Api
RUN dotnet publish "ProductCatalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Étape 2 : image d'exécution
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]