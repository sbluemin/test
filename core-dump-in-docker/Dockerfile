FROM ubuntu:18.04
LABEL sbluemin="sbluemin"

RUN apt-get update && apt-get install -y build-essential

COPY build/core-dump-in-docker .

CMD [ "/core-dump-in-docker" ]