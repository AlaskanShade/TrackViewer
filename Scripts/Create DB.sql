USE [master]
GO

/****** Object:  Database [TrackStore]    Script Date: 06/28/2011 14:34:06 ******/
CREATE DATABASE [TrackStore] ON  PRIMARY 
( NAME = N'TrackStore', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQL10\MSSQL\DATA\TrackStore.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TrackStore_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQL10\MSSQL\DATA\TrackStore_log.ldf' , SIZE = 1536KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [TrackStore] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TrackStore].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [TrackStore] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [TrackStore] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [TrackStore] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [TrackStore] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [TrackStore] SET ARITHABORT OFF 
GO

ALTER DATABASE [TrackStore] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [TrackStore] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [TrackStore] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [TrackStore] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [TrackStore] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [TrackStore] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [TrackStore] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [TrackStore] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [TrackStore] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [TrackStore] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [TrackStore] SET  DISABLE_BROKER 
GO

ALTER DATABASE [TrackStore] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [TrackStore] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [TrackStore] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [TrackStore] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [TrackStore] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [TrackStore] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [TrackStore] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [TrackStore] SET  READ_WRITE 
GO

ALTER DATABASE [TrackStore] SET RECOVERY FULL 
GO

ALTER DATABASE [TrackStore] SET  MULTI_USER 
GO

ALTER DATABASE [TrackStore] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [TrackStore] SET DB_CHAINING OFF 
GO

