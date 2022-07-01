UPDATE SupervisorDelegates
	SET DelegateUserID = (SELECT UserID FROM DelegateAccounts da WHERE da.CandidateNumber =
							(SELECT CandidateNumber FROM Candidates c WHERE c.CandidateID = CandidateID))

UPDATE CandidateAssessments
	SET DelegateUserID = (SELECT UserID FROM DelegateAccounts da WHERE da.CandidateNumber =
							(SELECT CandidateNumber FROM Candidates c WHERE c.CandidateID = CandidateID)),
		CentreID = (SELECT CentreID FROM Candidates c WHERE c.CandidateID = CandidateID)

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
