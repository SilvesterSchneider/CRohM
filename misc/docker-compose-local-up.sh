#!/bin/sh

echo
echo "Starting docker-stack..."

export CROHM_BRANCH=$(git branch --show-current)
export CROHM_IMAGE=$(echo "${CROHM_BRANCH}" | tr "/" -)

if [ ${CROHM_BRANCH} = "master" ]; then
     export CROHM_IMAGE=latest;
fi

echo
echo "Current Branch:    ${CROHM_BRANCH}"
echo "Docker Tag:        ${CROHM_IMAGE}"

echo
echo "Build local image"
docker build -t crohmcrms/crohm_crms:${CROHM_IMAGE} .

echo
echo "Start docker-stack"
docker-compose up -d
