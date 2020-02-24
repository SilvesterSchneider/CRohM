# docker build -t crms-frontend .
# docker run --name crms-frontend -p 8889:80 crms-frontend

### STAGE 1: Build ###
# Get node from docker
FROM node:lts-alpine3.11 AS build
WORKDIR /usr/src/app
# Copy package.json into container path (./)
COPY package.json ./
# Install fresh dependecies into container
RUN npm install
COPY . .
# Build
RUN npm run build

### STAGE 2: Run ###
# Get nginx from docker
FROM nginx:1.17-alpine
# Copy compiled dist into nginx as webserver
COPY --from=build /usr/src/app/dist/crms-frontend /usr/share/nginx/html
