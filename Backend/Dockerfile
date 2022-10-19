FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ./MangoTango.Api ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /src/out .
WORKDIR /config
ENTRYPOINT ["dotnet", "/app/MangoTango.Api.dll"]