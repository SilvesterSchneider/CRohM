#!/bin/sh

# Start server
if ! sh ./misc/start-server.sh
then
    echo "Start server failed"
    exit 1
fi

# Run tests
sh ./misc/run-tests.sh;

# Shutdown server
sh ./misc/docker-compose-down.sh;
