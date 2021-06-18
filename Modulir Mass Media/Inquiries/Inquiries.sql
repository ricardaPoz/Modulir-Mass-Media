Delete from Audio
Delete from Video
Delete from [Text]

Update [Text] set take = 0 where take = 1
Update Video set take = 0 where take = 1
Update Audio set take = 0 where take = 1

CREATE TABLE [dbo].[Text] (
    [id]          INT            IDENTITY (1, 1) NOT NULL,
    [title]             NVARCHAR (500) NOT NULL,
    [link]              NVARCHAR (500) NOT NULL,
    [date_publications] DATETIME           NOT NULL,
    [take]              BIT            NOT NULL,
    [category]          NVARCHAR(20)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

CREATE TABLE [dbo].[Audio] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [title]             NVARCHAR (500) NOT NULL,
    [link]              NVARCHAR (500) NOT NULL,
    [date_publications] DATETIME          NOT NULL,
    [take]              BIT            NOT NULL,
    [category]          NVARCHAR(20)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[Video] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [title]             NVARCHAR (500) NOT NULL,
    [link]              NVARCHAR (500) NOT NULL,
    [date_publications] DATETIME          NOT NULL,
    [take]              BIT            NOT NULL,
    [category]          NVARCHAR(20)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE RSS
(
    [id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [LinkRSS]  NVARCHAR (500) NOT NULL UNIQUE,
    [TypeRSS]  NVARCHAR (10) NOT NULL
);

CREATE TABLE SMI
(
    [NameSMI]  NVARCHAR (100) PRIMARY KEY NOT NULL,
);

create table Journalist
(
	PassportId nvarchar(100) not null primary key,
	NameJournalist nvarchar(100) not null,
    [Type] NVARCHAR (10) NOT NULL,
	[NameSMI] nvarchar(100) Foreign key ([NameSMI]) References SMI ([NameSMI]) NULL 
)

CREATE TABLE Client
(
    [id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Login]  NVARCHAR (100) NOT NULL,
    [Password]  NVARCHAR (100) NOT NULL,
    [Data] ntext 
);

select count([Login])
from Client
where [Login] = N'сас'

select count([Login])
from Client
where [Login] = N'Поздняков Семён'

select count(id)
from Client
where [Login] = N'1' and [Password] = N'1'

select [Password]
from Client
where [Login] = N'1' 

select PassportId, NameJournalist, [Type]
from Journalist



select *  from Journalist where [NameSMI] = N'Хуй'

Update Journalist set [NameSMI] = N'Ведомости' where [PassportId] = N'dsa'

update Journalist set [NameSMI] = NULL where [NameSMI] = N'Tass'

select * from Journalist where  [NameSMI]  = N'Tass'

select PassportId from Journalist where  [NameSMI] IS NULL

drop table SMI
drop table Journalist
drop table RSS

