--TD-788 Add ResetExpiryDateTime column in ResetPassword(UP)
ALTER TABLE ResetPassword
ADD ResetExpiryDateTime DateTime NULL;
