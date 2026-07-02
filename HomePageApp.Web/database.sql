IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [ToDoItems] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [DueDate] datetime2 NULL,
    [CompletedDate] datetime2 NULL,
    CONSTRAINT [PK_ToDoItems] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260617172927_InitialCreate', N'10.0.9');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Bills] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Reoccurring] bit NOT NULL,
    [Frequency] nvarchar(max) NULL,
    [StartDue] datetime2 NOT NULL,
    [NextDue] datetime2 NULL,
    [LastPaid] datetime2 NOT NULL,
    [EstimatedAmountDue] float NOT NULL,
    [PaymentUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_Bills] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260630183355_AddBillModelTable', N'10.0.9');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bills]') AND [c].[name] = N'EstimatedAmountDue');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Bills] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Bills] ALTER COLUMN [EstimatedAmountDue] float NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260630190711_ModifyingBillModel', N'10.0.9');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bills]') AND [c].[name] = N'LastPaid');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Bills] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [Bills] ALTER COLUMN [LastPaid] datetime2 NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260701165032_ModifyBillModel_LastPaid', N'10.0.9');

COMMIT;
GO

BEGIN TRANSACTION;
EXEC sp_rename N'[Bills].[LastPaid]', N'LastPaidOrSeen', 'COLUMN';

ALTER TABLE [Bills] ADD [AutoDraft] bit NOT NULL DEFAULT CAST(0 AS bit);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260701225522_ModelBillAddAutoDraft', N'10.0.9');

COMMIT;
GO

