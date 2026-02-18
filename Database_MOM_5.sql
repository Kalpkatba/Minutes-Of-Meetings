--CREATE DATABASE MOM_DB;

USE MOM_DB;


--MOM_MeetingType
CREATE TABLE MOM_MeetingType (
    MeetingTypeID INT IDENTITY(1,1) PRIMARY KEY,
    MeetingTypeName NVARCHAR(200) NOT NULL,
    Remarks NVARCHAR(500),
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE()
);

INSERT INTO MOM_MeetingType (MeetingTypeName, Remarks)
VALUES
('Weekly Review', 'Regular weekly status review'),
('Project Kickoff', 'Initial project discussion'),
('Client Meeting', 'Meeting with external client'),
('HR Policy', 'HR policy discussion'),
('Emergency Meeting', 'Urgent matters discussion');

--MOM_Department
CREATE TABLE MOM_Department (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(200) NOT NULL,
    Remarks NVARCHAR(500),
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE()
);
INSERT INTO MOM_Department (DepartmentName, Remarks)
VALUES
('IT', 'Software development department'),
('HR', 'Human resources'),
('Finance', 'Accounts and billing'),
('Sales', 'Sales and marketing'),
('Operations', 'Daily operations handling');

--CREATE PROCEDURE PR_MOM_Department_Insert
--    @DepartmentName NVARCHAR(200),
--    @Remarks NVARCHAR(500) = NULL,
--    @DepartmentID INT OUTPUT
--AS
--BEGIN
--    SET NOCOUNT ON;

--    DECLARE @Inserted TABLE (DepartmentID INT);

--    INSERT INTO MOM_Department
--    (
--        DepartmentName,
--        Remarks
--    )
--    OUTPUT INSERTED.DepartmentID INTO @Inserted
--    VALUES
--    (
--        @DepartmentName,
--        @Remarks
--    );

--    SELECT @DepartmentID = DepartmentID FROM @Inserted;
--END
--GO
select * from MOM_Department


--MOM_MeetingVenue
CREATE TABLE MOM_MeetingVenue (
    MeetingVenueID INT IDENTITY(1,1) PRIMARY KEY,
    MeetingVenueName NVARCHAR(200) NOT NULL,
    Remarks NVARCHAR(500),
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE()
);

INSERT INTO MOM_MeetingVenue (MeetingVenueName, Remarks)
VALUES
('Conference Room A', 'Main building first floor'),
('Conference Room B', 'Second floor meeting room'),
('Board Room', 'Executive meetings'),
('Training Hall', 'Employee training sessions'),
('Online - Microsoft Teams', 'Virtual meeting');

--MOM_Staff
CREATE TABLE MOM_Staff (
    StaffID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentID INT NOT NULL,
    StaffName NVARCHAR(200) NOT NULL,
    Mobile NVARCHAR(20),
    Email NVARCHAR(200),
    Remarks NVARCHAR(500),
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (DepartmentID) REFERENCES MOM_Department(DepartmentID)
);
INSERT INTO MOM_Staff (DepartmentID, StaffName, Mobile, Email, Remarks)
VALUES
(1, 'Amit Patel', '9876543210', 'amit@company.com', 'Senior Developer'),
(2, 'Neha Shah', '9876501234', 'neha@company.com', 'HR Manager'),
(3, 'Raj Mehta', '9898989898', 'raj@company.com', 'Accountant'),
(4, 'Priya Desai', '9123456780', 'priya@company.com', 'Sales Executive'),
(5, 'Kunal Joshi', '9000011111', 'kunal@company.com', 'Operations Lead');


--MOM_Meetings
CREATE TABLE MOM_Meetings (
    MeetingID INT IDENTITY(1,1) PRIMARY KEY,
    MeetingDate DATETIME NOT NULL,
    MeetingTypeID INT NOT NULL,
    DepartmentID INT NOT NULL,
    MeetingVenueID INT NOT NULL,
    MeetingDescription NVARCHAR(MAX),
    DocumentPath NVARCHAR(500),
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancellationDateTime DATETIME,
    CancellationReason NVARCHAR(500),
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (MeetingTypeID) REFERENCES MOM_MeetingType(MeetingTypeID),
    FOREIGN KEY (DepartmentID) REFERENCES MOM_Department(DepartmentID),
    FOREIGN KEY (MeetingVenueID) REFERENCES MOM_MeetingVenue(MeetingVenueID)
);
INSERT INTO MOM_Meetings
(MeetingDate, MeetingTypeID, DepartmentID, MeetingVenueID, MeetingDescription)
VALUES
('2026-02-01 10:00', 1, 1, 1, 'Weekly sprint review'),
('2026-02-03 11:00', 2, 1, 2, 'New ERP implementation kickoff'),
('2026-02-05 15:00', 3, 4, 3, 'Client product demo'),
('2026-02-07 12:00', 4, 2, 4, 'New leave policy discussion'),
('2026-02-10 09:30', 5, 5, 5, 'Production issue discussion');


--MOM_MeetingMember
CREATE TABLE MOM_MeetingMember (
    MeetingMemberID INT IDENTITY(1,1) PRIMARY KEY,
    MeetingID INT NOT NULL,
    StaffID INT NOT NULL,
    IsPresent BIT NOT NULL DEFAULT 0,
    Remarks NVARCHAR(500),
	Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (MeetingID) REFERENCES MOM_Meetings(MeetingID),
    FOREIGN KEY (StaffID) REFERENCES MOM_Staff(StaffID)
);
INSERT INTO MOM_MeetingMember (MeetingID, StaffID, IsPresent, Remarks)
VALUES
(1, 1, 1, 'Presented sprint progress'),
(2, 1, 1, 'Technical explanation given'),
(3, 4, 1, 'Handled client queries'),
(4, 2, 1, 'Explained HR policy'),
(5, 5, 0, 'Joined late due to site visit');


--SP:

-----------------------------------------(1) MOM_MeetingType
-----------------------------------------SelectAll

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_SelectAll
AS
SELECT
    MeetingTypeID,
    MeetingTypeName,
    Remarks,
    Created,
    Modified
FROM MOM_MeetingType
ORDER BY MeetingTypeName;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_SelectByPK
@MeetingTypeID INT
AS
SELECT
    MeetingTypeID,
    MeetingTypeName,
    Remarks,
    Created,
    Modified
FROM MOM_MeetingType
WHERE MeetingTypeID = @MeetingTypeID;


---------------------------------------Insert

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_Insert
@MeetingTypeName NVARCHAR(200),
@Remarks NVARCHAR(500)
AS
BEGIN
    INSERT INTO MOM_MeetingType
    (
        MeetingTypeName,
        Remarks,
        Created,
        Modified
    )
    VALUES
    (
        @MeetingTypeName,
        @Remarks,
        GETDATE(),
        GETDATE()
    );
END

-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_UpdateByPK
@MeetingTypeID INT,
@MeetingTypeName NVARCHAR(200),
@Remarks NVARCHAR(500)
AS
UPDATE MOM_MeetingType
SET
    MeetingTypeName = @MeetingTypeName,
    Remarks = @Remarks,
    Modified = GETDATE()
WHERE MeetingTypeID = @MeetingTypeID;


-----------------------------------------DelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingType_DeleteByPK
@MeetingTypeID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
    DELETE FROM MOM_MeetingType
    WHERE MeetingTypeID = @MeetingTypeID;

    -- Aggregate function
    SELECT @TotalRecords = COUNT(*) 
    FROM MOM_MeetingType;
END


-----------------------------------------(2) MOM_Department
select * from MOM_Department
-----------------------------------------SelectAll

CREATE OR ALTER PROCEDURE PR_MOM_Department_SelectAll
AS
SELECT
    DepartmentID,
    DepartmentName,
    Remarks,
    Created,
    Modified
FROM MOM_Department
ORDER BY DepartmentID;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_Department_SelectByPK
@DepartmentID INT
AS
SELECT
    DepartmentID,
    DepartmentName,
    Remarks,
    Created,
    Modified
FROM MOM_Department
WHERE DepartmentID = @DepartmentID;


-----------------------------------------Insert

CREATE OR ALTER PROCEDURE PR_MOM_Department_Insert
@DepartmentName NVARCHAR(200),
@Remarks NVARCHAR(500),
@TotalRecords INT OUTPUT
AS
BEGIN
	INSERT INTO MOM_Department 
	(
		DepartmentName,
		Remarks,
		Created,
		Modified
	)
	VALUES 
	(
		@DepartmentName, 
		@Remarks,
		GETDATE(),
		GETDATE()
);
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END


-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_Department_UpdateByPK
@DepartmentID INT,
@DepartmentName NVARCHAR(200),
@Remarks NVARCHAR(500)
AS
UPDATE MOM_Department
SET
    DepartmentName = @DepartmentName,
    Remarks = @Remarks,
    Modified = GETDATE()
WHERE DepartmentID = @DepartmentID;


-----------------------------------------DeleteByPK

CREATE OR ALTER PROCEDURE PR_MOM_Department_DeleteByPK
@DepartmentID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
	DELETE FROM MOM_Department
	WHERE DepartmentID = @DepartmentID;
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END


-----------------------------------------(3) MOM_MeetingVenue
-----------------------------------------SelectAll
select * from MOM_MeetingVenue

--INSERT INTO MOM_MeetingVenue (MeetingVenueName, Remarks)
--VALUES 
--('Conference Room A', 'Main discussion room near reception'),
--('Board Room', 'Reserved for senior management meetings'),
--('Online - Microsoft Teams', 'Virtual meetings for remote participants'),
--('Training Hall', 'Used for workshops and large gatherings');

--DELETE FROM MOM_MeetingVenue
--WHERE MeetingVenueID IN (5);

CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_SelectAll
AS
SELECT
    MeetingVenueID,
    MeetingVenueName,
    Remarks,
    Created,
    Modified
FROM MOM_MeetingVenue
ORDER BY MeetingVenueName;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_SelectByPK
@MeetingVenueID INT
AS
SELECT
    MeetingVenueID,
    MeetingVenueName,
    Remarks,
    Created,
    Modified
FROM MOM_MeetingVenue
WHERE MeetingVenueID = @MeetingVenueID;


-----------------------------------------Insert

--CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_Insert
--@MeetingVenueName NVARCHAR(200),
--@Remarks NVARCHAR(500)
----@TotalRecords INT OUTPUT
--AS
--BEGIN
--	INSERT INTO MOM_MeetingVenue 
--	(
--		MeetingVenueName,
--		Remarks,
--		Created,
--		Modified
--	)
--	VALUES 
--	(
--		@MeetingVenueName,
--		@Remarks,
--		GETDATE(),
--		GETDATE()
--	);
--    --SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
--END
CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_Insert
    @MeetingVenueName NVARCHAR(200),
    @Remarks NVARCHAR(500),
    @NewID INT OUTPUT
AS
BEGIN
    INSERT INTO MOM_MeetingVenue(MeetingVenueName, Remarks)
    VALUES (@MeetingVenueName, @Remarks)

    SET @NewID = SCOPE_IDENTITY();
END


-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_UpdateByPK
@MeetingVenueID INT,
@MeetingVenueName NVARCHAR(200),
@Remarks NVARCHAR(500)
AS
UPDATE MOM_MeetingVenue
SET
    MeetingVenueName = @MeetingVenueName,
    Remarks = @Remarks,
    Modified = GETDATE()
WHERE MeetingVenueID = @MeetingVenueID;


-----------------------------------------DeleteByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingVenue_DeleteByPK
@MeetingVenueID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
	DELETE FROM MOM_MeetingVenue
	WHERE MeetingVenueID = @MeetingVenueID;
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END

-----------------------------------------(4) MOM_Staff
-----------------------------------------SelectAll
CREATE OR ALTER PROCEDURE PR_MOM_Staff_SelectAll
AS
SELECT
    S.StaffID,
    S.StaffName,	
    S.Mobile,
    S.Email,
    D.DepartmentName,
    S.Remarks,
    S.Created
FROM MOM_Staff S
INNER JOIN MOM_Department D
    ON D.DepartmentID = S.DepartmentID
ORDER BY D.DepartmentName, S.StaffName;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_Staff_SelectByPK
@StaffID INT
AS
SELECT
    S.StaffID,
    S.StaffName,
    S.Mobile,
    S.Email,
    S.DepartmentID,
    D.DepartmentName,
    S.Remarks,
    S.Created,
    S.Modified
FROM MOM_Staff S
INNER JOIN MOM_Department D
    ON D.DepartmentID = S.DepartmentID
WHERE S.StaffID = @StaffID;


-----------------------------------------Insert

CREATE OR ALTER PROCEDURE PR_MOM_Staff_Insert
@DepartmentID INT,
@StaffName NVARCHAR(200),
@Mobile NVARCHAR(20),
@Email NVARCHAR(200),
@Remarks NVARCHAR(500),
@TotalRecords INT OUTPUT
AS
BEGIN
	INSERT INTO MOM_Staff
	(
		DepartmentID,
		StaffName,
		Mobile,
		Email,
		Remarks,
		Created,
		Modified
	)
	VALUES
	(
		@DepartmentID,
		@StaffName,
		@Mobile,
		@Email,
		@Remarks,
		GETDATE(),
		GETDATE()
	);
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END


-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_Staff_UpdateByPK
@StaffID INT,
@DepartmentID INT,
@StaffName NVARCHAR(200),
@Mobile NVARCHAR(20),
@Email NVARCHAR(200),
@Remarks NVARCHAR(500)
AS
UPDATE MOM_Staff
SET
    DepartmentID = @DepartmentID,
    StaffName = @StaffName,
    Mobile = @Mobile,
    Email = @Email,
    Remarks = @Remarks,
    Modified = GETDATE()
WHERE StaffID = @StaffID;


-----------------------------------------DeleteByPK

CREATE OR ALTER PROCEDURE PR_MOM_Staff_DeleteByPK
@StaffID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
	DELETE FROM MOM_Staff
	WHERE StaffID = @StaffID;
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END

-----------------------------------------(5) MOM_Meetings
-----------------------------------------SelectAll

CREATE OR ALTER PROCEDURE PR_MOM_Meetings_SelectAll
AS
SELECT
    M.MeetingID,
    M.MeetingDate,
    MT.MeetingTypeName,
    D.DepartmentName,
    MV.MeetingVenueName,
    M.IsCancelled,
	M.CancellationDateTime,
	M.CancellationReason,
	M.MeetingDescription,
    M.Created
FROM MOM_Meetings M
INNER JOIN MOM_MeetingType MT ON MT.MeetingTypeID = M.MeetingTypeID
INNER JOIN MOM_Department D ON D.DepartmentID = M.DepartmentID
INNER JOIN MOM_MeetingVenue MV ON MV.MeetingVenueID = M.MeetingVenueID
ORDER BY M.MeetingDate DESC;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_Meetings_SelectByPK
@MeetingID INT
AS
SELECT
    M.MeetingID,
    M.MeetingDate,
    MT.MeetingTypeName,
    D.DepartmentName,
    MV.MeetingVenueName,
    M.MeetingDescription,
    M.DocumentPath,
    M.IsCancelled,
    M.CancellationDateTime,
    M.CancellationReason,
    M.Created,
    M.Modified
FROM MOM_Meetings M
JOIN MOM_MeetingType MT ON MT.MeetingTypeID = M.MeetingTypeID
JOIN MOM_Department D ON D.DepartmentID = M.DepartmentID
JOIN MOM_MeetingVenue MV ON MV.MeetingVenueID = M.MeetingVenueID
WHERE M.MeetingID = @MeetingID;


-----------------------------------------Insert

CREATE OR ALTER PROCEDURE PR_MOM_Meetings_Insert
@MeetingDate DATETIME,
@MeetingTypeID INT,
@DepartmentID INT,
@MeetingVenueID INT,
@MeetingDescription NVARCHAR(MAX),
@DocumentPath NVARCHAR(500),
@TotalRecords INT OUTPUT
AS
BEGIN
	INSERT INTO MOM_Meetings
	(
		MeetingDate,
		MeetingTypeID,
		DepartmentID,
		MeetingVenueID,
		MeetingDescription,
		DocumentPath,
		Created,
		Modified
	)
	VALUES
	(
		@MeetingDate,
		@MeetingTypeID,
		@DepartmentID,
		@MeetingVenueID,
		@MeetingDescription,
		@DocumentPath,
		GETDATE(),
		GETDATE()
	);
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END


-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_Meetings_UpdateByPK
@MeetingID INT,
@MeetingDate DATETIME,
@MeetingTypeID INT,
@DepartmentID INT,
@MeetingVenueID INT,
@MeetingDescription NVARCHAR(MAX),
@DocumentPath NVARCHAR(500)
AS
UPDATE MOM_Meetings
SET
    MeetingDate = @MeetingDate,
    MeetingTypeID = @MeetingTypeID,
    DepartmentID = @DepartmentID,
    MeetingVenueID = @MeetingVenueID,
    MeetingDescription = @MeetingDescription,
    DocumentPath = @DocumentPath,
    Modified = GETDATE()
WHERE MeetingID = @MeetingID;


-----------------------------------------DeleteByPK

CREATE OR ALTER PROCEDURE PR_MOM_Meetings_DeleteByPK
@MeetingID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
	DELETE FROM MOM_Meetings
	WHERE MeetingID = @MeetingID;
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END



-----------------------------------------(6) MOM_MeetingMember
-----------------------------------------SelectAll

CREATE OR ALTER PROCEDURE PR_MOM_MeetingMember_SelectAll			
AS
SELECT
    MM.MeetingMemberID,
	M.MeetingID,
    M.MeetingDate,
	S.StaffID,
    S.StaffName,
    D.DepartmentName,
    MM.IsPresent,
    MM.Remarks
FROM MOM_MeetingMember MM
INNER JOIN MOM_Staff S ON S.StaffID = MM.StaffID
INNER JOIN MOM_Department D ON D.DepartmentID = S.DepartmentID
INNER JOIN MOM_Meetings M ON M.MeetingID = MM.MeetingID
ORDER BY M.MeetingDate DESC, S.StaffName;


-----------------------------------------SelectByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingMember_SelectByPK
@MeetingMemberID INT
AS
SELECT
    MM.MeetingMemberID,
    MM.MeetingID,
    M.MeetingDate,
    MM.StaffID,
    S.StaffName,
    D.DepartmentName,
    MM.IsPresent,
    MM.Remarks
FROM MOM_MeetingMember MM
INNER JOIN MOM_Meetings M ON M.MeetingID = MM.MeetingID
INNER JOIN MOM_Staff S ON S.StaffID = MM.StaffID
INNER JOIN MOM_Department D ON D.DepartmentID = S.DepartmentID
WHERE MM.MeetingMemberID = @MeetingMemberID;


-----------------------------------------Insert

CREATE OR ALTER PROCEDURE PR_MOM_MeetingMember_Insert
@MeetingID INT,
@StaffID INT,
@IsPresent BIT,
@Remarks NVARCHAR(500),
@TotalRecords INT OUTPUT
AS
BEGIN
	INSERT INTO MOM_MeetingMember
	(
		MeetingID, 
		StaffID,
		IsPresent,
		Remarks,
		Created,
		Modified
	)
	VALUES
	(
		@MeetingID,
		@StaffID, 
		@IsPresent,
		@Remarks,
		GETDATE(),
		GETDATE()
		);
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END


-----------------------------------------UpdateByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingMember_UpdateByPK
@MeetingMemberID INT,		
@IsPresent BIT,
@Remarks NVARCHAR(500)
AS
UPDATE MOM_MeetingMember
SET
    IsPresent = @IsPresent,
    Remarks = @Remarks,
    Modified = GETDATE()
WHERE MeetingMemberID = @MeetingMemberID;


-----------------------------------------DeleteByPK

CREATE OR ALTER PROCEDURE PR_MOM_MeetingMember_DeleteByPK
@MeetingMemberID INT,
@TotalRecords INT OUTPUT
AS
BEGIN
	DELETE FROM MOM_MeetingMember
	WHERE MeetingMemberID = @MeetingMemberID;
    SELECT @TotalRecords = COUNT(*) FROM MOM_Department;
END