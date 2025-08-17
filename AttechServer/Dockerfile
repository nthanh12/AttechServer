# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["AttechServer.csproj", "./"]
RUN dotnet restore "AttechServer.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "AttechServer.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AttechServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create uploads directory
RUN mkdir -p /app/uploads/images /app/uploads/videos /app/uploads/documents

# Copy published app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BASE_URL=https://api.attech.space

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Start the app
ENTRYPOINT ["dotnet", "AttechServer.dll"] 