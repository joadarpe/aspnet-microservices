#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["web-apps/AspnetRunBasics/AspnetRunBasics.csproj", "web-apps/AspnetRunBasics/"]
COPY ["auth/IdentityClient.Extensions/IdentityClient.Extensions.csproj", "auth/IdentityClient.Extensions/"]
RUN dotnet restore "web-apps/AspnetRunBasics/AspnetRunBasics.csproj"
COPY . .
WORKDIR "/src/web-apps/AspnetRunBasics"
RUN dotnet build "AspnetRunBasics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspnetRunBasics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspnetRunBasics.dll"]
