#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM nginx:1.24-alpine AS base
COPY my-nginx/init-and-run.sh /
RUN chmod +x /init-and-run.sh
COPY ./my-nginx/nginx.conf /etc/nginx/
COPY ./data/nginx /etc/nginx/conf.d/