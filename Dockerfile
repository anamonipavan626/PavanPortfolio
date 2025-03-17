# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PavanPortfolio.csproj", "./"]
RUN dotnet restore "./PavanPortfolio.csproj"
COPY . .
RUN dotnet publish "./PavanPortfolio.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PavanPortfolio.dll"]
