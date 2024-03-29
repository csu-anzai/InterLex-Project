USE [master]
GO
/****** Object:  Database [Interlex]    Script Date: 21.5.2019 г. 11:33:05 ******/
CREATE DATABASE [Interlex]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Interlex', FILENAME = N'D:\MSSQL\Data\Interlex.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Interlex_log', FILENAME = N'D:\MSSQL\Data\Interlex_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Interlex] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Interlex].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Interlex] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Interlex] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Interlex] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Interlex] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Interlex] SET ARITHABORT OFF 
GO
ALTER DATABASE [Interlex] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Interlex] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Interlex] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Interlex] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Interlex] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Interlex] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Interlex] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Interlex] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Interlex] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Interlex] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Interlex] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Interlex] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Interlex] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Interlex] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Interlex] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Interlex] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Interlex] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Interlex] SET RECOVERY FULL 
GO
ALTER DATABASE [Interlex] SET  MULTI_USER 
GO
ALTER DATABASE [Interlex] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Interlex] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Interlex] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Interlex] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Interlex] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Interlex', N'ON'
GO
USE [Interlex]
GO
/****** Object:  Schema [classifier]    Script Date: 21.5.2019 г. 11:33:05 ******/
CREATE SCHEMA [classifier]
GO
/****** Object:  Schema [suggest]    Script Date: 21.5.2019 г. 11:33:05 ******/
CREATE SCHEMA [suggest]
GO
/****** Object:  Table [classifier].[Codes]    Script Date: 21.5.2019 г. 11:33:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classifier].[Codes](
	[Id] [uniqueidentifier] NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[Level] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_classifier_Codes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [classifier].[Relationships]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classifier].[Relationships](
	[ParentId] [uniqueidentifier] NOT NULL,
	[ChildId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_classifier_Relationships] PRIMARY KEY CLUSTERED 
(
	[ParentId] ASC,
	[ChildId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [classifier].[Texts]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [classifier].[Texts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClassifierId] [uniqueidentifier] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[LanguageId] [int] NOT NULL,
 CONSTRAINT [PK_classifier_Texts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[OrganizationId] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cases]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cases](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [nvarchar](450) NOT NULL,
	[content] [nvarchar](max) NOT NULL,
	[caption] [nvarchar](4000) NOT NULL,
	[lastChange] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CasesLog]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CasesLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[caseId] [int] NOT NULL,
	[userId] [nvarchar](450) NOT NULL,
	[ChangeDate] [datetime2](7) NOT NULL,
	[content] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Languages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TwoLetter] [nvarchar](2) NOT NULL,
	[ThreeLetter] [nvarchar](3) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Metadata]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Metadata](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [nvarchar](450) NOT NULL,
	[content] [nvarchar](max) NOT NULL,
	[caption] [nvarchar](4000) NOT NULL,
	[lastChange] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetadataLog]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetadataLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[metadataId] [int] NOT NULL,
	[userId] [nvarchar](450) NOT NULL,
	[ChangeDate] [datetime2](7) NOT NULL,
	[content] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetaFile]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetaFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MetadataId] [int] NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[MimeType] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MetaTranslatedFile]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetaTranslatedFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MetadataId] [int] NOT NULL,
	[Content] [varbinary](max) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[MimeType] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organizations]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organizations](
	[Id] [nvarchar](450) NOT NULL,
	[ShortName] [nvarchar](256) NOT NULL,
	[FullName] [nvarchar](256) NULL,
 CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [suggest].[Court]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [suggest].[Court](
	[Name] [nvarchar](256) NOT NULL,
	[JurId] [int] NOT NULL,
 CONSTRAINT [PK_Court] PRIMARY KEY CLUSTERED 
(
	[Name] ASC,
	[JurId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [suggest].[CourtEng]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [suggest].[CourtEng](
	[Name] [nvarchar](256) NOT NULL,
	[JurId] [int] NOT NULL,
 CONSTRAINT [PK_CourtEng] PRIMARY KEY CLUSTERED 
(
	[Name] ASC,
	[JurId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [suggest].[Jurisdictions]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [suggest].[Jurisdictions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JurCode] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [suggest].[Keywords]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [suggest].[Keywords](
	[Name] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [suggest].[Source]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [suggest].[Source](
	[Name] [nvarchar](256) NOT NULL,
	[JurId] [int] NOT NULL,
 CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED 
(
	[Name] ASC,
	[JurId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_classifier_Codes_Code]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_classifier_Codes_Code] ON [classifier].[Codes]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_classifier_Codes_Level]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_classifier_Codes_Level] ON [classifier].[Codes]
(
	[Level] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_classifier_Relationships_ChildId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_classifier_Relationships_ChildId] ON [classifier].[Relationships]
(
	[ChildId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_classifier_Relationships_ParentId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_classifier_Relationships_ParentId] ON [classifier].[Relationships]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_userId_cases]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_userId_cases] ON [dbo].[Cases]
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_caseid_caseslog]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_caseid_caseslog] ON [dbo].[CasesLog]
(
	[caseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_userId_caseslog]    Script Date: 21.5.2019 г. 11:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_userId_caseslog] ON [dbo].[CasesLog]
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Cases] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[CasesLog] ADD  DEFAULT (getdate()) FOR [ChangeDate]
GO
ALTER TABLE [dbo].[Metadata] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MetadataLog] ADD  DEFAULT (getdate()) FOR [ChangeDate]
GO
ALTER TABLE [classifier].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_classifier_Relationships_ChildId] FOREIGN KEY([ChildId])
REFERENCES [classifier].[Codes] ([Id])
GO
ALTER TABLE [classifier].[Relationships] CHECK CONSTRAINT [FK_classifier_Relationships_ChildId]
GO
ALTER TABLE [classifier].[Relationships]  WITH CHECK ADD  CONSTRAINT [FK_classifier_Relationships_ParentId] FOREIGN KEY([ParentId])
REFERENCES [classifier].[Codes] ([Id])
GO
ALTER TABLE [classifier].[Relationships] CHECK CONSTRAINT [FK_classifier_Relationships_ParentId]
GO
ALTER TABLE [classifier].[Texts]  WITH CHECK ADD  CONSTRAINT [FK_classifier_Texts_Codes] FOREIGN KEY([ClassifierId])
REFERENCES [classifier].[Codes] ([Id])
GO
ALTER TABLE [classifier].[Texts] CHECK CONSTRAINT [FK_classifier_Texts_Codes]
GO
ALTER TABLE [classifier].[Texts]  WITH CHECK ADD  CONSTRAINT [FK_Texts_Languages] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
GO
ALTER TABLE [classifier].[Texts] CHECK CONSTRAINT [FK_Texts_Languages]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_Organizations_Users] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organizations] ([Id])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_Organizations_Users]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Cases]  WITH CHECK ADD  CONSTRAINT [FK__Cases__userId__4BAC3F29] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cases] CHECK CONSTRAINT [FK__Cases__userId__4BAC3F29]
GO
ALTER TABLE [dbo].[CasesLog]  WITH CHECK ADD  CONSTRAINT [FK__CasesLog__caseId__59063A47] FOREIGN KEY([caseId])
REFERENCES [dbo].[Cases] ([id])
GO
ALTER TABLE [dbo].[CasesLog] CHECK CONSTRAINT [FK__CasesLog__caseId__59063A47]
GO
ALTER TABLE [dbo].[CasesLog]  WITH CHECK ADD  CONSTRAINT [FK__CasesLog__userId__5812160E] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CasesLog] CHECK CONSTRAINT [FK__CasesLog__userId__5812160E]
GO
ALTER TABLE [dbo].[Metadata]  WITH CHECK ADD  CONSTRAINT [FK_Users_Metadata] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Metadata] CHECK CONSTRAINT [FK_Users_Metadata]
GO
ALTER TABLE [dbo].[MetadataLog]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_MetadataLog] FOREIGN KEY([metadataId])
REFERENCES [dbo].[Metadata] ([id])
GO
ALTER TABLE [dbo].[MetadataLog] CHECK CONSTRAINT [FK_Metadata_MetadataLog]
GO
ALTER TABLE [dbo].[MetadataLog]  WITH CHECK ADD  CONSTRAINT [FK_Users_MetadataLog] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MetadataLog] CHECK CONSTRAINT [FK_Users_MetadataLog]
GO
ALTER TABLE [dbo].[MetaFile]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_MetaFile] FOREIGN KEY([MetadataId])
REFERENCES [dbo].[Metadata] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MetaFile] CHECK CONSTRAINT [FK_Metadata_MetaFile]
GO
ALTER TABLE [dbo].[MetaTranslatedFile]  WITH CHECK ADD  CONSTRAINT [FK_Metadata_MetaTranslatedFile] FOREIGN KEY([MetadataId])
REFERENCES [dbo].[Metadata] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MetaTranslatedFile] CHECK CONSTRAINT [FK_Metadata_MetaTranslatedFile]
GO
ALTER TABLE [suggest].[Court]  WITH CHECK ADD  CONSTRAINT [FK_Court_Jurisdiction] FOREIGN KEY([JurId])
REFERENCES [suggest].[Jurisdictions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [suggest].[Court] CHECK CONSTRAINT [FK_Court_Jurisdiction]
GO
ALTER TABLE [suggest].[CourtEng]  WITH CHECK ADD  CONSTRAINT [FK_CourtEng_Jurisdiction] FOREIGN KEY([JurId])
REFERENCES [suggest].[Jurisdictions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [suggest].[CourtEng] CHECK CONSTRAINT [FK_CourtEng_Jurisdiction]
GO
ALTER TABLE [suggest].[Source]  WITH CHECK ADD  CONSTRAINT [FK_Source_Jurisdiction] FOREIGN KEY([JurId])
REFERENCES [suggest].[Jurisdictions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [suggest].[Source] CHECK CONSTRAINT [FK_Source_Jurisdiction]
GO
/****** Object:  StoredProcedure [suggest].[InsertSuggestions]    Script Date: 21.5.2019 г. 11:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  CREATE PROCEDURE [suggest].[InsertSuggestions]
    @Jurisdiction Nvarchar(20),
	@Court Nvarchar(256),
	@CourtEng Nvarchar(256),
	@Source Nvarchar(256)
AS
BEGIN
	DECLARE @JurId int
    SET NOCOUNT ON;
	SELECT @JurId = Id FROM suggest.Jurisdictions WHERE JurCode = @Jurisdiction
	BEGIN TRANSACTION
    IF @Court IS NOT NULL AND NOT EXISTS (SELECT * FROM suggest.Court WHERE Name = @Court)
	BEGIN
		INSERT INTO suggest.Court(Name, JurId) VALUES (@Court, @JurId)
	END
	IF @CourtEng IS NOT NULL AND NOT EXISTS (SELECT * FROM suggest.CourtEng WHERE Name = @CourtEng)
	BEGIN
		INSERT INTO suggest.CourtEng(Name, JurId) VALUES (@CourtEng, @JurId)
	END
	IF @Source IS NOT NULL AND NOT EXISTS (SELECT * FROM suggest.Source WHERE Name = @Source)
	BEGIN
		INSERT INTO suggest.Source(Name, JurId) VALUES (@Source, @JurId)
	END
	COMMIT;
END
GO
USE [master]
GO
ALTER DATABASE [Interlex] SET  READ_WRITE 
GO
