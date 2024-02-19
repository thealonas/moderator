FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-arm64v8 AS base
WORKDIR /app
EXPOSE 1222

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-arm64v8 AS build
WORKDIR /src
COPY ["moderator.csproj", "./"]
RUN dotnet restore "moderator.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "moderator.csproj" -c Release -o /app/build --no-self-contained

FROM build AS publish
RUN dotnet publish "moderator.csproj" -c Release -o /app/publish --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "moderator.dll"]
