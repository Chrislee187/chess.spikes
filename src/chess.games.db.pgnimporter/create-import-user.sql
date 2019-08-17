USE [master]
GO

/****** Object:  Login [game-importer]    Script Date: 12/08/2019 11:54:50 ******/
DROP LOGIN [game-importer]
GO

/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [game-importer]    Script Date: 12/08/2019 11:54:50 ******/
CREATE LOGIN [game-importer] WITH PASSWORD=N'Abcde123!', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [game-importer] ENABLE
GO

ALTER SERVER ROLE [dbcreator] ADD MEMBER [game-importer]
GO
