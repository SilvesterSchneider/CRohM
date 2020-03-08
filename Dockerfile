# Build container
# docker build -t crms . 
# Run container
# docker run --name crms -d -p 8889:80 crms

### STAGE 1: Build frontend ###
# base image
FROM node:12.2.0 as buildFrontend

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install and cache app dependencies
COPY /frontend/package.json ./
RUN npm install
RUN npm install -g @angular/cli@latest

# add app
COPY ./frontend /app

# generate build
RUN ng build --output-path=dist


### STAGE 2: Build backend ###
# Get sdk
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS buildBackend
WORKDIR /src

# Copy required files into container
COPY ["backend/WebApi/WebApi.csproj", "./WebApi/"]
COPY ["backend/ModelLayer/ModelLayer.csproj", "./ModelLayer/"]
COPY ["backend/RepositoryLayer/RepositoryLayer.csproj", "./RepositoryLayer/"]
COPY ["backend/ServiceLayer/ServiceLayer.csproj", "./ServiceLayer/"]
# Install and get dependencies
RUN dotnet restore "./WebApi/WebApi.csproj"

COPY --from=buildFrontend /app/dist ./backend/WebApi/wwwroot
COPY ./backend .
# Build
RUN dotnet publish "./WebApi/WebApi.csproj" -c Release -o /app


### STAGE 3: Run ###
# Get asp.net
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.11

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=buildBackend /app ./
ENTRYPOINT ["dotnet", "WebApi.dll"]