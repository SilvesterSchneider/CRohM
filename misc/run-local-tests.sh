#!/bin/sh

# Start server
if ! sh ./misc/start-local-server.sh
then
    echo "Start server failed"
    exit 1
fi

# Start tests
sh ./misc/start-tests.sh;

# Shutdown server
sh ./misc/docker-compose-down.sh;
