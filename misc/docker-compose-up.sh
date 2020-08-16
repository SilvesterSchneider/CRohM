#!/bin/sh

echo
echo
echo "Starting docker-stack..."

export CROHM_BRANCH=$(git branch --show-current)
export CROHM_IMAGE=$(echo "${CROHM_BRANCH}" | tr "/" -)

echo
echo "Current Branch:    ${CROHM_BRANCH}"
echo "Docker Tag:        ${CROHM_IMAGE}"

echo
echo "Pull most recent image for branch"
docker pull crohmcrms/crohm_crms:${CROHM_IMAGE}

echo
echo "Start docker-stack"
docker-compose up -d
