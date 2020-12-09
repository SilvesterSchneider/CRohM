#!/bin/sh

# Build docker image from local data and start server
if ! sh ./misc/docker-compose-local-up.sh;
then
    echo "Build docker image from local data and start server failed"
    exit 1
fi

# Wait for server to be up
if ! sh ./misc/wait-for-server.sh;
then
    echo "Wait for server to be up failed"
    exit 1
fi
