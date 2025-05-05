# src/AuthWithStorage.API/Dockerfile

# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything from the root of your repo (including sql/, src/, etc.)
COPY . .

# Restore & build your API
WORKDIR /src/src/AuthWithStorage.API
RUN dotnet restore "AuthWithStorage.API.csproj"
RUN dotnet build   "AuthWithStorage.API.csproj" -c Release -o /app/build -p:SolutionDir=/src

# 2) Publish stage
FROM build AS publish
WORKDIR /src/src/AuthWithStorage.API
RUN dotnet publish "AuthWithStorage.API.csproj" -c Release -o /app/publish -p:SolutionDir=/src

# 3) Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Copy your SQL folder into the final image so it's available
# at /app/sql relative to the working directory.
COPY sql ./sql

ENTRYPOINT ["dotnet", "AuthWithStorage.API.dll"]
