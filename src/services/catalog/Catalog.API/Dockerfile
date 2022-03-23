#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["services/catalog/Catalog.API/Catalog.API.csproj", "services/catalog/Catalog.API/"]
COPY ["services/catalog/Catalog.API/mkcert", "services/catalog/Catalog.API/"]
COPY ["services/catalog/Catalog.API/rootCA*.pem", "services/catalog/Catalog.API/"]
COPY ["auth/IdentityClient.Extensions/IdentityClient.Extensions.csproj", "auth/IdentityClient.Extensions/"]
RUN dotnet restore "services/catalog/Catalog.API/Catalog.API.csproj"
COPY . .
WORKDIR "/src/services/catalog/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build src/services/catalog/Catalog.API/mkcert /usr/local/bin
COPY --from=build src/services/catalog/Catalog.API/rootCA*.pem /root/.local/share/mkcert/
RUN chmod +x /usr/local/bin/mkcert \
  && mkcert -install \
  && rm -rf /usr/local/bin/mkcert 
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
