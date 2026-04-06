FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Notifications/Notifications.Service.csproj", "Notifications/"]
RUN dotnet restore "Notifications/Notifications.Service.csproj"

COPY . .
WORKDIR "/src/Notifications"
RUN dotnet publish "Notifications.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Notifications.Service.dll"]
