-- Deletes EmailVerificationHashes records older than 4 days. Should be run every night at 2:02AM.
BEGIN
DELETE FROM EmailVerificationHashes
WHERE CreatedDate < DATEADD(DAY, -4, GETUTCDATE())
END
