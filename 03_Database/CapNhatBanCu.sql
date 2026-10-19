USE [AutoraDb];
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    DROP INDEX [IX_DepositRequests_CustomerId_CarId] ON [DepositRequests];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    ALTER TABLE [DepositRequests] DROP CONSTRAINT [CK_Request_Status];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    CREATE TABLE [ContactInquiries] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(150) NOT NULL,
        [Phone] nvarchar(20) NULL,
        [Email] nvarchar(256) NULL,
        [InterestedModel] nvarchar(100) NULL,
        [Message] nvarchar(1000) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_ContactInquiries] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    CREATE TABLE [ShowroomSettings] (
        [Id] int NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Hotline] nvarchar(50) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Address] nvarchar(300) NOT NULL,
        [OpeningHours] nvarchar(100) NULL,
        [Policy] nvarchar(1000) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_ShowroomSettings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_DepositRequests_CustomerId_CarId] ON [DepositRequests] ([CustomerId], [CarId]) WHERE [Status] IN (0, 3)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    EXEC(N'ALTER TABLE [DepositRequests] ADD CONSTRAINT [CK_Request_Status] CHECK ([Status] BETWEEN 0 AND 3)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019170014_CompleteShowroomWorkflow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019170014_CompleteShowroomWorkflow', N'8.0.11');
END;
GO

COMMIT;
GO

