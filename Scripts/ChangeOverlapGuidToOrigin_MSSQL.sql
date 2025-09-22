DECLARE @extsystemid NVARCHAR(50) = N'1C_ACCOUNTING';

BEGIN TRY
	UPDATE eel
	SET eel.entitytype = LOWER(doc.discriminator)
	FROM dbo.Sungero_Commons_ExtEntityLinks eel
	JOIN dbo.Sungero_Content_EDoc doc
		ON eel.entityid = doc.id
	WHERE eel.extentitytype IN (N'РеализацияТоваровУслуг', N'СчетНаОплатуПокупателю')
	  AND eel.extsystemid = @extsystemid;
END TRY
BEGIN CATCH
	PRINT 'Ошибка при обновлении:' + CONVERT(VARCHAR, ERROR_NUMBER()) + ':' + ERROR_MESSAGE();
END CATCH;