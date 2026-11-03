-- Fill GUIDs and names in a LOCAL copy only. Run once by an authorized DBA after baseline.
-- These zero GUIDs deliberately prevent accidental execution.
SET XACT_ABORT ON;
DECLARE @tenant uniqueidentifier='00000000-0000-0000-0000-000000000000';
DECLARE @object uniqueidentifier='00000000-0000-0000-0000-000000000000';
DECLARE @unit uniqueidentifier='00000000-0000-0000-0000-000000000000';
DECLARE @area uniqueidentifier='00000000-0000-0000-0000-000000000000';
DECLARE @unitName nvarchar(150)=N'CONFIGURAR';
DECLARE @areaName nvarchar(150)=N'CONFIGURAR';
IF @tenant='00000000-0000-0000-0000-000000000000' OR @object='00000000-0000-0000-0000-000000000000'
 OR @unit='00000000-0000-0000-0000-000000000000' OR @area='00000000-0000-0000-0000-000000000000'
 OR @unitName=N'CONFIGURAR' OR @areaName=N'CONFIGURAR'
 THROW 50002, 'Configure explicit directory IDs and organizational records first.',1;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;
IF EXISTS(SELECT 1 FROM [identity].[Users] WITH(UPDLOCK,HOLDLOCK) WHERE [IsAdministrator]=1 AND [TenantId]=@tenant)
 THROW 50003, 'An administrator is already provisioned. Use a controlled administrative change.',1;
IF NOT EXISTS(SELECT 1 FROM [identity].[Units] WHERE [Id]=@unit)
 INSERT [identity].[Units] VALUES(@unit,@unitName,1);
IF NOT EXISTS(SELECT 1 FROM [identity].[Areas] WHERE [Id]=@area)
 INSERT [identity].[Areas] VALUES(@area,@areaName,1);
DECLARE @id uniqueidentifier=(SELECT [Id] FROM [identity].[Users] WHERE [TenantId]=@tenant AND [ObjectId]=@object);
IF @id IS NULL BEGIN
 SET @id=NEWID();
 INSERT [identity].[Users]([Id],[TenantId],[ObjectId],[DisplayName],[State],[IsAdministrator],[UnitId],[AreaId],[CreatedAt])
 VALUES(@id,@tenant,@object,N'Administrador inicial','Active',1,@unit,@area,SYSUTCDATETIME());
END ELSE BEGIN
 UPDATE [identity].[Users] SET [State]='Active',[IsAdministrator]=1,[UnitId]=@unit,[AreaId]=@area WHERE [Id]=@id;
END;
INSERT [identity].[Audit]([ActorId],[TargetId],[Action],[At]) VALUES(@id,@id,'ExplicitAdminBootstrap',SYSUTCDATETIME());
COMMIT;
