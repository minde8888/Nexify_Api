#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Nexify.Api/Nexify.Api.csproj", "src/Nexify.Api/"]
COPY ["src/Nexify.Data/Nexify.Data.csproj", "src/Nexify.Data/"]
COPY ["src/Nexify.Domain/Nexify.Domain.csproj", "src/Nexify.Domain/"]
COPY ["src/Nexify.Service/Nexify.Service.csproj", "src/Nexify.Service/"]
RUN dotnet restore "src/Nexify.Api/Nexify.Api.csproj"
COPY . .
WORKDIR "/src/src/Nexify.Api"
RUN dotnet build "Nexify.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nexify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexify.Api.dll"]