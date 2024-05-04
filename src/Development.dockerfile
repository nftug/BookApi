FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet dev-certs https --clean && dotnet dev-certs https -t
