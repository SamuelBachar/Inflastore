CREATE TABLE [dbo].[Regions] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Company_Id] INT            NOT NULL,
    CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

