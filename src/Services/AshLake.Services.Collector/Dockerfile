#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/AshLake.Services.Collector/AshLake.Services.Collector.csproj", "src/Services/AshLake.Services.Collector/"]
COPY ["src/Contracts/AshLake.Contracts.Archiver/AshLake.Contracts.Archiver.csproj", "src/Contracts/AshLake.Contracts.Archiver/"]
COPY ["src/Contracts/AshLake.Contracts.Seedwork/AshLake.Contracts.Seedwork.csproj", "src/Contracts/AshLake.Contracts.Seedwork/"]
COPY ["src/BuildingBlocks/AshLake.BuildingBlocks.EventBus/AshLake.BuildingBlocks.EventBus.csproj", "src/BuildingBlocks/AshLake.BuildingBlocks.EventBus/"]
COPY ["src/BuildingBlocks/AshLake.BuildingBlocks.S3Object/AshLake.BuildingBlocks.S3Object.csproj", "src/BuildingBlocks/AshLake.BuildingBlocks.S3Object/"]
COPY ["src/BuildingBlocks/AshLake.BuildingBlocks.Healthchecks/AshLake.BuildingBlocks.Healthchecks.csproj", "src/BuildingBlocks/AshLake.BuildingBlocks.Healthchecks/"]
COPY ["src/Contracts/AshLake.Contracts.Collector/AshLake.Contracts.Collector.csproj", "src/Contracts/AshLake.Contracts.Collector/"]
RUN dotnet restore "src/Services/AshLake.Services.Collector/AshLake.Services.Collector.csproj"
COPY . .
WORKDIR "/src/src/Services/AshLake.Services.Collector"
RUN dotnet build "AshLake.Services.Collector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AshLake.Services.Collector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AshLake.Services.Collector.dll"]