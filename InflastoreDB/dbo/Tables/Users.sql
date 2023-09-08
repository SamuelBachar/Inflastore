CREATE TABLE [dbo].[Users] (
    [Id]                            INT            IDENTITY (1, 1) NOT NULL,
    [Email]                         NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PasswordHashWithSalt]          NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [VerificationToken]             NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [VerifiedAt]                    DATETIME2 (7)  NULL,
    [PasswordResetToken]            NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ResetTokenExpires]             DATETIME2 (7)  NULL,
    [Region]                        INT            DEFAULT ((0)) NOT NULL,
    [TempResetPasswordHashWithSalt] NVARCHAR (MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

