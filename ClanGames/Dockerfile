FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["ClanGames/ClanGames.Api.csproj", "ClanGames/"]
RUN dotnet restore "ClanGames/ClanGames.Api.csproj"

COPY . .
WORKDIR "/src/ClanGames"
RUN dotnet publish "ClanGames.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClanGames.Api.dll"]
