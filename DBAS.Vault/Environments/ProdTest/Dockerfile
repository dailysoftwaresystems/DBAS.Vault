#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /app
RUN rm -rf /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/DBAS.Vault/DBAS.Vault/DBAS.Vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"

FROM base AS build-env
WORKDIR /app
RUN dotnet restore /app/src/DBAS.Vault/DBAS.Vault/DBAS.Vault.csproj
RUN dotnet build /app/src/DBAS.Vault/DBAS.Vault/DBAS.Vault.csproj --configuration Release --no-cache --output /app/build
RUN dotnet publish /app/src/DBAS.Vault/DBAS.Vault/DBAS.Vault.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY ./DBAS.Vault/DBAS.Vault/Environments/ProdTest/key-vault.pfx /usr/local/share/ca-certificates/
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=key-vault
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/usr/local/share/ca-certificates/key-vault.pfx
RUN apt-get update && apt-get install -y curl default-mysql-client ca-certificates && update-ca-certificates
HEALTHCHECK --interval=3s --timeout=1s --retries=3 CMD curl -f -k https://localhost/health --fail
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "DBAS.Vault.dll"]