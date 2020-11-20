BEGIN TRANSACTION

ALTER TABLE Customisations 
ADD 
	Question1 NVARCHAR(100) NULL, 
	Question2 NVARCHAR(100) NULL, 
	Question3 NVARCHAR(100) NULL, 
	CreatedTime DATETIME NOT NULL CONSTRAINT DF_Customisations_CreatedDate DEFAULT GETUTCDATE()

ALTER TABLE Customisations
DROP
	CONSTRAINT DF_Customisations_DefaultSupervisorAdminID,
	CONSTRAINT DF_Customisations_DefaultSupervisionTool,
	COLUMN DefaultSupervisorAdminID,
	COLUMN DefaultAppointmentTypeID

COMMIT