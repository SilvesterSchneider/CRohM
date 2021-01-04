#!/bin/sh

echo
echo "Starting docker-stack..."

CROHM_BRANCH="$(git branch --show-current)"
export CROHM_BRANCH

# Replace foreslashes
CROHM_IMAGE_PREP="$(echo "${CROHM_BRANCH}" | tr "/" -)"
export CROHM_IMAGE_PREP

# Replace underscores
CROHM_IMAGE="$(echo "${CROHM_IMAGE_PREP}" | tr "_" -)"
export CROHM_IMAGE

#if [ "${CROHM_BRANCH}" = "master" ]; then
#    export CROHM_IMAGE=latest
#fi

echo
echo "Current Branch:    ${CROHM_BRANCH}"
echo "Docker Tag:        ${CROHM_IMAGE}"

echo
echo "Pull most recent image for branch"
docker pull crohmcrms/crohm_crms:${CROHM_IMAGE}

echo
echo "Start docker-stack"
docker-compose up -d
