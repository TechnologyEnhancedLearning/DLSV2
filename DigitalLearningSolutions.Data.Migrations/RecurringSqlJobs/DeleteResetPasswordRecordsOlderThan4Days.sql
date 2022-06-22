-- Deletes ResetPassword records older than 4 days. Should be run every night at 3AM.
BEGIN
UPDATE c
SET c.ResetPasswordID = NULL FROM Candidates AS c
INNER JOIN ResetPassword AS r
ON r.ID = c.ResetPasswordID
WHERE CAST (r.PasswordResetDateTime AS DATE) <= GETDATE() -4

DELETE
FROM ResetPassword
WHERE CAST(PasswordResetDateTime AS DATE) <= GETDATE() - 4
END
