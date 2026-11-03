-- Apply explicitly to a new ITSM database. No application startup schema changes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'identity.Users', N'U') IS NOT NULL
    THROW 50001, 'Identity schema already exists; do not rerun the baseline.', 1;
IF SCHEMA_ID(N'identity') IS NULL EXEC(N'CREATE SCHEMA [identity]');
CREATE TABLE [identity].[Units] ([Id] uniqueidentifier NOT NULL PRIMARY KEY,[Name] nvarchar(150) NOT NULL,[Active] bit NOT NULL);
CREATE TABLE [identity].[Areas] ([Id] uniqueidentifier NOT NULL PRIMARY KEY,[Name] nvarchar(150) NOT NULL,[Active] bit NOT NULL);
CREATE TABLE [identity].[Users] (
 [Id] uniqueidentifier NOT NULL PRIMARY KEY,
 [TenantId] uniqueidentifier NOT NULL,[ObjectId] uniqueidentifier NOT NULL,
 [DisplayName] nvarchar(200) NOT NULL,[State] nvarchar(20) NOT NULL,
 [IsAdministrator] bit NOT NULL,[UnitId] uniqueidentifier NULL,[AreaId] uniqueidentifier NULL,
 [CreatedAt] datetimeoffset NOT NULL,[Version] rowversion NOT NULL,
 CONSTRAINT [CK_Users_State] CHECK ([State] IN ('Pending','Active','Disabled')),
 CONSTRAINT [FK_Users_Units] FOREIGN KEY ([UnitId]) REFERENCES [identity].[Units]([Id]),
 CONSTRAINT [FK_Users_Areas] FOREIGN KEY ([AreaId]) REFERENCES [identity].[Areas]([Id])
);
CREATE UNIQUE INDEX [IX_Users_TenantId_ObjectId] ON [identity].[Users]([TenantId],[ObjectId]);
CREATE TABLE [identity].[Audit] ([Id] bigint IDENTITY PRIMARY KEY,[ActorId] uniqueidentifier NOT NULL,[TargetId] uniqueidentifier NOT NULL,[Action] nvarchar(100) NOT NULL,[At] datetimeoffset NOT NULL);
COMMIT;
