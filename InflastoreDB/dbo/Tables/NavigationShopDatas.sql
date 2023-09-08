CREATE TABLE [dbo].[NavigationShopDatas] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Company_Id]  INT            NOT NULL,
    [FullAddress] NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Latitude]    REAL           NOT NULL,
    [Longtitude]  REAL           NOT NULL,
    CONSTRAINT [PK_NavigationShopDatas] PRIMARY KEY CLUSTERED ([Id] ASC)
);

