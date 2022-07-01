UPDATE SupervisorDelegates
	SET SupervisorDelegates.DelegateUserID = da.UserID
	FROM Candidates c
		JOIN DelegateAccounts da
		ON c.CandidateNumber = da.CandidateNumber
	WHERE c.CandidateID = SupervisorDelegates.CandidateID

UPDATE CandidateAssessments
	SET CandidateAssessments.DelegateUserID = da.UserID,
		CandidateAssessments.CentreID = c.CentreID
	FROM Candidates c
		JOIN DelegateAccounts da
		ON c.CandidateNumber = da.CandidateNumber
	WHERE c.CandidateID = CandidateAssessments.CandidateID
