INSERT INTO [CS_webapp].[dbo].[Podatki] (Id) 
SELECT Id FROM [CS_webapp].[dbo].[Category]
WHERE Id NOT IN
    (SELECT Id
     FROM [CS_webapp].[dbo].[Podatki])
--WHERE (Id != 31 AND ID!=39)
--FROM [CS_webapp].[dbo].[Category] 
