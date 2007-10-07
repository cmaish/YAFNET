-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider Tables
-- =============================================
-- To do list
-- ---------------------------------------------
-- Primary Keys for tables


IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yafprov_Members') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yafprov_Membership](
		[UserID] [uniqueidentifier] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[Username] [nvarchar](50) NOT NULL,
		[Password] [nvarchar](50) NULL,
		[PasswordSalt] [nvarchar](50) NULL,
		[PasswordFormat] [nvarchar](50) NULL,
		[Email] [nvarchar](50) NULL,
		[PasswordQuestion] [nvarchar](50) NULL,
		[PasswordAnswer] [nvarchar](50) NULL,
		[IsApproved] [bit] NULL,
		[IsLockedOut] [bit] NULL,
		[LastLoginDate] [datetime] NULL,
		[LastPasswordChangeDate] [datetime] NULL,
		[LastLockOutDate] [datetime] NULL,
		[FailedPasswordAttempts] [int] NULL,
		[FailedAnswerAttempts] [int] NULL,
		[FailedPasswordWindow] [datetime] NULL,
		[FailedAnswerWindow] [datetime] NULL
		)
go

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yafprov_Applications') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yafprov_Application](
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[ApplicationName] [nvarchar](50) NULL,
		[Description] [ntext] NULL
		)
go


