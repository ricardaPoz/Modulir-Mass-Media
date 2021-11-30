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
    [category]          NVARCHAR(10)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


CREATE TABLE [dbo].[Video] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [title]             NVARCHAR (500) NOT NULL,
    [link]              NVARCHAR (500) NOT NULL,
    [date_publications] DATETIME          NOT NULL,
    [take]              BIT            NOT NULL,
    [category]          NVARCHAR(100)    NOT NULL,
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

create table MediaSubscription
(
    [id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [NameSMI] nvarchar(100) Foreign key ([NameSMI]) References SMI ([NameSMI]) NULL,
    [Login] nvarchar(100) Foreign key ([Login]) References Client([Login]) NULL 
);

CREATE TABLE Client
(
    [Login]  NVARCHAR (100) PRIMARY KEY NOT NULL,
    [Password]  NVARCHAR (100) NOT NULL
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

drop table MediaSubscription
drop table SMI
drop table Journalist
drop table RSS
drop table Text
drop table Video
drop table Client


select * from Journalist, SMI WHERE Journalist.[NameSMI] = SMI.[NameSMI]

select * from Text where Take = 1


update Journalist set [NameSMI] = NULL where [NameSMI] = N'Tass'
delete from SMI where [NameSMI] = N'Ведомости'


Insert Into Video(title, link, date_publications, take, category) values(N'Пожар на Лужнецкой набережной в Москве — LIVE', N'https://www.youtube.com/watch?v=rztKXwHFqS8', '2021-6-19 20:55:33', 0, N'Категория отсутствует')