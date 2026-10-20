-- AUTORA: schema and data exported by the group, portable installation.
-- Run the ENTIRE file once on a local SQL Server 2019+ / LocalDB connection.
-- Existing AutoraDb is never dropped, overwritten or merged.
-- No SQLCMD mode is needed. Do not select only a portion of this file.
IF @@TRANCOUNT > 0
BEGIN
    RAISERROR(N'Hay chay script trong cua so query moi, khong co transaction dang mo.', 16, 1);
    RETURN;
END;
USE [master];
SET IMPLICIT_TRANSACTIONS OFF;
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @database sysname = N'AutoraDb';
DECLARE @lockName nvarchar(255) = N'Autora.Install.Full.' + LOWER(@database);
DECLARE @lockResult int;
EXEC @lockResult = sys.sp_getapplock
    @Resource=@lockName, @LockMode='Exclusive',
    @LockOwner='Session', @LockTimeout=60000;
IF @lockResult < 0
    THROW 51000, N'Khong lay duoc khoa cai dat. Hay thu lai sau.', 1;
BEGIN TRY
    IF NULLIF(LTRIM(RTRIM(@database)), N'') IS NULL
        THROW 51004, N'Ten database khong hop le.', 1;
    IF DB_ID(@database) IS NOT NULL
        THROW 51001, N'AutoraDb da ton tai. Script dung lai de giu nguyen du lieu. Xem huong dan de dung DB cu hoac cai rieng.', 1;
    IF CONVERT(int, SERVERPROPERTY('ProductMajorVersion')) < 15
        THROW 51002, N'Can SQL Server 2019 / LocalDB major 15 hoac moi hon.', 1;
    DECLARE @databaseSql nvarchar(max);
    SET @databaseSql = N'CREATE DATABASE ' + QUOTENAME(@database);
    EXEC sys.sp_executesql @databaseSql;
    SET @databaseSql = N'ALTER DATABASE ' + QUOTENAME(@database) + N' SET COMPATIBILITY_LEVEL = 150';
    EXEC sys.sp_executesql @databaseSql;
    SET @databaseSql = N'ALTER DATABASE ' + QUOTENAME(@database) + N' SET AUTO_CLOSE OFF';
    EXEC sys.sp_executesql @databaseSql;
    SET @databaseSql = N'ALTER DATABASE ' + QUOTENAME(@database) + N' SET RECOVERY SIMPLE';
    EXEC sys.sp_executesql @databaseSql;
    SET @databaseSql = N'ALTER DATABASE ' + QUOTENAME(@database) + N' SET READ_COMMITTED_SNAPSHOT ON';
    EXEC sys.sp_executesql @databaseSql;
    DECLARE @schemaAndData nvarchar(max) = N'
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[AdminNotificationReads]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AdminNotificationReads](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[EventKey] [nvarchar](100) NOT NULL,
	[ReadAt] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_AdminNotificationReads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[FullName] [nvarchar](150) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
/****** Object:  Table [dbo].[CarImages]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[CarImages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CarId] [int] NOT NULL,
	[ImagePath] [nvarchar](1000) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_CarImages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[CarModels]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[CarModels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManufacturerId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[BodyType] [nvarchar](30) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_CarModels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[Cars]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Cars](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CarModelId] [int] NOT NULL,
	[VIN] [varchar](17) NOT NULL,
	[ManufactureYear] [smallint] NOT NULL,
	[Color] [nvarchar](50) NOT NULL,
	[Condition] [tinyint] NOT NULL,
	[Mileage] [int] NOT NULL,
	[Fuel] [nvarchar](30) NOT NULL,
	[Transmission] [nvarchar](30) NOT NULL,
	[ListedPrice] [decimal](18, 2) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Cars] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[ContactInquiries]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[ContactInquiries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](150) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](256) NULL,
	[InterestedModel] [nvarchar](100) NULL,
	[Message] [nvarchar](1000) NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ContactInquiries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[Customers]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NULL,
	[FullName] [nvarchar](150) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[Address] [nvarchar](300) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[Deliveries]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Deliveries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleTransactionId] [int] NOT NULL,
	[DeliveredAt] [datetimeoffset](7) NOT NULL,
	[Recipient] [nvarchar](150) NOT NULL,
	[Note] [nvarchar](500) NULL,
 CONSTRAINT [PK_Deliveries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[DepositRequests]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[DepositRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CarId] [int] NOT NULL,
	[Note] [nvarchar](1000) NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_DepositRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[Manufacturers]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Manufacturers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Country] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Manufacturers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[PaymentEntries]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[PaymentEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleTransactionId] [int] NOT NULL,
	[CreatedById] [nvarchar](450) NOT NULL,
	[EntryType] [tinyint] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[Note] [nvarchar](500) NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_PaymentEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[PurchaseDetails]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[PurchaseDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseReceiptId] [int] NOT NULL,
	[CarId] [int] NOT NULL,
	[PurchasePrice] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_PurchaseDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[PurchaseReceipts]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[PurchaseReceipts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [int] NOT NULL,
	[EmployeeId] [nvarchar](450) NOT NULL,
	[PurchasedAt] [datetimeoffset](7) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PurchaseReceipts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[SaleTransactions]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[SaleTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CarId] [int] NOT NULL,
	[DepositRequestId] [int] NULL,
	[EmployeeId] [nvarchar](450) NOT NULL,
	[AgreedPrice] [decimal](18, 2) NOT NULL,
	[RequiredDeposit] [decimal](18, 2) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[ConfirmedAt] [datetimeoffset](7) NULL,
	[UpdatedAt] [datetimeoffset](7) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_SaleTransactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[ShowroomSettings]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[ShowroomSettings](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Hotline] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Address] [nvarchar](300) NOT NULL,
	[OpeningHours] [nvarchar](100) NULL,
	[Policy] [nvarchar](1000) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ShowroomSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
/****** Object:  Table [dbo].[Suppliers]    Script Date: 20-Oct-26 11:48:39 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[Suppliers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[SupplierType] [nvarchar](30) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Address] [nvarchar](300) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N''20261008194209_InitialAutora'', N''8.0.11'')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N''20261016135000_AddAdminNotificationReads'', N''8.0.11'')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N''20261019170014_CompleteShowroomWorkflow'', N''8.0.11'')
SET IDENTITY_INSERT [dbo].[AdminNotificationReads] ON 

INSERT [dbo].[AdminNotificationReads] ([Id], [UserId], [EventKey], [ReadAt]) VALUES (1, N''075409ca-0540-4e01-b608-e3ef06f532e2'', N''request:3'', CAST(N''2026-10-16T14:36:15.2194643+00:00'' AS DateTimeOffset))
SET IDENTITY_INSERT [dbo].[AdminNotificationReads] OFF
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N''229a2d75-0b8b-4b8f-8563-833a7295793b'', N''Customer'', N''CUSTOMER'', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N''2d4fab66-202d-4111-8e37-7ab76e15b4f5'', N''Employee'', N''EMPLOYEE'', NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N''c526b678-1dd0-4ed0-8d06-bd447d22662a'', N''Admin'', N''ADMIN'', NULL)
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N''3f2f8ec7-74bb-4385-8bd4-e754dfd9b538'', N''229a2d75-0b8b-4b8f-8563-833a7295793b'')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N''b61644a6-6276-4e72-9c80-c6a6dcbe0618'', N''229a2d75-0b8b-4b8f-8563-833a7295793b'')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N''d04fc359-015e-4769-a746-b1da2a4df6ff'', N''229a2d75-0b8b-4b8f-8563-833a7295793b'')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N''075409ca-0540-4e01-b608-e3ef06f532e2'', N''c526b678-1dd0-4ed0-8d06-bd447d22662a'')
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [IsActive], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N''075409ca-0540-4e01-b608-e3ef06f532e2'', N''Quản trị Autora'', 1, N''admin@autora.local'', N''ADMIN@AUTORA.LOCAL'', N''admin@autora.local'', N''ADMIN@AUTORA.LOCAL'', 1, N''AQAAAAIAAYagAAAAEFF7m0+u/CeJ0cKHoAm20pXmgTHca2ikod9tg0fechQWlweEeVaFBakkfd98asxVtQ=='', N''QHIORRGVGTHH3QQ4WQON4CNCZFO2MYQQ'', N''ae498b00-ef05-41c4-a63a-8fde7b8a3eff'', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [IsActive], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N''3f2f8ec7-74bb-4385-8bd4-e754dfd9b538'', N''Lầu An'', 1, N''an@gmail.com'', N''AN@GMAIL.COM'', N''an@gmail.com'', N''AN@GMAIL.COM'', 0, N''AQAAAAIAAYagAAAAECGpIwba/k8kKse79UHaJpyjHmlva3nfzXKZFIyGtnFEMi3UsfgCJXvXnY5cP7V9Vg=='', N''QP7IXYSUCDXRCR6QH76R4CBJSVLFIWOF'', N''d1341040-bf0b-4064-a63a-a8e84543f514'', N''0123456789'', 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [IsActive], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N''b61644a6-6276-4e72-9c80-c6a6dcbe0618'', N''an'', 1, N''quocanlau@gmail.com'', N''QUOCANLAU@GMAIL.COM'', N''quocanlau@gmail.com'', N''QUOCANLAU@GMAIL.COM'', 0, N''AQAAAAIAAYagAAAAEEKLkHlf7k6DOnGfIn4u35/r8g4gX9Dlvf5OXQqx4CDOa/iDalAQXQt0k+3fHHE6lg=='', N''BQS6PD4FMNQF7XSWKKVG22CWISJGQDFX'', N''ccf74fbb-b377-41ef-bf84-74209850a7e4'', N''01'', 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [FullName], [IsActive], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N''d04fc359-015e-4769-a746-b1da2a4df6ff'', N''dung'', 1, N''dung@gmail.com'', N''DUNG@GMAIL.COM'', N''dung@gmail.com'', N''DUNG@GMAIL.COM'', 0, N''AQAAAAIAAYagAAAAEMGIMK3E9P0DkrI1cTMPmONEJbE3f80RrJBNpvNwfKdjDXBPMraorvUJI74h8JcjNg=='', N''4L44WRVXDORD22R6QZNLVJOPOP7OELLS'', N''47fe8c37-fdba-4985-b12a-fa6e804e567f'', N''0123456789'', 0, 0, NULL, 1, 0)
SET IDENTITY_INSERT [dbo].[CarImages] ON 

INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (1, 1, N''/images/cars/toyota-vios.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (2, 2, N''/images/cars/toyota-yaris-cross-c-230920-05.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (3, 3, N''/images/cars/toyota-yaris-cross.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (4, 4, N''/images/cars/toyota-corolla-cross.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (5, 5, N''/images/cars/toyota-corolla-cross-19943.webp'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (6, 6, N''/images/cars/honda-city.png'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (7, 7, N''/images/cars/2135235-2139205j29545.webp'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (8, 8, N''/images/cars/9y0tzmv-2885400j33687.webp'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (9, 9, N''/images/cars/xpanderpreat.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (10, 10, N''/images/cars/mitsubishiattragemt.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (11, 11, N''/images/cars/mitsubishi-attrage-cvt-premium-2236481j32336.webp'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (12, 12, N''/images/cars/territory.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (13, 13, N''/images/cars/ford-territory.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (14, 14, N''/images/cars/territory-den-dau-xe-trai-truoc.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (15, 15, N''/images/cars/2024-toyota-camry-102-64cbc4858e198.jpg'', 1, 0)
INSERT [dbo].[CarImages] ([Id], [CarId], [ImagePath], [IsPrimary], [SortOrder]) VALUES (16, 16, N''/images/cars/camrytrang.jpg'', 1, 0)
SET IDENTITY_INSERT [dbo].[CarImages] OFF
SET IDENTITY_INSERT [dbo].[CarModels] ON 

INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (1, 1, N''Camry'', N''Sedan'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (2, 1, N''Corolla Cross 1.8HEV'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (3, 1, N''Corolla Cross 1.8V'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (4, 1, N''Vios 1.5E CVT'', N''Sedan'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (5, 1, N''Yaris Cross'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (6, 1, N''Yaris Cross HEV'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (7, 2, N''Territory Titanium 1.5L AT'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (8, 2, N''Territory Titanium X 1.5L AT'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (9, 2, N''Territory Trend 1.5L AT'', N''SUV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (10, 3, N''City G'', N''Sedan'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (11, 4, N''Attrage CVT Premium'', N''Sedan'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (12, 4, N''Attrage MT'', N''Sedan'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (13, 4, N''Xpander AT'', N''MPV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (14, 4, N''Xpander AT Premium'', N''MPV'', 1)
INSERT [dbo].[CarModels] ([Id], [ManufacturerId], [Name], [BodyType], [IsActive]) VALUES (15, 4, N''Xpander MT'', N''MPV'', 1)
SET IDENTITY_INSERT [dbo].[CarModels] OFF
SET IDENTITY_INSERT [dbo].[Cars] ON 

INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (1, 4, N''ATRA0000000000001'', 2026, N''Đen'', 0, 0, N''Xăng'', N''CVT'', CAST(488000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0196166+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (2, 5, N''ATRA0000000000002'', 2026, N''Đen'', 0, 0, N''Xăng'', N''CVT'', CAST(650000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199939+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (3, 6, N''ATRA0000000000003'', 2026, N''Đen'', 0, 0, N''Hybrid'', N''CVT'', CAST(728000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199961+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (4, 3, N''ATRA0000000000004'', 2026, N''Đen'', 0, 0, N''Xăng'', N''CVT'', CAST(820000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199966+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (5, 2, N''ATRA0000000000005'', 2026, N''Đen'', 0, 0, N''Hybrid'', N''e-CVT'', CAST(865000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199969+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (6, 10, N''ATRA0000000000006'', 2026, N''Đen'', 0, 0, N''Xăng'', N''CVT'', CAST(499000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199977+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (7, 15, N''ATRA0000000000007'', 2026, N''Trắng'', 0, 0, N''Xăng'', N''Số sàn 5 cấp'', CAST(568000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199980+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (8, 13, N''ATRA0000000000008'', 2026, N''Trắng'', 0, 0, N''Xăng'', N''Tự động 4 cấp'', CAST(598000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199982+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (9, 14, N''ATRA0000000000009'', 2026, N''Đen'', 0, 0, N''Xăng'', N''Tự động 4 cấp'', CAST(659000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199985+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (10, 12, N''ATRA0000000000010'', 2026, N''Trắng'', 0, 0, N''Xăng'', N''Số sàn 5 cấp'', CAST(380000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199988+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (11, 11, N''ATRA0000000000011'', 2026, N''Xám'', 0, 0, N''Xăng'', N''CVT'', CAST(490000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199990+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (12, 9, N''ATRA0000000000012'', 2026, N''Đen'', 0, 0, N''Xăng'', N''Tự động 7 cấp'', CAST(739000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0199992+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (13, 7, N''ATRA0000000000013'', 2026, N''Đen'', 0, 0, N''Xăng'', N''Tự động 7 cấp'', CAST(819000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0200056+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (14, 8, N''ATRA0000000000014'', 2026, N''Đen'', 0, 0, N''Xăng'', N''Tự động 7 cấp'', CAST(875000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0200059+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (15, 1, N''ATRA0000000000015'', 2024, N''Đỏ'', 1, 15000, N''Hybrid'', N''Tự động'', CAST(950000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0200061+00:00'' AS DateTimeOffset))
INSERT [dbo].[Cars] ([Id], [CarModelId], [VIN], [ManufactureYear], [Color], [Condition], [Mileage], [Fuel], [Transmission], [ListedPrice], [Status], [IsPublished], [CreatedAt]) VALUES (16, 1, N''ATRA0000000000016'', 2024, N''Trắng'', 0, 0, N''Xăng'', N''Tự động'', CAST(960000000.00 AS Decimal(18, 2)), 1, 1, CAST(N''2026-10-09T13:47:48.0200063+00:00'' AS DateTimeOffset))
SET IDENTITY_INSERT [dbo].[Cars] OFF
SET IDENTITY_INSERT [dbo].[Customers] ON 

INSERT [dbo].[Customers] ([Id], [UserId], [FullName], [Phone], [Email], [Address]) VALUES (1, N''3f2f8ec7-74bb-4385-8bd4-e754dfd9b538'', N''Lầu An'', N''0123456789'', N''an@gmail.com'', NULL)
INSERT [dbo].[Customers] ([Id], [UserId], [FullName], [Phone], [Email], [Address]) VALUES (2, N''d04fc359-015e-4769-a746-b1da2a4df6ff'', N''dung'', N''0123456789'', N''dung@gmail.com'', NULL)
INSERT [dbo].[Customers] ([Id], [UserId], [FullName], [Phone], [Email], [Address]) VALUES (3, N''b61644a6-6276-4e72-9c80-c6a6dcbe0618'', N''an'', N''01'', N''quocanlau@gmail.com'', NULL)
SET IDENTITY_INSERT [dbo].[Customers] OFF
SET IDENTITY_INSERT [dbo].[DepositRequests] ON 

INSERT [dbo].[DepositRequests] ([Id], [CustomerId], [CarId], [Note], [Status], [CreatedAt]) VALUES (1, 1, 16, NULL, 1, CAST(N''2026-10-09T14:06:43.7397417+00:00'' AS DateTimeOffset))
INSERT [dbo].[DepositRequests] ([Id], [CustomerId], [CarId], [Note], [Status], [CreatedAt]) VALUES (2, 1, 15, NULL, 1, CAST(N''2026-10-09T14:08:52.7828561+00:00'' AS DateTimeOffset))
INSERT [dbo].[DepositRequests] ([Id], [CustomerId], [CarId], [Note], [Status], [CreatedAt]) VALUES (3, 2, 14, NULL, 0, CAST(N''2026-10-10T03:51:46.5327517+00:00'' AS DateTimeOffset))
SET IDENTITY_INSERT [dbo].[DepositRequests] OFF
SET IDENTITY_INSERT [dbo].[Manufacturers] ON 

INSERT [dbo].[Manufacturers] ([Id], [Name], [Country], [IsActive]) VALUES (1, N''Toyota'', N''Nhật Bản'', 1)
INSERT [dbo].[Manufacturers] ([Id], [Name], [Country], [IsActive]) VALUES (2, N''Ford'', N''Hoa Kỳ'', 1)
INSERT [dbo].[Manufacturers] ([Id], [Name], [Country], [IsActive]) VALUES (3, N''Honda'', N''Nhật Bản'', 1)
INSERT [dbo].[Manufacturers] ([Id], [Name], [Country], [IsActive]) VALUES (4, N''Mitsubishi'', N''Nhật Bản'', 1)
SET IDENTITY_INSERT [dbo].[Manufacturers] OFF
SET IDENTITY_INSERT [dbo].[PurchaseDetails] ON 

INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (1, 1, 1, CAST(710000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (2, 1, 2, CAST(720000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (3, 1, 3, CAST(730000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (4, 1, 4, CAST(740000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (5, 1, 5, CAST(750000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (6, 1, 6, CAST(760000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (7, 1, 7, CAST(770000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (8, 1, 8, CAST(780000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (9, 1, 9, CAST(790000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (10, 1, 10, CAST(800000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (11, 1, 11, CAST(810000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (12, 1, 12, CAST(820000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (13, 1, 13, CAST(830000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (14, 1, 14, CAST(840000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (15, 1, 15, CAST(850000000.00 AS Decimal(18, 2)))
INSERT [dbo].[PurchaseDetails] ([Id], [PurchaseReceiptId], [CarId], [PurchasePrice]) VALUES (16, 1, 16, CAST(860000000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[PurchaseDetails] OFF
SET IDENTITY_INSERT [dbo].[PurchaseReceipts] ON 

INSERT [dbo].[PurchaseReceipts] ([Id], [SupplierId], [EmployeeId], [PurchasedAt], [Status]) VALUES (1, 1, N''075409ca-0540-4e01-b608-e3ef06f532e2'', CAST(N''2026-10-09T13:47:48.0194619+00:00'' AS DateTimeOffset), 1)
SET IDENTITY_INSERT [dbo].[PurchaseReceipts] OFF
SET IDENTITY_INSERT [dbo].[SaleTransactions] ON 

INSERT [dbo].[SaleTransactions] ([Id], [CustomerId], [CarId], [DepositRequestId], [EmployeeId], [AgreedPrice], [RequiredDeposit], [Status], [CreatedAt], [ConfirmedAt], [UpdatedAt]) VALUES (1, 1, 15, 2, N''075409ca-0540-4e01-b608-e3ef06f532e2'', CAST(950000000.00 AS Decimal(18, 2)), CAST(19000000.00 AS Decimal(18, 2)), 4, CAST(N''2026-10-09T14:09:38.1782360+00:00'' AS DateTimeOffset), CAST(N''2026-10-09T14:09:38.1785323+00:00'' AS DateTimeOffset), CAST(N''2026-10-09T14:59:52.7632802+00:00'' AS DateTimeOffset))
INSERT [dbo].[SaleTransactions] ([Id], [CustomerId], [CarId], [DepositRequestId], [EmployeeId], [AgreedPrice], [RequiredDeposit], [Status], [CreatedAt], [ConfirmedAt], [UpdatedAt]) VALUES (2, 1, 16, 1, N''075409ca-0540-4e01-b608-e3ef06f532e2'', CAST(960000000.00 AS Decimal(18, 2)), CAST(19200000.00 AS Decimal(18, 2)), 4, CAST(N''2026-10-09T14:09:39.1816036+00:00'' AS DateTimeOffset), CAST(N''2026-10-09T14:09:39.1816058+00:00'' AS DateTimeOffset), CAST(N''2026-10-09T14:59:51.3522662+00:00'' AS DateTimeOffset))
SET IDENTITY_INSERT [dbo].[SaleTransactions] OFF
SET IDENTITY_INSERT [dbo].[Suppliers] ON 

INSERT [dbo].[Suppliers] ([Id], [Name], [SupplierType], [Phone], [Address], [IsActive]) VALUES (1, N''Đối tác dữ liệu mẫu'', N''Tổ chức'', N''0900000000'', NULL, 1)
SET IDENTITY_INSERT [dbo].[Suppliers] OFF
SET ANSI_PADDING ON
/****** Object:  Index [IX_AdminNotificationReads_UserId_EventKey]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AdminNotificationReads_UserId_EventKey] ON [dbo].[AdminNotificationReads]
(
	[UserId] ASC,
	[EventKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [RoleNameIndex]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [EmailIndex]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [UserNameIndex]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_CarImages_CarId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarImages_CarId] ON [dbo].[CarImages]
(
	[CarId] ASC
)
WHERE ([IsPrimary]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_CarModels_ManufacturerId_Name]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CarModels_ManufacturerId_Name] ON [dbo].[CarModels]
(
	[ManufacturerId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_Cars_CarModelId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_Cars_CarModelId] ON [dbo].[Cars]
(
	[CarModelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_Cars_IsPublished_Status_CarModelId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_Cars_IsPublished_Status_CarModelId] ON [dbo].[Cars]
(
	[IsPublished] ASC,
	[Status] ASC,
	[CarModelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_Cars_VIN]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Cars_VIN] ON [dbo].[Cars]
(
	[VIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_Customers_UserId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Customers_UserId] ON [dbo].[Customers]
(
	[UserId] ASC
)
WHERE ([UserId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_Deliveries_SaleTransactionId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Deliveries_SaleTransactionId] ON [dbo].[Deliveries]
(
	[SaleTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_DepositRequests_CarId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_DepositRequests_CarId] ON [dbo].[DepositRequests]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_DepositRequests_CustomerId_CarId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_DepositRequests_CustomerId_CarId] ON [dbo].[DepositRequests]
(
	[CustomerId] ASC,
	[CarId] ASC
)
WHERE ([Status] IN ((0), (3)))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_Manufacturers_Name]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Manufacturers_Name] ON [dbo].[Manufacturers]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_PaymentEntries_CreatedById]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_PaymentEntries_CreatedById] ON [dbo].[PaymentEntries]
(
	[CreatedById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_PaymentEntries_SaleTransactionId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_PaymentEntries_SaleTransactionId] ON [dbo].[PaymentEntries]
(
	[SaleTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_PurchaseDetails_CarId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_PurchaseDetails_CarId] ON [dbo].[PurchaseDetails]
(
	[CarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_PurchaseDetails_PurchaseReceiptId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseDetails_PurchaseReceiptId] ON [dbo].[PurchaseDetails]
(
	[PurchaseReceiptId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_PurchaseReceipts_EmployeeId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_EmployeeId] ON [dbo].[PurchaseReceipts]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_PurchaseReceipts_SupplierId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_SupplierId] ON [dbo].[PurchaseReceipts]
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_SaleTransactions_CarId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SaleTransactions_CarId] ON [dbo].[SaleTransactions]
(
	[CarId] ASC
)
WHERE ([Status] IN ((1), (2), (3)))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_SaleTransactions_CustomerId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_SaleTransactions_CustomerId] ON [dbo].[SaleTransactions]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
/****** Object:  Index [IX_SaleTransactions_DepositRequestId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SaleTransactions_DepositRequestId] ON [dbo].[SaleTransactions]
(
	[DepositRequestId] ASC
)
WHERE ([DepositRequestId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
SET ANSI_PADDING ON
/****** Object:  Index [IX_SaleTransactions_EmployeeId]    Script Date: 20-Oct-26 11:48:39 AM ******/
CREATE NONCLUSTERED INDEX [IX_SaleTransactions_EmployeeId] ON [dbo].[SaleTransactions]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
ALTER TABLE [dbo].[AdminNotificationReads]  WITH CHECK ADD  CONSTRAINT [FK_AdminNotificationReads_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[AdminNotificationReads] CHECK CONSTRAINT [FK_AdminNotificationReads_AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
ALTER TABLE [dbo].[CarImages]  WITH CHECK ADD  CONSTRAINT [FK_CarImages_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([Id])
ALTER TABLE [dbo].[CarImages] CHECK CONSTRAINT [FK_CarImages_Cars_CarId]
ALTER TABLE [dbo].[CarModels]  WITH CHECK ADD  CONSTRAINT [FK_CarModels_Manufacturers_ManufacturerId] FOREIGN KEY([ManufacturerId])
REFERENCES [dbo].[Manufacturers] ([Id])
ALTER TABLE [dbo].[CarModels] CHECK CONSTRAINT [FK_CarModels_Manufacturers_ManufacturerId]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [FK_Cars_CarModels_CarModelId] FOREIGN KEY([CarModelId])
REFERENCES [dbo].[CarModels] ([Id])
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [FK_Cars_CarModels_CarModelId]
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customers_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customers_AspNetUsers_UserId]
ALTER TABLE [dbo].[Deliveries]  WITH CHECK ADD  CONSTRAINT [FK_Deliveries_SaleTransactions_SaleTransactionId] FOREIGN KEY([SaleTransactionId])
REFERENCES [dbo].[SaleTransactions] ([Id])
ALTER TABLE [dbo].[Deliveries] CHECK CONSTRAINT [FK_Deliveries_SaleTransactions_SaleTransactionId]
ALTER TABLE [dbo].[DepositRequests]  WITH CHECK ADD  CONSTRAINT [FK_DepositRequests_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([Id])
ALTER TABLE [dbo].[DepositRequests] CHECK CONSTRAINT [FK_DepositRequests_Cars_CarId]
ALTER TABLE [dbo].[DepositRequests]  WITH CHECK ADD  CONSTRAINT [FK_DepositRequests_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ALTER TABLE [dbo].[DepositRequests] CHECK CONSTRAINT [FK_DepositRequests_Customers_CustomerId]
ALTER TABLE [dbo].[PaymentEntries]  WITH CHECK ADD  CONSTRAINT [FK_PaymentEntries_AspNetUsers_CreatedById] FOREIGN KEY([CreatedById])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[PaymentEntries] CHECK CONSTRAINT [FK_PaymentEntries_AspNetUsers_CreatedById]
ALTER TABLE [dbo].[PaymentEntries]  WITH CHECK ADD  CONSTRAINT [FK_PaymentEntries_SaleTransactions_SaleTransactionId] FOREIGN KEY([SaleTransactionId])
REFERENCES [dbo].[SaleTransactions] ([Id])
ALTER TABLE [dbo].[PaymentEntries] CHECK CONSTRAINT [FK_PaymentEntries_SaleTransactions_SaleTransactionId]
ALTER TABLE [dbo].[PurchaseDetails]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetails_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([Id])
ALTER TABLE [dbo].[PurchaseDetails] CHECK CONSTRAINT [FK_PurchaseDetails_Cars_CarId]
ALTER TABLE [dbo].[PurchaseDetails]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetails_PurchaseReceipts_PurchaseReceiptId] FOREIGN KEY([PurchaseReceiptId])
REFERENCES [dbo].[PurchaseReceipts] ([Id])
ALTER TABLE [dbo].[PurchaseDetails] CHECK CONSTRAINT [FK_PurchaseDetails_PurchaseReceipts_PurchaseReceiptId]
ALTER TABLE [dbo].[PurchaseReceipts]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseReceipts_AspNetUsers_EmployeeId] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[PurchaseReceipts] CHECK CONSTRAINT [FK_PurchaseReceipts_AspNetUsers_EmployeeId]
ALTER TABLE [dbo].[PurchaseReceipts]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseReceipts_Suppliers_SupplierId] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Suppliers] ([Id])
ALTER TABLE [dbo].[PurchaseReceipts] CHECK CONSTRAINT [FK_PurchaseReceipts_Suppliers_SupplierId]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransactions_AspNetUsers_EmployeeId] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [FK_SaleTransactions_AspNetUsers_EmployeeId]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransactions_Cars_CarId] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([Id])
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [FK_SaleTransactions_Cars_CarId]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransactions_Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [FK_SaleTransactions_Customers_CustomerId]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransactions_DepositRequests_DepositRequestId] FOREIGN KEY([DepositRequestId])
REFERENCES [dbo].[DepositRequests] ([Id])
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [FK_SaleTransactions_DepositRequests_DepositRequestId]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Car_Condition] CHECK  (([Condition]=(1) OR [Condition]=(0)))
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Car_Condition]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Car_Mileage] CHECK  (([Mileage]>=(0)))
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Car_Mileage]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Car_Price] CHECK  (([ListedPrice]>(0)))
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Car_Price]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Car_Status] CHECK  (([Status]>=(0) AND [Status]<=(4)))
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Car_Status]
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Car_Year] CHECK  (([ManufactureYear]>=(1900) AND [ManufactureYear]<=(2100)))
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Car_Year]
ALTER TABLE [dbo].[DepositRequests]  WITH CHECK ADD  CONSTRAINT [CK_Request_Status] CHECK  (([Status]>=(0) AND [Status]<=(3)))
ALTER TABLE [dbo].[DepositRequests] CHECK CONSTRAINT [CK_Request_Status]
ALTER TABLE [dbo].[PaymentEntries]  WITH CHECK ADD  CONSTRAINT [CK_Payment_Amount] CHECK  (([Amount]>(0)))
ALTER TABLE [dbo].[PaymentEntries] CHECK CONSTRAINT [CK_Payment_Amount]
ALTER TABLE [dbo].[PaymentEntries]  WITH CHECK ADD  CONSTRAINT [CK_Payment_Type] CHECK  (([EntryType]=(1) OR [EntryType]=(0)))
ALTER TABLE [dbo].[PaymentEntries] CHECK CONSTRAINT [CK_Payment_Type]
ALTER TABLE [dbo].[PurchaseDetails]  WITH CHECK ADD  CONSTRAINT [CK_PurchaseDetail_Price] CHECK  (([PurchasePrice]>(0)))
ALTER TABLE [dbo].[PurchaseDetails] CHECK CONSTRAINT [CK_PurchaseDetail_Price]
ALTER TABLE [dbo].[PurchaseReceipts]  WITH CHECK ADD  CONSTRAINT [CK_Purchase_Status] CHECK  (([Status]=(1) OR [Status]=(0)))
ALTER TABLE [dbo].[PurchaseReceipts] CHECK CONSTRAINT [CK_Purchase_Status]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [CK_Sale_Deposit] CHECK  (([RequiredDeposit]=round([AgreedPrice]*(0.02),(0))))
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [CK_Sale_Deposit]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [CK_Sale_Price] CHECK  (([AgreedPrice]>(0)))
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [CK_Sale_Price]
ALTER TABLE [dbo].[SaleTransactions]  WITH CHECK ADD  CONSTRAINT [CK_Sale_Status] CHECK  (([Status]>=(0) AND [Status]<=(4)))
ALTER TABLE [dbo].[SaleTransactions] CHECK CONSTRAINT [CK_Sale_Status]


IF (SELECT COUNT_BIG(*) FROM [dbo].[__EFMigrationsHistory]) <> 3
    THROW 51010, N''So ban ghi khong khop: __EFMigrationsHistory'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AdminNotificationReads]) <> 1
    THROW 51010, N''So ban ghi khong khop: AdminNotificationReads'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetRoleClaims]) <> 0
    THROW 51010, N''So ban ghi khong khop: AspNetRoleClaims'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetRoles]) <> 3
    THROW 51010, N''So ban ghi khong khop: AspNetRoles'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetUserClaims]) <> 0
    THROW 51010, N''So ban ghi khong khop: AspNetUserClaims'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetUserLogins]) <> 0
    THROW 51010, N''So ban ghi khong khop: AspNetUserLogins'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetUserRoles]) <> 4
    THROW 51010, N''So ban ghi khong khop: AspNetUserRoles'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetUsers]) <> 4
    THROW 51010, N''So ban ghi khong khop: AspNetUsers'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[AspNetUserTokens]) <> 0
    THROW 51010, N''So ban ghi khong khop: AspNetUserTokens'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[CarImages]) <> 16
    THROW 51010, N''So ban ghi khong khop: CarImages'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[CarModels]) <> 15
    THROW 51010, N''So ban ghi khong khop: CarModels'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[Cars]) <> 16
    THROW 51010, N''So ban ghi khong khop: Cars'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[ContactInquiries]) <> 0
    THROW 51010, N''So ban ghi khong khop: ContactInquiries'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[Customers]) <> 3
    THROW 51010, N''So ban ghi khong khop: Customers'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[Deliveries]) <> 0
    THROW 51010, N''So ban ghi khong khop: Deliveries'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[DepositRequests]) <> 3
    THROW 51010, N''So ban ghi khong khop: DepositRequests'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[Manufacturers]) <> 4
    THROW 51010, N''So ban ghi khong khop: Manufacturers'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[PaymentEntries]) <> 0
    THROW 51010, N''So ban ghi khong khop: PaymentEntries'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[PurchaseDetails]) <> 16
    THROW 51010, N''So ban ghi khong khop: PurchaseDetails'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[PurchaseReceipts]) <> 1
    THROW 51010, N''So ban ghi khong khop: PurchaseReceipts'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[SaleTransactions]) <> 2
    THROW 51010, N''So ban ghi khong khop: SaleTransactions'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[ShowroomSettings]) <> 0
    THROW 51010, N''So ban ghi khong khop: ShowroomSettings'', 1;
IF (SELECT COUNT_BIG(*) FROM [dbo].[Suppliers]) <> 1
    THROW 51010, N''So ban ghi khong khop: Suppliers'', 1;
';
    SET @schemaAndData = N'USE ' + QUOTENAME(@database) + N';' + CHAR(13) + CHAR(10) + @schemaAndData;
    BEGIN TRANSACTION;
    EXEC sys.sp_executesql @schemaAndData;
    COMMIT TRANSACTION;
    EXEC sys.sp_releaseapplock @Resource=@lockName, @LockOwner='Session';
    PRINT N'AUTORA: Database ' + QUOTENAME(@database) + N' da tao day du schema va du lieu. Mo project va chay profile http. Khong can seed hoac Update-Database lan dau.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    EXEC sys.sp_releaseapplock @Resource=@lockName, @LockOwner='Session';
    THROW;
END CATCH;
