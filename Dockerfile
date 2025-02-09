# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
##WORKDIR

# Copy solution and project files
COPY OrderManagementSystem.Docker.sln .
COPY src/OrderManagementService.WebApi/OrderManagementService.WebApi.csproj ./src/OrderManagementService.WebApi/
COPY src/OrderManagementService.Core/OrderManagementService.Core.csproj ./src/OrderManagementService.Core/
COPY src/OrderManagementService.Infrastructure/OrderManagementService.Infrastructure.csproj ./src/OrderManagementService.Infrastructure/

# Restore NuGet packages
# RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet publish OrderManagementSystem.Docker.sln -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Create non-root user for security
RUN useradd -M --uid 1001 --no-create-home nonroot && \
    chown -R nonroot:nonroot /app
USER nonroot

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "OrderManagementService.WebApi.dll"]