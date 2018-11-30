FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["OAuth2SecurityService.Service/NuGet.Config", "."]
COPY ["OAuth2SecurityService.Service/OAuth2SecurityService.Service.csproj", "OAuth2SecurityService.Service/"]
COPY ["OAuth2SecurityService.Service/NuGet.Config", "OAuth2SecurityService.Service/"]
COPY ["OAuth2SecurityService.Manager/OAuth2SecurityService.Manager.csproj", "OAuth2SecurityService.Manager/"]
RUN dotnet restore "OAuth2SecurityService.Service/OAuth2SecurityService.Service.csproj"
COPY . .
WORKDIR "/src/OAuth2SecurityService.Service"
RUN dotnet build "OAuth2SecurityService.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OAuth2SecurityService.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OAuth2SecurityService.Service.dll"]