FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Clans/Clans.Service.csproj", "Clans/"]
RUN dotnet restore "Clans/Clans.Service.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/Clans"
RUN dotnet publish "Clans.Service.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Clans.Service.dll"]
