FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . ./FiapVideoProcessorWorker
COPY FiapVideoProcessorWorker/FiapVideoProcessorWorker ./FiapVideoProcessorWorker

WORKDIR /src/FiapVideoProcessorWorker
RUN dotnet restore "FiapVideoProcessorWorker.csproj"

FROM build AS publish
RUN dotnet publish "FiapVideoProcessorWorker.csproj" -c Release -o /app/publish --self-contained false -r linux-x64 /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FiapVideoProcessorWorker.dll"]