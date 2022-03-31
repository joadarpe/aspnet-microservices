#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["web-apps/Shopping.WebApp/Shopping.WebApp.csproj", "web-apps/Shopping.WebApp/"]
COPY ["auth/IdentityClient.Extensions/IdentityClient.Extensions.csproj", "auth/IdentityClient.Extensions/"]
COPY ["building-blocks/Common/Common.csproj", "building-blocks/Common/"]
RUN dotnet restore "web-apps/Shopping.WebApp/Shopping.WebApp.csproj"
COPY . .
WORKDIR "/src/web-apps/Shopping.WebApp"
RUN dotnet build "Shopping.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shopping.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.WebApp.dll"]
