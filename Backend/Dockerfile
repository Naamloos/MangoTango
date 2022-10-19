FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build
COPY ./src/MangoTango.Api ./
RUN dotnet restore && dotnet publish --no-restore -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /build/out .
WORKDIR /config
ENTRYPOINT ["dotnet", "/app/MangoTango.Api.dll"]