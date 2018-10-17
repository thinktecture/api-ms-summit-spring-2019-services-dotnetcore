echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin ttconferences.azurecr.io

cd client && bash docker-push.sh && cd ..
cd services/IdentityServer && bash docker-push.sh && cd ../..
cd services/PushApi && bash docker-push.sh && cd ../..
cd services/TodoApi && bash docker-push.sh && cd ../..