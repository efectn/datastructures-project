# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj ./
COPY . .
RUN dotnet restore

RUN dotnet publish -c Release -o /src/publish --self-contained false

ENTRYPOINT ["dotnet", "/src/publish/datastructures-project.dll"]
