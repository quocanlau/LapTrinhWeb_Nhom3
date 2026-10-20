-- Read-only checks after installing Autora_Full.sql.
-- Counts may differ legitimately after the website has been used.
USE [AutoraDb];
GO
SELECT @@SERVERNAME AS SQLServer, DB_NAME() AS DatabaseHienTai;
SELECT N'__EFMigrationsHistory' AS TenBang, CAST(3 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[__EFMigrationsHistory]
UNION ALL
SELECT N'AdminNotificationReads' AS TenBang, CAST(1 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AdminNotificationReads]
UNION ALL
SELECT N'AspNetRoleClaims' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetRoleClaims]
UNION ALL
SELECT N'AspNetRoles' AS TenBang, CAST(3 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetRoles]
UNION ALL
SELECT N'AspNetUserClaims' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetUserClaims]
UNION ALL
SELECT N'AspNetUserLogins' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetUserLogins]
UNION ALL
SELECT N'AspNetUserRoles' AS TenBang, CAST(4 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetUserRoles]
UNION ALL
SELECT N'AspNetUsers' AS TenBang, CAST(4 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetUsers]
UNION ALL
SELECT N'AspNetUserTokens' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[AspNetUserTokens]
UNION ALL
SELECT N'CarImages' AS TenBang, CAST(16 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[CarImages]
UNION ALL
SELECT N'CarModels' AS TenBang, CAST(15 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[CarModels]
UNION ALL
SELECT N'Cars' AS TenBang, CAST(16 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[Cars]
UNION ALL
SELECT N'ContactInquiries' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[ContactInquiries]
UNION ALL
SELECT N'Customers' AS TenBang, CAST(3 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[Customers]
UNION ALL
SELECT N'Deliveries' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[Deliveries]
UNION ALL
SELECT N'DepositRequests' AS TenBang, CAST(3 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[DepositRequests]
UNION ALL
SELECT N'Manufacturers' AS TenBang, CAST(4 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[Manufacturers]
UNION ALL
SELECT N'PaymentEntries' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[PaymentEntries]
UNION ALL
SELECT N'PurchaseDetails' AS TenBang, CAST(16 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[PurchaseDetails]
UNION ALL
SELECT N'PurchaseReceipts' AS TenBang, CAST(1 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[PurchaseReceipts]
UNION ALL
SELECT N'SaleTransactions' AS TenBang, CAST(2 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[SaleTransactions]
UNION ALL
SELECT N'ShowroomSettings' AS TenBang, CAST(0 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[ShowroomSettings]
UNION ALL
SELECT N'Suppliers' AS TenBang, CAST(1 AS bigint) AS SoDongBanXuat, COUNT_BIG(*) AS SoDongHienTai FROM [dbo].[Suppliers];
SELECT [MigrationId], [ProductVersion] FROM [dbo].[__EFMigrationsHistory] ORDER BY [MigrationId];
SELECT N'FOREIGN KEY' AS Loai, [name], [is_disabled], [is_not_trusted] FROM sys.foreign_keys
UNION ALL
SELECT N'CHECK', [name], [is_disabled], [is_not_trusted] FROM sys.check_constraints;
