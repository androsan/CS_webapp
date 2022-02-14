UPDATE
    table2
SET

    table2.Name = table1.Name
   --table2.Id = table1.Id
    
FROM
    [CS_webapp].[dbo].[Category] as table1
    INNER JOIN [CS_webapp].[dbo].[Podatki] as table2
        --ON table2.Id = null
		ON table2.Id = table1.Id
WHERE table2.Name IS NULL

--WHERE
    --table2.col3 = 'something'