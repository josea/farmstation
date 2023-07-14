#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM php:fpm-alpine AS base
RUN docker-php-ext-install mysqli
COPY html/ /var/www/html