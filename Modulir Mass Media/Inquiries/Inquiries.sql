﻿Delete from Audio
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


