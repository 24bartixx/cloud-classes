FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Inventories/Inventories.Service.csproj", "Inventories/"]
RUN dotnet restore "Inventories/Inventories.Service.csproj"

COPY . .
WORKDIR "/src/Inventories"
RUN dotnet publish "Inventories.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Inventories.Service.dll"]
