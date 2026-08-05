/*
  SQLCMD değişkenleriyle çalıştırın veya DBA tarafından değerleri inceleyerek değiştirin.
  Migration işlemi bu kullanıcıyla yapılmamalıdır; bu kullanıcı yalnız uygulamanın çalışma zamanı içindir.
*/
:setvar DatabaseName "DigitalPano"
:setvar AppPoolName "DigitalPano"

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$(AppPoolName)')
    CREATE LOGIN [IIS APPPOOL\$(AppPoolName)] FROM WINDOWS;
GO

USE [$(DatabaseName)];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\$(AppPoolName)')
    CREATE USER [IIS APPPOOL\$(AppPoolName)] FOR LOGIN [IIS APPPOOL\$(AppPoolName)];
GO

ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\$(AppPoolName)];
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\$(AppPoolName)];
GO
