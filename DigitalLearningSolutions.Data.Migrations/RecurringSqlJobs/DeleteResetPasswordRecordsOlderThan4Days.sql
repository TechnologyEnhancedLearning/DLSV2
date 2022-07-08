-- Deletes ResetPassword records older than 4 days. Should be run every night at 3AM.
BEGIN
UPDATE au
SET au.ResetPasswordID = NULL FROM AdminUsers AS au
INNER JOIN ResetPassword AS r
ON r.ID = au.ResetPasswordID
WHERE r.PasswordResetDateTime < DATEADD(day, -4, GETDATE())

UPDATE c
SET c.ResetPasswordID = NULL FROM Candidates AS c
INNER JOIN ResetPassword AS r
ON r.ID = c.ResetPasswordID
WHERE r.PasswordResetDateTime < DATEADD(day, -4, GETDATE())

DELETE
FROM ResetPassword
WHERE PasswordResetDateTime < DATEADD(day, -4, GETDATE())
END
