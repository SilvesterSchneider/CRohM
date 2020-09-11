#!/bin/sh

export CROHM_IMAGE=$(echo "${CROHM_BRANCH}" | tr "/" -)

docker-compose down
