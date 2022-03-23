#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["api-gateways/Ocelot.ApiGw/Ocelot.ApiGw.csproj", "api-gateways/Ocelot.ApiGw/"]
COPY ["auth/IdentityClient.Extensions/IdentityClient.Extensions.csproj", "auth/IdentityClient.Extensions/"]
COPY ["building-blocks/Common.Logging/Common.Logging.csproj", "building-blocks/Common.Logging/"]
RUN dotnet restore "api-gateways/Ocelot.ApiGw/Ocelot.ApiGw.csproj"
COPY . .
WORKDIR "/src/api-gateways/Ocelot.ApiGw"
RUN dotnet build "Ocelot.ApiGw.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ocelot.ApiGw.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.ApiGw.dll"]
