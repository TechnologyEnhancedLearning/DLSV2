UPDATE [dbo].[AdminUsers]
   SET [Login] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Login],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','th'),'ee','e'),'oo','or'),'ll','sk')
      ,[Forename] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Forename],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','th'),'ee','e'),'oo','or'),'ll','sk')
      ,[Surname] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Surname],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','th'),'ee','e'),'oo','or'),'ll','sk')
      ,[Email] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Email],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','th'),'ee','e'),'oo','or'),'ll','si')
 WHERE AdminID > 1
GO
UPDATE [dbo].[Candidates]
   SET [FirstName] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([FirstName],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[LastName] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([LastName],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[Answer1] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer1],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[Answer2] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer2],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[Answer3] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer3],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[AliasID] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([AliasID],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[EmailAddress] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([EmailAddress],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','x'),'ee','iy'),'oo','or'),'ll','sk')
      ,[Answer4] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer4],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[Answer5] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer5],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
      ,[Answer6] = replace(replace(replace(replace(replace(replace(replace(replace(replace(replace([Answer6],'o','e'),'a','o'),'i','a'),'u','i'),'t','p'),'c','k'),'d','j'),'ee','e'),'oo','ir'),'ll','sk')
 WHERE CandidateID <> 254480
GO



