CREATE TABLE [dbo].[Items] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Unit_Id] INT            NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([Id] ASC)
);

