--TD-1283 replace the HEE information and address
BEGIN
DECLARE @ConfigText AS VARCHAR(MAX)
SET @ConfigText = '<p>If you represent a centre using or interested in using Digital Learning Solutions, please contact us at <a href="mailto:support@dls.nhs.uk">support@dls.nhs.uk</a></p><p>Digital Learning Solutions is provided by NHS England Technology Enhanced Learning:</p><p>NHS England<br />PO Box 16738<br />Redditch<br />B97 9PT</p>'
UPDATE Config SET ConfigText = @ConfigText WHERE ConfigName='ContactUsHtml'
END
