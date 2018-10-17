cd client && bash docker-build.sh && cd ..
cd services/IdentityServer && bash docker-build.sh && cd ../..
cd services/TodoApi && bash docker-build.sh && cd ../..
cd services/PushApi && bash docker-build.sh && cd ../..