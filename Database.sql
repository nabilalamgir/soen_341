CREATE TABLE [dbo].[Users] (
    [ID]			INT	IDENTITY(1,1)	NOT NULL PRIMARY KEY, /* IDENTITY means it starts at 1 and increments by 1 */
	[First_Name]	VARCHAR (100)		NOT NULL,
	[Last_Name]		VARCHAR (100)		NOT NULL,
	[Username]      VARCHAR (100)		NOT NULL UNIQUE,
	[Password]		VARCHAR (100)		NOT NULL,
	[Email]			VARCHAR (100)		NOT NULL UNIQUE,
    [Age]			INT					NULL,
    [DOB]			DATE				NULL, /* Date of birth */
	[Date_Joined]	DATE DEFAULT GETDATE()
);

CREATE TABLE [dbo].[UserPosts](
	[ID]			INT	IDENTITY (1, 1)	NOT NULL PRIMARY KEY,
	[Title]			VARCHAR (100)		NOT NULL,
	[Description]	VARCHAR(100)		NOT NULL,
	[POST]			varbinary (max)		NOT NULL,
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
	delete from [dbo].[Users] where [dbo].[Users].[ID] = @ID
END;