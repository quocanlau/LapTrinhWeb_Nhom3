-- Tao database neu chua co. Khong xoa du lieu hien co.
IF DB_ID(N'AutoraDb') IS NULL EXEC(N'CREATE DATABASE [AutoraDb]');
GO
USE [AutoraDb];
GO

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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [IsActive] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [Manufacturers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Country] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Manufacturers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [SupplierType] nvarchar(30) NOT NULL,
        [Phone] nvarchar(20) NULL,
        [Address] nvarchar(300) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [FullName] nvarchar(150) NOT NULL,
        [Phone] nvarchar(20) NOT NULL,
        [Email] nvarchar(256) NULL,
        [Address] nvarchar(300) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Customers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [CarModels] (
        [Id] int NOT NULL IDENTITY,
        [ManufacturerId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [BodyType] nvarchar(30) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_CarModels] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarModels_Manufacturers_ManufacturerId] FOREIGN KEY ([ManufacturerId]) REFERENCES [Manufacturers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [PurchaseReceipts] (
        [Id] int NOT NULL IDENTITY,
        [SupplierId] int NOT NULL,
        [EmployeeId] nvarchar(450) NOT NULL,
        [PurchasedAt] datetimeoffset NOT NULL,
        [Status] tinyint NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PurchaseReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Purchase_Status] CHECK ([Status] IN (0, 1)),
        CONSTRAINT [FK_PurchaseReceipts_AspNetUsers_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseReceipts_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [Cars] (
        [Id] int NOT NULL IDENTITY,
        [CarModelId] int NOT NULL,
        [VIN] varchar(17) NOT NULL,
        [ManufactureYear] smallint NOT NULL,
        [Color] nvarchar(50) NOT NULL,
        [Condition] tinyint NOT NULL,
        [Mileage] int NOT NULL,
        [Fuel] nvarchar(30) NOT NULL,
        [Transmission] nvarchar(30) NOT NULL,
        [ListedPrice] decimal(18,2) NOT NULL,
        [Status] tinyint NOT NULL,
        [IsPublished] bit NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Cars] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Car_Condition] CHECK ([Condition] IN (0, 1)),
        CONSTRAINT [CK_Car_Mileage] CHECK ([Mileage] >= 0),
        CONSTRAINT [CK_Car_Price] CHECK ([ListedPrice] > 0),
        CONSTRAINT [CK_Car_Status] CHECK ([Status] BETWEEN 0 AND 4),
        CONSTRAINT [CK_Car_Year] CHECK ([ManufactureYear] BETWEEN 1900 AND 2100),
        CONSTRAINT [FK_Cars_CarModels_CarModelId] FOREIGN KEY ([CarModelId]) REFERENCES [CarModels] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [CarImages] (
        [Id] int NOT NULL IDENTITY,
        [CarId] int NOT NULL,
        [ImagePath] nvarchar(1000) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [SortOrder] int NOT NULL,
        CONSTRAINT [PK_CarImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarImages_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [DepositRequests] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [CarId] int NOT NULL,
        [Note] nvarchar(1000) NULL,
        [Status] tinyint NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_DepositRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Request_Status] CHECK ([Status] BETWEEN 0 AND 2),
        CONSTRAINT [FK_DepositRequests_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DepositRequests_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [PurchaseDetails] (
        [Id] int NOT NULL IDENTITY,
        [PurchaseReceiptId] int NOT NULL,
        [CarId] int NOT NULL,
        [PurchasePrice] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PurchaseDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PurchaseDetail_Price] CHECK ([PurchasePrice] > 0),
        CONSTRAINT [FK_PurchaseDetails_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseDetails_PurchaseReceipts_PurchaseReceiptId] FOREIGN KEY ([PurchaseReceiptId]) REFERENCES [PurchaseReceipts] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [SaleTransactions] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [CarId] int NOT NULL,
        [DepositRequestId] int NULL,
        [EmployeeId] nvarchar(450) NOT NULL,
        [AgreedPrice] decimal(18,2) NOT NULL,
        [RequiredDeposit] decimal(18,2) NOT NULL,
        [Status] tinyint NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [ConfirmedAt] datetimeoffset NULL,
        [UpdatedAt] datetimeoffset NOT NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_SaleTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Sale_Deposit] CHECK ([RequiredDeposit] = ROUND([AgreedPrice] * 0.02, 0)),
        CONSTRAINT [CK_Sale_Price] CHECK ([AgreedPrice] > 0),
        CONSTRAINT [CK_Sale_Status] CHECK ([Status] BETWEEN 0 AND 4),
        CONSTRAINT [FK_SaleTransactions_AspNetUsers_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleTransactions_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleTransactions_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleTransactions_DepositRequests_DepositRequestId] FOREIGN KEY ([DepositRequestId]) REFERENCES [DepositRequests] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [Deliveries] (
        [Id] int NOT NULL IDENTITY,
        [SaleTransactionId] int NOT NULL,
        [DeliveredAt] datetimeoffset NOT NULL,
        [Recipient] nvarchar(150) NOT NULL,
        [Note] nvarchar(500) NULL,
        CONSTRAINT [PK_Deliveries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Deliveries_SaleTransactions_SaleTransactionId] FOREIGN KEY ([SaleTransactionId]) REFERENCES [SaleTransactions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE TABLE [PaymentEntries] (
        [Id] int NOT NULL IDENTITY,
        [SaleTransactionId] int NOT NULL,
        [CreatedById] nvarchar(450) NOT NULL,
        [EntryType] tinyint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentMethod] nvarchar(50) NOT NULL,
        [Note] nvarchar(500) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_PaymentEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Payment_Amount] CHECK ([Amount] > 0),
        CONSTRAINT [CK_Payment_Type] CHECK ([EntryType] IN (0, 1)),
        CONSTRAINT [FK_PaymentEntries_AspNetUsers_CreatedById] FOREIGN KEY ([CreatedById]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PaymentEntries_SaleTransactions_SaleTransactionId] FOREIGN KEY ([SaleTransactionId]) REFERENCES [SaleTransactions] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_CarImages_CarId] ON [CarImages] ([CarId]) WHERE [IsPrimary] = 1');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CarModels_ManufacturerId_Name] ON [CarModels] ([ManufacturerId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_Cars_CarModelId] ON [Cars] ([CarModelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_Cars_IsPublished_Status_CarModelId] ON [Cars] ([IsPublished], [Status], [CarModelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Cars_VIN] ON [Cars] ([VIN]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Customers_UserId] ON [Customers] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Deliveries_SaleTransactionId] ON [Deliveries] ([SaleTransactionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_DepositRequests_CarId] ON [DepositRequests] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_DepositRequests_CustomerId_CarId] ON [DepositRequests] ([CustomerId], [CarId]) WHERE [Status] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Manufacturers_Name] ON [Manufacturers] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_PaymentEntries_CreatedById] ON [PaymentEntries] ([CreatedById]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_PaymentEntries_SaleTransactionId] ON [PaymentEntries] ([SaleTransactionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseDetails_CarId] ON [PurchaseDetails] ([CarId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_PurchaseDetails_PurchaseReceiptId] ON [PurchaseDetails] ([PurchaseReceiptId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_PurchaseReceipts_EmployeeId] ON [PurchaseReceipts] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_PurchaseReceipts_SupplierId] ON [PurchaseReceipts] ([SupplierId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_SaleTransactions_CarId] ON [SaleTransactions] ([CarId]) WHERE [Status] IN (1, 2, 3)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_SaleTransactions_CustomerId] ON [SaleTransactions] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_SaleTransactions_DepositRequestId] ON [SaleTransactions] ([DepositRequestId]) WHERE [DepositRequestId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    CREATE INDEX [IX_SaleTransactions_EmployeeId] ON [SaleTransactions] ([EmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008194209_InitialAutora'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008194209_InitialAutora', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016135000_AddAdminNotificationReads'
)
BEGIN
    CREATE TABLE [AdminNotificationReads] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [EventKey] nvarchar(100) NOT NULL,
        [ReadAt] datetimeoffset NOT NULL,
        CONSTRAINT [PK_AdminNotificationReads] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdminNotificationReads_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016135000_AddAdminNotificationReads'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdminNotificationReads_UserId_EventKey] ON [AdminNotificationReads] ([UserId], [EventKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016135000_AddAdminNotificationReads'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016135000_AddAdminNotificationReads', N'8.0.11');
END;
GO

COMMIT;
GO

