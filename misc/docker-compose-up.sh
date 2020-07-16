export BRANCH=$(git branch --show-current)

docker pull crohmcrms/crohm_crms:${BRANCH}

docker-compose up -d