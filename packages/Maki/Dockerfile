FROM ubuntu:20.04

# Set the working directory to /app
WORKDIR /

# Copy the current directory contents into the container at /app
ADD ./published /app
ADD ./engines/stockfish-ubuntu /app/
RUN mv /app/stockfish-ubuntu /app/stockfish
RUN chmod +x /app/stockfish

RUN chmod +x /app/Maki
RUN DEBIAN_FRONTEND=noninteractive && apt-get update
RUN DEBIAN_FRONTEND=noninteractive && apt-get install -y ca-certificates
RUN DEBIAN_FRONTEND=noninteractive && apt-get install zlib1g libc6 libgcc1 libgssapi-krb5-2 libssl1.1 libstdc++6 libicu66 -y

ENTRYPOINT /app/Maki