# Docker Compose

```
docker compose up --build
```

# API

```
dotnet run --project ClanWars.Api/ClanWars.Api.csproj
```

# Services

```
dotnet run --project .\Clans.Service\Clans.Service.csproj
```

```
dotnet run --project .\Inventories.Service\Inventories.Service.csproj
```

```
dotnet run --project .\Notifications.Service\Notifications.Service.csproj
```

```
dotnet run --project .\PlayerMatchesHistory.Service\PlayerMatchesHistory.Service.csproj
```

```
dotnet run --project .\Rewards.Service\Rewards.Service.csproj
```

# Load aws creds

. .\load-aws-env.ps1

# Check my IP

Invoke-RestMethod https://checkip.amazonaws.com

# Push to ECR

### Requirements

```powershell
winget install Amazon.AWSCLI
aws --version
```

### Before

```powershell
. .\load-aws-env.ps1
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 517392773395.dkr.ecr.us-east-1.amazonaws.com
$env:DOCKER_BUILDKIT=0
```

### Push microservices

1. Clan games

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clan-games:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clan-games:latest -f ClanGames/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clan-games:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clan-games:latest
```

2. Clans

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clans:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clans:latest -f Clans/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clans:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-clans:latest
```

3. Inventories

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-inventories:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-inventories:latest -f Inventories/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-inventories:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-inventories:latest
```

4. Notifications

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-notifications:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-notifications:latest -f Notifications/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-notifications:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-notifications:latest
```

5. Player matches history

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-player-matches-history:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-player-matches-history:latest -f PlayerMatchesHistory/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-player-matches-history:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-player-matches-history:latest
```

6. Rewards

```powershell
docker build -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-rewards:v1.0.0 -t 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-rewards:latest -f Rewards/Dockerfile .
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-rewards:v1.0.0
docker push 517392773395.dkr.ecr.us-east-1.amazonaws.com/tanks-rewards:latest
```
