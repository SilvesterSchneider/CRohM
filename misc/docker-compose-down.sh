#!/bin/sh

# Replace foreslashes
CROHM_IMAGE_PREP="$(echo "${CROHM_BRANCH}" | tr "/" -)"
export CROHM_IMAGE_PREP

# Replace underscores
CROHM_IMAGE="$(echo "${CROHM_IMAGE_PREP}" | tr "_" -)"
export CROHM_IMAGE

docker-compose down
