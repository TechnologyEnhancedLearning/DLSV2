-- Deletes EmailVerificationHash records older than 4 days. Should be run every night at 2:02AM.

DELETE
FROM EmailVerificationHashes
WHERE CreatedDate < DATEADD(day, -4, GETUTCDATE());

