CREATE TABLE [Item]
(
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    [ItemId] INT NOT NULL,
	[NameFrFr] NVARCHAR(MAX) NOT NULL,
	[NameEnUs] NVARCHAR(MAX) NOT NULL,
	[NameEnGb] NVARCHAR(MAX) NOT NULL,
	[Quality] NVARCHAR(MAX) NOT NULL,
	[ItemClass] NVARCHAR(MAX) NOT NULL,
	[ItemSubClass] NVARCHAR(MAX) NOT NULL,
	[InventoryType] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
	CreateAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreateBy NVARCHAR(48) NOT NULL,
    DeleteAt DATETIME NULL,
    DeleteBy NVARCHAR(48) NULL,
    UpdateAt DATETIME NULL,
    UpdateBy NVARCHAR(48) NULL
);