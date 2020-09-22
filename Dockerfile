# Build container
# docker build -f Dockerfile --no-cache --force-rm -t crohmcrms/crohm_crms:latest .
# Run container
# docker run --name crms -d -p 80:80 crohmcrms/crohm_crms:latest

### STAGE 1: Build Frontend ###
# base image
FROM node:14.11.0 as buildFrontend

# set working directory
WORKDIR /usr/src/frontend

# install and cache app dependencies
COPY frontend/package.json ./
COPY frontend/package-lock.json ./
RUN npm ci

# Copy frontend src to workdir
COPY frontend ./

# Build frontend
RUN npm run build -- --output-path=dist

### STAGE 2: Build Backend ###
# Get sdk
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS buildBackend
WORKDIR /app

# Copy required files into container
COPY ["backend/WebApi/WebApi.csproj", "./WebApi/"]
COPY ["backend/ModelLayer/ModelLayer.csproj", "./ModelLayer/"]
COPY ["backend/RepositoryLayer/RepositoryLayer.csproj", "./RepositoryLayer/"]
COPY ["backend/ServiceLayer/ServiceLayer.csproj", "./ServiceLayer/"]

# Install and get dependencies
RUN dotnet restore "./WebApi/WebApi.csproj"

# Copy frontend build into backend
COPY --from=buildFrontend /usr/src/frontend/dist /app/WebApi/wwwroot

# Copy backend src to workdir
COPY ./backend .

# Build backend into app folder
WORKDIR /app/WebApi
RUN dotnet publish -c Release -o ./app

### STAGE 3: Run Webserver ###
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.11

RUN apk add --no-cache curl icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE ${HTTP_PORT}
EXPOSE ${HTTPS_PORT}

COPY --from=buildBackend /app /app

WORKDIR /app/WebApi/app

HEALTHCHECK --interval=5s --timeout=5s --start-period=5s --retries=10 CMD curl --silent --fail --insecure https://localhost:${HTTPS_PORT}/health || exit 1
ENTRYPOINT ["dotnet", "WebApi.dll"]
