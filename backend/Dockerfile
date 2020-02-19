# Build container
# docker build -t crms-backend . 
# Run container
# docker run --name crms-backend -d -p 8889:80 crms-backend

### STAGE 1: Build ###
# Get sdk
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
# Copy required files into container
COPY ["WebApi/WebApi.csproj", "./WebApi/"]
COPY ["ModelLayer/ModelLayer.csproj", "./ModelLayer/"]
COPY ["RepositoryLayer/RepositoryLayer.csproj", "./RepositoryLayer/"]
COPY ["ServiceLayer/ServiceLayer.csproj", "./ServiceLayer/"]
# Install and get dependencies
RUN dotnet restore "./WebApi/WebApi.csproj"
COPY . .
# Build
RUN dotnet publish "./WebApi/WebApi.csproj" -c Debug -o /app

### STAGE 2: Run ###
# Get asp.net
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.11
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WebApi.dll"]