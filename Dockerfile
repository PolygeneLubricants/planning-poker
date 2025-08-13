# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy project files for dependency restoration
COPY src/PlanningPoker.Server/PlanningPoker.Server.csproj src/PlanningPoker.Server/
COPY src/PlanningPoker.Client/PlanningPoker.Client.csproj src/PlanningPoker.Client/
COPY src/PlanningPoker.Engine.Core/PlanningPoker.Engine.Core.csproj src/PlanningPoker.Engine.Core/
COPY src/PlanningPoker.Engine.Core.Models/PlanningPoker.Engine.Core.Models.csproj src/PlanningPoker.Engine.Core.Models/
COPY src/PlanningPoker.Hub.Client/PlanningPoker.Hub.Client.csproj src/PlanningPoker.Hub.Client/
COPY src/PlanningPoker.Hub.Client.Abstractions/PlanningPoker.Hub.Client.Abstractions.csproj src/PlanningPoker.Hub.Client.Abstractions/
COPY src/PlanningPoker.Server.Infrastructure/PlanningPoker.Server.Infrastructure.csproj src/PlanningPoker.Server.Infrastructure/

# Restore dependencies for each project
RUN dotnet restore src/PlanningPoker.Server/PlanningPoker.Server.csproj

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish src/PlanningPoker.Server/PlanningPoker.Server.csproj -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Set working directory
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Set the entry point
ENTRYPOINT ["dotnet", "PlanningPoker.Server.dll"]
