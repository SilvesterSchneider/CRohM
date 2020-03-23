# Build container
# docker build -f Dockerfile --no-cache --force-rm -t crohmcrms/crohm_crms .
# Run container
# docker run --name crms -d -p 8889:80 crms

### STAGE 1: Build Frontend ###
# base image
FROM node:12.2.0 as buildFrontend

# set working directory
WORKDIR /usr/src/frontend

# install and cache app dependencies
#COPY frontend/package.json ./
#RUN npm install --no-fund --no-optional
#RUN npm install -g @angular/cli@latest --no-fund --no-optional

# Copy frontend src to workdir
#COPY frontend ./

# Build frontend
#RUN ng build --output-path=dist

### STAGE 2: Build Backend ###
# Get sdk
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS buildBackend
WORKDIR /usr/src/backend

# Copy required files into container
COPY ["backend/WebApi/WebApi.csproj", "./WebApi/"]
COPY ["backend/ModelLayer/ModelLayer.csproj", "./ModelLayer/"]
COPY ["backend/RepositoryLayer/RepositoryLayer.csproj", "./RepositoryLayer/"]
COPY ["backend/ServiceLayer/ServiceLayer.csproj", "./ServiceLayer/"]

# Install and get dependencies
RUN dotnet restore "./WebApi/WebApi.csproj"

# Copy frontend build into backend
#COPY --from=buildFrontend /usr/src/frontend/dist /usr/src/backend/WebApi/wwwroot

# Copy backend src to workdir
COPY ./backend .

# Build backend into app folder
RUN dotnet publish "./WebApi/WebApi.csproj" -c Release -o /app

WORKDIR /app
RUN ls -la
RUN cat appsettings.json

### STAGE 3: Run Webserver ###
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.11

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=buildBackend /app /app
ENTRYPOINT ["dotnet", "/app/WebApi.dll"]