#!/bin/sh

# Replace foreslashes
export CROHM_IMAGE_PREP=$(echo "${CROHM_BRANCH}" | tr "/" -)

# Replace underscores
export CROHM_IMAGE=$(echo "${CROHM_IMAGE_PREP}" | tr "_" -)

docker-compose down
