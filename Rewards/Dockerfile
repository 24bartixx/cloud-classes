FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Rewards/Rewards.Service.csproj", "Rewards/"]
RUN dotnet restore "Rewards/Rewards.Service.csproj"

COPY . .
WORKDIR "/src/Rewards"
RUN dotnet publish "Rewards.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Rewards.Service.dll"]
