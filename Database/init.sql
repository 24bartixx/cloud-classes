IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MicroservicesDb')
BEGIN
    CREATE DATABASE MicroservicesDb;
END
GO
USE MicroservicesDb;
GO

-- Rewards Service
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'RewardsLogin')
BEGIN
    CREATE LOGIN RewardsLogin WITH PASSWORD = 'StrongPassword123!';
END
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'RewardsUser')
BEGIN
    CREATE USER RewardsUser FOR LOGIN RewardsLogin;
END
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'RewardsSchema')
BEGIN
    EXEC('CREATE SCHEMA RewardsSchema');
END
ALTER USER RewardsUser WITH DEFAULT_SCHEMA = RewardsSchema;
GRANT CONTROL ON SCHEMA::RewardsSchema TO RewardsUser;
GRANT CREATE TABLE TO RewardsUser;

-- Notifications Service
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NotificationsLogin')
BEGIN
    CREATE LOGIN NotificationsLogin WITH PASSWORD = 'StrongPassword456!';
END
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'NotificationsUser')
BEGIN
    CREATE USER NotificationsUser FOR LOGIN NotificationsLogin;
END
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'NotificationsSchema')
BEGIN
    EXEC('CREATE SCHEMA NotificationsSchema');
END
ALTER USER NotificationsUser WITH DEFAULT_SCHEMA = NotificationsSchema;
GRANT CONTROL ON SCHEMA::NotificationsSchema TO NotificationsUser;
GRANT CREATE TABLE TO NotificationsUser;

-- Clans Service
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'ClansLogin')
BEGIN
    CREATE LOGIN ClansLogin WITH PASSWORD = 'StrongPassword789!';
END
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'ClansUser')
BEGIN
    CREATE USER ClansUser FOR LOGIN ClansLogin;
END
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ClansSchema')
BEGIN
    EXEC('CREATE SCHEMA ClansSchema');
END
ALTER USER ClansUser WITH DEFAULT_SCHEMA = ClansSchema;
GRANT CONTROL ON SCHEMA::ClansSchema TO ClansUser;
GRANT CREATE TABLE TO ClansUser;
GO

-- Create Tables and Seed Data
USE MicroservicesDb;
GO

-- Rewards Tables
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[RewardsSchema].[Rewards]') AND type in (N'U'))
BEGIN
    CREATE TABLE [RewardsSchema].[Rewards] (
        [RewardId] UNIQUEIDENTIFIER PRIMARY KEY,
        [MinKills] INT NULL,
        [MaxDeaths] INT NULL,
        [MinDamageDealt] INT NULL,
        [MaxDamageReceived] INT NULL,
        [MinSurvived] INT NULL,
        [MinExperienceEarned] INT NULL,
        [TankId] NVARCHAR(MAX) NULL,
        [Experience] INT DEFAULT 0 NOT NULL,
        [Credits] INT DEFAULT 0 NOT NULL
    );

    -- Seed 40 Rewards
    DECLARE @j INT = 1;
    WHILE @j <= 40
    BEGIN
        INSERT INTO [RewardsSchema].[Rewards] (RewardId, MinKills, MaxDeaths, MinDamageDealt, MaxDamageReceived, MinSurvived, MinExperienceEarned, TankId, Experience, Credits)
        VALUES (
            CAST(CAST(@j + 3000 AS BINARY(16)) AS UNIQUEIDENTIFIER), 
            CASE WHEN @j % 5 = 0 THEN NULL ELSE (@j % 10) + 1 END,
            CASE WHEN @j % 7 = 0 THEN NULL ELSE (@j % 5) END,
            CASE WHEN @j % 3 = 0 THEN NULL ELSE @j * 100 END,
            CASE WHEN @j % 4 = 0 THEN NULL ELSE @j * 50 END,
            CASE WHEN @j % 6 = 0 THEN NULL ELSE 1 END,
            CASE WHEN @j % 2 = 0 THEN NULL ELSE @j * 20 END,
            CASE WHEN @j % 10 = 0 THEN 'T-34' WHEN @j % 10 = 1 THEN 'Tiger I' ELSE NULL END,
            @j * 50,
            @j * 1000
        );
        SET @j = @j + 1;
    END
END

-- Clans Tables
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ClansSchema].[Clans]') AND type in (N'U'))
BEGIN
    CREATE TABLE [ClansSchema].[Clans] (
        [ClanId] UNIQUEIDENTIFIER PRIMARY KEY,
        [ClanName] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL
    );

    -- Seed 10 Clans
    DECLARE @i INT = 1;
    WHILE @i <= 10
    BEGIN
        INSERT INTO [ClansSchema].[Clans] (ClanId, ClanName, CreatedAt)
        VALUES (CAST(CAST(@i AS BINARY(16)) AS UNIQUEIDENTIFIER), 'Clan ' + CAST(@i AS NVARCHAR(2)), GETUTCDATE());
        SET @i = @i + 1;
    END
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ClansSchema].[ClanWarResults]') AND type in (N'U'))
BEGIN
    CREATE TABLE [ClansSchema].[ClanWarResults] (
        [ClanWarId] UNIQUEIDENTIFIER PRIMARY KEY,
        [FinishDate] DATETIME2 NOT NULL,
        [TotalClans] INT NOT NULL
    );

    -- Seed 70 Wars
    SET @i = 1;
    WHILE @i <= 70
    BEGIN
        INSERT INTO [ClansSchema].[ClanWarResults] (ClanWarId, FinishDate, TotalClans)
        VALUES (CAST(CAST(@i + 1000 AS BINARY(16)) AS UNIQUEIDENTIFIER), DATEADD(HOUR, -@i, GETUTCDATE()), (@i % 3) + 2);
        SET @i = @i + 1;
    END
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ClansSchema].[ClansResults]') AND type in (N'U'))
BEGIN
    CREATE TABLE [ClansSchema].[ClansResults] (
        [ClanResultId] UNIQUEIDENTIFIER PRIMARY KEY,
        [ClanId] UNIQUEIDENTIFIER NOT NULL,
        [ClanWarId] UNIQUEIDENTIFIER NOT NULL,
        [Placement] INT NOT NULL,
        [Score] INT NOT NULL,
        CONSTRAINT FK_ClansResults_Clans FOREIGN KEY (ClanId) REFERENCES [ClansSchema].[Clans](ClanId),
        CONSTRAINT FK_ClansResults_ClanWarResults FOREIGN KEY (ClanWarId) REFERENCES [ClansSchema].[ClanWarResults](ClanWarId)
    );

    -- Seed 300 Results
    SET @i = 1;
    WHILE @i <= 300
    BEGIN
        INSERT INTO [ClansSchema].[ClansResults] (ClanResultId, ClanId, ClanWarId, Placement, Score)
        VALUES (
            CAST(CAST(@i + 2000 AS BINARY(16)) AS UNIQUEIDENTIFIER),
            CAST(CAST((@i % 10) + 1 AS BINARY(16)) AS UNIQUEIDENTIFIER),
            CAST(CAST((@i % 70) + 1001 AS BINARY(16)) AS UNIQUEIDENTIFIER),
            (@i % 4) + 1,
            1000 + (@i * 10)
        );
        SET @i = @i + 1;
    END
END
GO
