FROM node:24-bookworm-slim AS ui
WORKDIR /build/ui
COPY src/itsm-ui/package*.json ./
RUN npm ci
COPY src/itsm-ui/ ./
RUN npm test && npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build
WORKDIR /build
COPY global.json ./
COPY src/Itsm.Web/ src/Itsm.Web/
COPY --from=ui /build/ui/dist/ src/Itsm.Web/wwwroot/
RUN dotnet publish src/Itsm.Web/Itsm.Web.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /out/ ./
ENV ASPNETCORE_HTTP_PORTS=8080
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Itsm.Web.dll"]
