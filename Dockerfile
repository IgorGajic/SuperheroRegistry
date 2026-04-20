FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy project files
COPY SuperheroRegistry.Domain/SuperheroRegistry.Domain.csproj SuperheroRegistry.Domain/
COPY SuperheroRegistry.Application/SuperheroRegistry.Application.csproj SuperheroRegistry.Application/
COPY SuperheroRegistry.Infrastructure/SuperheroRegistry.Infrastructure.csproj SuperheroRegistry.Infrastructure/
COPY SuperheroRegistry.API/SuperheroRegistry.Api.csproj SuperheroRegistry.API/
COPY SuperheroRegistry.Tests/SuperheroRegistry.Tests.csproj SuperheroRegistry.Tests/

# Restore dependencies
RUN dotnet restore SuperheroRegistry.API/SuperheroRegistry.Api.csproj

# Copy entire source
COPY . .

# Build and publish
RUN dotnet publish SuperheroRegistry.API/SuperheroRegistry.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "SuperheroRegistry.Api.dll"]
