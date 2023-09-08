CREATE TABLE [dbo].[ItemsPrices] (
    [Id]            INT  IDENTITY (1, 1) NOT NULL,
    [Company_Id]    INT  NOT NULL,
    [Item_Id]       INT  NOT NULL,
    [Price]         REAL NOT NULL,
    [PriceDiscount] REAL NOT NULL,
    CONSTRAINT [PK_ItemsPrices] PRIMARY KEY CLUSTERED ([Id] ASC)
);

