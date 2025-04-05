USE [InventoryAPI]
GO

INSERT INTO [dbo].[Product] 
([Name], [Description], [SKU], [Price], [CreatedAt], [CreatedBy], [LastUpdatedAt], [LastUpdatedBy])
VALUES 
('Product A', 'Description of Product A', 'SKU12345', 19.99, GETDATE(), 'Admin', GETDATE(), 'Admin'),
('Product B', 'Description of Product B', 'SKU12346', 29.99, GETDATE(), 'Admin', GETDATE(), 'Admin'),
('Product C', 'Description of Product C', 'SKU12347', 39.99, GETDATE(), 'Admin', GETDATE(), 'Admin'),
('Product D', 'Description of Product D', 'SKU12348', 49.99, GETDATE(), 'Admin', GETDATE(), 'Admin');


