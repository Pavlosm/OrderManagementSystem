# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln .
COPY OrderManagementService/*.csproj ./OrderManagementService/
COPY OrderManagementService.Core/*.csproj ./OrderManagementService.Core/
COPY OrderManagementService.Infrastructure/*.csproj ./OrderManagementService.Infrastructure/

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Create non-root user for security
RUN useradd -M --uid 1001 --no-create-home nonroot && \
    chown -R nonroot:nonroot /app
USER nonroot

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "OrderManagementService.dll"]