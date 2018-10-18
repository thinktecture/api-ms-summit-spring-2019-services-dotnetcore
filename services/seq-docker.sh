#!/usr/bin/env bash

rm -rf ./seq
mkdir seq

docker run \
  -e ACCEPT_EULA=Y \
  -v seq:/data \
  -p 4041:80 \
  -p 5341:5341 \
  datalust/seq:latest