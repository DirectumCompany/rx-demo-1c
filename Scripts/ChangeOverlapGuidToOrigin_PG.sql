DO $$
DECLARE
	v_extsystemid text := '1C_ACCOUNTING';
BEGIN
	BEGIN
		UPDATE public.sungero_commons_extentitylinks eel
			SET entitytype = doc.discriminator
			FROM public.sungero_content_edoc doc
			WHERE 
				eel.extentitytype IN ('РеализацияТоваровУслуг','СчетНаОплатуПокупателю')
				AND eel.entityid = doc.id
				AND eel.extsystemid = v_extsystemid;
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'Ошибка при обновлении УПД / Счетов на оплату: %', SQLERRM;
	END;

END$$;