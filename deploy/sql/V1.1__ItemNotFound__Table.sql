CREATE TABLE [ItemNotFound]
(
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    [ItemId] INT NOT NULL,
    CreateAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreateBy NVARCHAR(48) NOT NULL,
    DeleteAt DATETIME NULL,
    DeleteBy NVARCHAR(48) NULL,
    UpdateAt DATETIME NULL,
    UpdateBy NVARCHAR(48) NULL

);