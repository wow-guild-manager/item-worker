CREATE TABLE [SpellNotFound]
(
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    [SpellId] INT NOT NULL,
    CreateAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    DeleteAt DATETIME NULL,
    UpdateAt DATETIME NULL
);