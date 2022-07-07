-- Deletes MultiPageFormData records older than 30 days. Should be run every night at 2AM.
DELETE
FROM MultiPageFormData
WHERE CreatedDate < DATEADD(day, -30, GETDATE())
