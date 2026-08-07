FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DigitalPano.sln ./
COPY src/DigitalPano.Web/DigitalPano.Web.csproj src/DigitalPano.Web/
RUN dotnet restore src/DigitalPano.Web/DigitalPano.Web.csproj

COPY src/DigitalPano.Web/ src/DigitalPano.Web/
RUN dotnet publish src/DigitalPano.Web/DigitalPano.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=10000
EXPOSE 10000

USER app
ENTRYPOINT ["dotnet", "DigitalPano.Web.dll"]
