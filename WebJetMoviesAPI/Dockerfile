FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY WebJetMoviesAPI/WebJetMoviesAPI.csproj WebJetMoviesAPI/
RUN dotnet restore WebJetMoviesAPI/WebJetMoviesAPI.csproj
COPY . .
WORKDIR /src/WebJetMoviesAPI
RUN dotnet build WebJetMoviesAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebJetMoviesAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebJetMoviesAPI.dll"]
