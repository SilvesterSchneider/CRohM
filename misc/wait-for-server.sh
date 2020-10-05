#!/bin/sh

SERVER_TIMEOUT=300

while [ "$(curl --insecure --silent --output /dev/null --write-out "%{http_code}\n" https://localhost/health)" != "200" ]; do
    echo "Waiting ${SERVER_TIMEOUT}s till timeout ..."

    SERVER_TIMEOUT=$((SERVER_TIMEOUT - 1))

    if [ "$SERVER_TIMEOUT" -lt 0 ]; then
        exit 1
    fi

    sleep 1
done
