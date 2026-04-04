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
