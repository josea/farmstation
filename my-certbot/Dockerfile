#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM certbot/certbot AS base
RUN apk add --update curl
RUN rm -rf /var/cache/apk/*
RUN mkdir /letsencrypt.defaults
RUN curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf > /letsencrypt.defaults/options-ssl-nginx.conf
RUN curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem > /letsencrypt.defaults/ssl-dhparams.pem
RUN apk del curl
COPY ./init-and-run.sh /
RUN chmod +x /init-and-run.sh