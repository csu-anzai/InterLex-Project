USE [master]
GO
/****** Object:  Database [CrawlerInterlex]    Script Date: 20.5.2019 г. 15:05:52 ******/
CREATE DATABASE [CrawlerInterlex]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CrawlerInterlex_dat', FILENAME = N'D:\MSSQL\DATA\CrawlerInterlex.mdf' , SIZE = 102400KB , MAXSIZE = UNLIMITED, FILEGROWTH = 5%)
 LOG ON 
( NAME = N'CrawlerInterlex_log', FILENAME = N'D:\MSSQL\DATA\CrawlerInterlex_log.ldf' , SIZE = 5120KB , MAXSIZE = 2048GB , FILEGROWTH = 5%)
GO
ALTER DATABASE [CrawlerInterlex] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CrawlerInterlex].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CrawlerInterlex] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET ARITHABORT OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CrawlerInterlex] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CrawlerInterlex] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CrawlerInterlex] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CrawlerInterlex] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET RECOVERY FULL 
GO
ALTER DATABASE [CrawlerInterlex] SET  MULTI_USER 
GO
ALTER DATABASE [CrawlerInterlex] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CrawlerInterlex] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CrawlerInterlex] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CrawlerInterlex] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [CrawlerInterlex]
GO
/****** Object:  Table [dbo].[Crawlers]    Script Date: 20.5.2019 г. 15:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Crawlers](
	[CrawlerId] [int] IDENTITY(1,1) NOT NULL,
	[CrawlerName] [nvarchar](max) NOT NULL,
 CONSTRAINT [pk_CrawlerId] PRIMARY KEY CLUSTERED 
(
	[CrawlerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentGroups]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentGroups](
	[DocumentGroupId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentGroupDate] [nvarchar](100) NOT NULL,
	[Lang] [nvarchar](50) NOT NULL,
	[GroupType] [int] NULL,
	[DocumentGroupFormat] [nvarchar](100) NOT NULL,
	[DocumentGroupName] [nvarchar](250) NOT NULL,
	[Identifier] [nvarchar](50) NOT NULL,
	[Operation] [int] NOT NULL,
	[DataContent] [varbinary](max) NOT NULL,
	[CrawlerId] [int] NOT NULL,
 CONSTRAINT [pk_DocumentGroupId] PRIMARY KEY CLUSTERED 
(
	[DocumentGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Documents]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[DocumentId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentFormat] [nvarchar](100) NOT NULL,
	[DocumentName] [nvarchar](250) NOT NULL,
	[Identifier] [nvarchar](50) NOT NULL,
	[Operation] [int] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[Md5] [nvarchar](50) NOT NULL,
	[DocumentOrder] [int] NOT NULL,
	[DocumentGroupId] [int] NOT NULL,
 CONSTRAINT [pk_DocumentId] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationStatus]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationStatus](
	[Identifier] [uniqueidentifier] NOT NULL,
	[CurrentStatus] [int] NOT NULL,
	[LastModificationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [pk_OperationStatus_Identifier] PRIMARY KEY CLUSTERED 
(
	[Identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationStatusLogs]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationStatusLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime2](7) NOT NULL,
	[OldStatus] [int] NULL,
	[NewStatus] [int] NULL,
	[OperationStatusIdentifier] [uniqueidentifier] NOT NULL,
	[Authenticator] [nvarchar](250) NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
 CONSTRAINT [pk_OperationStatusLogs_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[f_get_new_or_update_interlex_editor_tool_docs]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE function [dbo].[f_get_new_or_update_interlex_editor_tool_docs]()
returns table
as
return
(
	with document_group_selector
	as
	(
		select
			dg.Identifier,
			dg.DocumentGroupName,
			dg.DocumentGroupDate
		from DocumentGroups as dg
		join Crawlers as cr on dg.CrawlerId = cr.CrawlerId and cr.CrawlerName = 'Interlex.Crawler.Crawlers.Interlex_editor_tool.InterlexEditorTool'
	),

	cases_selector
	as
	(
		select
			concat(c.id, '_case') as [id],
			c.content as [content]
		from Interlex.dbo.Cases as c
		left join document_group_selector as dg on concat(c.id, '_case.zip') = dg.DocumentGroupName
		where c.isDeleted = 0
		and (dg.Identifier is null or c.lastChange > dg.DocumentGroupDate)
	),
	legi_selector
	as
	(
		select
			concat(c.id, '_legi') as [id],
			c.content as [content]
		from Interlex.dbo.Metadata as c
		left join document_group_selector as dg on concat(c.id, '_legi.zip') = dg.DocumentGroupName
		where c.isDeleted = 0
		and (dg.Identifier is null or c.lastChange > dg.DocumentGroupDate)
	)

	select
		*
	from cases_selector

	union

	select
		*
	from legi_selector
);
GO
ALTER TABLE [dbo].[DocumentGroups]  WITH CHECK ADD  CONSTRAINT [fk_DocumentGroups_Crawlers] FOREIGN KEY([CrawlerId])
REFERENCES [dbo].[Crawlers] ([CrawlerId])
GO
ALTER TABLE [dbo].[DocumentGroups] CHECK CONSTRAINT [fk_DocumentGroups_Crawlers]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [fk_Documents_DocumentGroups] FOREIGN KEY([DocumentGroupId])
REFERENCES [dbo].[DocumentGroups] ([DocumentGroupId])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [fk_Documents_DocumentGroups]
GO
ALTER TABLE [dbo].[OperationStatusLogs]  WITH CHECK ADD  CONSTRAINT [fk_OperationStatusLogs_OperationStatusIdentifier] FOREIGN KEY([OperationStatusIdentifier])
REFERENCES [dbo].[OperationStatus] ([Identifier])
GO
ALTER TABLE [dbo].[OperationStatusLogs] CHECK CONSTRAINT [fk_OperationStatusLogs_OperationStatusIdentifier]
GO
/****** Object:  StoredProcedure [dbo].[p_ChangeOperationStatus]    Script Date: 20.5.2019 г. 15:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[p_ChangeOperationStatus] @Identifier UNIQUEIDENTIFIER, @NewStatus INT, @authenticator NVARCHAR(250), @errorMessage NVARCHAR(MAX)
AS
BEGIN	

	DECLARE @OldStatus INT
	SET @OldStatus = 0

		IF EXISTS (SELECT Identifier FROM OperationStatus  WHERE Identifier = @Identifier)
		BEGIN
			SELECT @OldStatus = CurrentStatus FROM OperationStatus 
			WHERE Identifier = @Identifier

			UPDATE OperationStatus SET CurrentStatus = @NewStatus, LastModificationDate = GETDATE()
			WHERE Identifier = @Identifier
		END
		ELSE
		BEGIN
			INSERT INTO OperationStatus(Identifier, CurrentStatus, LastModificationDate) VALUES  (@Identifier, @NewStatus, GETDATE())
		END

		INSERT INTO OperationStatusLogs(LogDate, OldStatus, NewStatus, OperationStatusIdentifier, Authenticator, ErrorMessage) VALUES (GETDATE(), @OldStatus, @NewStatus, @Identifier, @authenticator, @errorMessage)
END
GO
USE [master]
GO
ALTER DATABASE [CrawlerInterlex] SET  READ_WRITE 
GO
