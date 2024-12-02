IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'dbo' AND name like 'AuthorizedAttributesByRole')  
   DROP TABLE [dbo].[AuthorizedAttributesByRole];  
GO
IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'dbo' AND name like 'AuthorizedAttributes')  
   DROP TABLE [dbo].[AuthorizedAttributes];  
GO
IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'dbo' AND name like 'User')  
   DROP TABLE [dbo].[User];  
GO
IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'dbo' AND name like 'Role')  
   DROP TABLE [dbo].[Role];  
GO

DECLARE @ROLE_COUNT int = 100;
DECLARE @USER_COUNT int = 10000;

create table [Role] (
	RoleId varchar(50),
	CONSTRAINT PK_Role PRIMARY KEY (RoleId)
);

create table [User] (
	UserId varchar(50),
	RoleId varchar(50),
	CONSTRAINT PK_User PRIMARY KEY (UserId),
	CONSTRAINT FK_User_RoleId FOREIGN KEY (RoleId) REFERENCES [Role](RoleId),
);

create table [AuthorizedAttributesByRole] (
	RoleId varchar(50) not null,
	Method varchar(10) not null,
	[Path] varchar(255) not null,
	AttributeList json,
	CONSTRAINT FK_AuthorizedAttributesByRole_RoleId FOREIGN KEY (RoleId) REFERENCES [Role](RoleId),
	CONSTRAINT PK_AuthorizedAttributesByRole PRIMARY KEY (RoleId, Method, [Path])
);

create table [AuthorizedAttributes] (
	ClientId varchar(50) not null,
	Method varchar(10) not null,
	[Path] varchar(255) not null,
	AttributeList json,
	CONSTRAINT FK_AuthorizedAttributes_ClientId FOREIGN KEY (ClientId) REFERENCES [User](UserId),
	CONSTRAINT PK_AuthorizedAttributes PRIMARY KEY (ClientId, Method, [Path])
);

WITH Numbers AS (
    SELECT 1 AS Num
    UNION ALL
    SELECT Num + 1
    FROM Numbers
    WHERE Num < @ROLE_COUNT
)
INSERT INTO [Role]
SELECT RIGHT('000000' + CAST(Num AS VARCHAR), 6) AS ID
FROM Numbers
OPTION (MAXRECURSION 0);

WITH Numbers AS (
    SELECT 1 AS Num
    UNION ALL
    SELECT Num + 1
    FROM Numbers
    WHERE Num < @USER_COUNT
)
INSERT INTO [User] (UserID, RoleID)
SELECT 
    RIGHT('000000' + CAST(Num AS VARCHAR), 6) AS UserID,
    RIGHT('000000' + CAST((Num - 1) % 100 + 1 AS VARCHAR), 6) AS RoleID
FROM Numbers
OPTION (MAXRECURSION 0);