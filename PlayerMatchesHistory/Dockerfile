FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["PlayerMatchesHistory/PlayerMatchesHistory.Service.csproj", "PlayerMatchesHistory/"]
RUN dotnet restore "PlayerMatchesHistory/PlayerMatchesHistory.Service.csproj"

COPY . .
WORKDIR "/src/PlayerMatchesHistory"
RUN dotnet publish "PlayerMatchesHistory.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PlayerMatchesHistory.Service.dll"]
