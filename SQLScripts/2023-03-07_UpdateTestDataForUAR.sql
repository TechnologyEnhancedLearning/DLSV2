UPDATE GroupDelegates
SET DelegateID = 3
WHERE GroupDelegateID = 154
GO
UPDATE DelegateAccounts
SET OldPassword = 'ADyLcAmuAkPwMkZW+ivvk/GCq/0yn0m08eP2hIPPvjKJwmvj6pBfmDrTv16tMz8xww=='
WHERE CandidateNumber = 'ES2'
GO
UPDATE Users
SET PasswordHash = 'ADyLcAmuAkPwMkZW+ivvk/GCq/0yn0m08eP2hIPPvjKJwmvj6pBfmDrTv16tMz8xww==', DetailsLastChecked=GETDATE()
WHERE ID = 1
GO
