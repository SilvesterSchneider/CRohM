#!/bin/sh

# Pull docker image from dockerhub and start server
if ! sh ./misc/docker-compose-up.sh;
then
    echo "Pull docker image from dockerhub and start server failed"
    exit 1
fi

# Wait for server to be up
if ! sh ./misc/wait-for-server.sh;
then
    echo "Wait for server to be up failed"
    exit 1
fi
