FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SecurityService.Service/NuGet.Config", "."]
COPY ["SecurityService.Service/SecurityService.Service.csproj", "SecurityService.Service/"]
COPY ["SecurityService.Service/NuGet.Config", "SecurityService.Service/"]
COPY ["SecurityService.Manager/SecurityService.Manager.csproj", "SecurityService.Manager/"]
RUN dotnet restore "SecurityService.Service/SecurityService.Service.csproj"
COPY . .
WORKDIR "/src/SecurityService.Service"
RUN dotnet build "SecurityService.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SecurityService.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SecurityService.Service.dll"]