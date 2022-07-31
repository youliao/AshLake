#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/Services/AshLake.Services.Archiver/AshLake.Services.Archiver.csproj", "src/Services/AshLake.Services.Archiver/"]
COPY ["src/Contracts/AshLake.Contracts.Archiver/AshLake.Contracts.Archiver.csproj", "src/Contracts/AshLake.Contracts.Archiver/"]
COPY ["src/Contracts/AshLake.Contracts.Seedwork/AshLake.Contracts.Seedwork.csproj", "src/Contracts/AshLake.Contracts.Seedwork/"]
COPY ["src/Contracts/AshLake.Contracts.Collector/AshLake.Contracts.Collector.csproj", "src/Contracts/AshLake.Contracts.Collector/"]
RUN dotnet restore "src/Services/AshLake.Services.Archiver/AshLake.Services.Archiver.csproj"
COPY . .
WORKDIR "/src/src/Services/AshLake.Services.Archiver"
RUN dotnet build "AshLake.Services.Archiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AshLake.Services.Archiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AshLake.Services.Archiver.dll"]