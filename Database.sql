CREATE TABLE [dbo].[Users] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [First_Name]  VARCHAR (100) NOT NULL,
    [Last_Name]   VARCHAR (100) NOT NULL,
    [Username]    VARCHAR (100) NOT NULL,
    [Password]    VARCHAR (100) NOT NULL,
    [Email]       VARCHAR (100) NOT NULL,
    [Age]         INT           DEFAULT ((0)) NULL,
    [DOB]         DATE          NULL,
    [Date_Joined] DATE          DEFAULT (getdate()) NULL,
    [Role]        VARCHAR (100) DEFAULT ('REGULAR') NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);
CREATE TABLE [dbo].[UserPosts](
	[ID]			INT	IDENTITY (1, 1)	NOT NULL PRIMARY KEY,
	[Title]			VARCHAR (100)		NOT NULL,
	[Description]	VARCHAR(100)		NOT NULL,
	[POST]			varbinary (max)		NOT NULL,
	[Likes]		  INT			  DEFAULT ((0)) NULL,
	[Dislikes]	  INT			  DEFAULT ((0)) NULL,
	[Rating]		INT DEFAULT 0,
	[Date_Posted]	DATETIME DEFAULT GETDATE(),
	[User_ID]		INT NOT NULL,
	
	/*
	This will mean that when a user is deleted, then any row that users
	id is in will also be deleted
	*/
	FOREIGN KEY (User_ID) REFERENCES [dbo].[Users] ON DELETE CASCADE 
);

CREATE TABLE [dbo].[Comments](
	[ID]			INT	IDENTITY(1,1)	NOT NULL PRIMARY KEY,
	[Comment]		VARCHAR(100) NOT NULL,
	[Date_Posted]	DATETIME DEFAULT GETDATE(),
	[User_ID]		INT NOT NULL,
    [Username]    VARCHAR (100) NOT NULL,
	[Post_ID]		INT NOT NULL,
	FOREIGN KEY (User_ID) REFERENCES [dbo].[Users], /* this will be deleted by the trigger */
	FOREIGN KEY (Post_ID) REFERENCES [dbo].[UserPosts] ON DELETE CASCADE
);


CREATE TABLE [dbo].[FollowList](
	[ID]			INT	IDENTITY(1,1)	NOT NULL PRIMARY KEY,
	[FolloweeID]	INT NOT NULL, /* The user being followed	*/
	[FollowerID]	INT NOT NULL, /* The user that is following */

	/*
	This will mean that when either the user following or the one being followed,
	no longer has an account, then they are removed from the list along with the
	users they are following.
	*/
	FOREIGN KEY (FolloweeID) REFERENCES [dbo].[Users],
	FOREIGN KEY (FollowerID) REFERENCES [dbo].[Users]
);

CREATE TABLE [dbo].[LikeDislikeList] (
    [ID]            INT IDENTITY (1, 1) NOT NULL,
    [LikeOrDislike] INT DEFAULT ((0)) NOT NULL,
    [UserId]        INT NOT NULL,
    [PostId]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([ID]),
    FOREIGN KEY ([PostId]) REFERENCES [dbo].[UserPosts] ([ID]) ON DELETE CASCADE
	/*
	0 - NOTHING
	1 - LIKE
	2 - DISLIKE
	*/
	
);

CREATE TABLE [dbo].[FavouritesList] (
    [ID]            INT IDENTITY (1, 1) NOT NULL,
    [UserId]        INT NOT NULL,
    [PostId]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([ID]),
    FOREIGN KEY ([PostId]) REFERENCES [dbo].[UserPosts] ([ID]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[LinkedAccounts] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [Account1ID] INT NOT NULL,
    [Account2ID] INT NOT NULL,
    FOREIGN KEY ([Account1ID]) REFERENCES [dbo].[Users] ([ID]),
    FOREIGN KEY ([Account2ID]) REFERENCES [dbo].[Users] ([ID])
);

GO
CREATE   TRIGGER [dbo].[delete_user_associated_content]
ON [dbo].[Users] INSTEAD OF DELETE
AS
BEGIN
Declare @ID int
select @ID = ID from deleted
	delete from [dbo].[Comments] where [dbo].[Comments].[User_ID] = @ID
	delete from [dbo].[UserPosts] where [dbo].[UserPosts].[User_ID] = @ID
	delete from [dbo].[FollowList] where [dbo].[FollowList].[FolloweeID] = @ID
	delete from [dbo].[FollowList] where [dbo].[FollowList].[FollowerID] = @ID
	delete from [dbo].[LikeDislikeList] where [dbo].[LikeDislikeList].UserId = @ID
	delete from [dbo].[FavouritesList] where [dbo].FavouritesList.UserId = @ID
	delete from [dbo].[LinkedAccounts] where [dbo].[LinkedAccounts].[Account1ID] = @ID or [dbo].[LinkedAccounts].[Account2ID] = @ID
	delete from [dbo].[Users] where [dbo].[Users].[ID] = @ID	
END;

CREATE TRIGGER [reduce_like_dislike_number]
	ON [dbo].[LikeDislikeList]
	FOR DELETE
AS

BEGIN
	Declare @ID int
	DECLARE @postID int;
	select @ID = ID from deleted
	select @postID = PostId from deleted

	if
	(select LikeOrDislike from LikeDislikeList where ID = @ID) > 1
		update UserPosts set Dislikes = (Dislikes - 1) where UserPosts.Id = @postID;
	else
		update UserPosts set Likes = (Likes - 1) where UserPosts.Id = @postID;

	delete from [dbo].[LikeDislikeList] where [dbo].[LikeDislikeList].[ID] = @ID
	end;