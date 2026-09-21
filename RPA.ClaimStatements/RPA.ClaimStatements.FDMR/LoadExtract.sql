-- =============================================
-- Author:		Paul Doyle
-- Create date: 04/08/2016
-- Description:	Populate FDMR Extract
-- =============================================
CREATE PROCEDURE FDMR.LoadExtract 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO FDMR.[Extract]
		   ([scheme_year]
           ,[business_name]
           ,[sbi]
           ,[frn]
           ,[cross_compliance_percent]
           ,[country]
           ,[claim_number]
           ,[last_invoice_number]
           ,[last_invoice_currency])
SELECT	 [SchemeYear]	
		,[BusinessName]	
		,[SBI]	
		,[FRN]	
		,[ClaimNet4]
		,[CrossCompliancePercent]	
		,[Country]
		,[ClaimNum]	
		,[InvoiceNumber]
		,[TransactionCurrency]
FROM	
	(SELECT	 S.[ClaimID]
			,[SchemeYear]	
			,[BusinessName]	
			,[SBI]	
			,[FRN]		
			,[CrossCompliancePercent]	
			,Country='England'	
			,ClaimNum = CASE LEN([ApplicationID])	
				WHEN 7 THEN 'C'+[ApplicationID]
				WHEN 6 THEN 'C0'+[ApplicationID]
				WHEN 5 THEN 'C00'+[ApplicationID]
				WHEN 4 THEN 'C000'+[ApplicationID]
				WHEN 3 THEN 'C0000'+[ApplicationID]
				WHEN 2 THEN 'C00000'+[ApplicationID]
				ELSE 'C00000'+[ApplicationID]
			 END	
			,[InvoiceNumber]
	FROM [RPA.ClaimStatements].[SITI].[SUM]		S
	Join [RPA.ClaimStatements].[SITI].[SUM2]	S2	
		ON S.[ClaimID]=S2.[ClaimID]	
	WHERE S.[InvoiceNumber] IN (SELECT [InvoiceNumber] = MAX([InvoiceNumber])
	FROM [RPA.ClaimStatements].[SITI].[SUM]
	GROUP BY [FRN])) Sums
JOIN
	(SELECT 
		 BPSN4.[ClaimID]
		,ClaimNet4 = CAST((ROUND(BPS_Net4+GR_Net4+YF_Net4,2)) AS DECIMAL(18,2))
	FROM
	(SELECT	 BPS.[ClaimID]
			,BPS_Net4 = CASE WHEN (BPSGross-BPSReductions)<0 THEN 0
							ELSE BPSGross-BPSReductions
						END
	FROM
		(SELECT  [ClaimID]
				,BPSGross = CAST((ROUND([NonSDATotal]+[SDATotal]+[MoorlandSDATotal],2)) AS DECIMAL(18,2))
		FROM [RPA.ClaimStatements].[SITI].[BPS]) BPS
		JOIN
		(SELECT  [ClaimID]
				,BPSReductions = CAST((ROUND([OverDeclarationReduction]+[LateClaimSubmissionReduction]+[LateEntitlementsApplicationReduction]+[LateAmendmentReduction]+[LateEntitlementsAmendmentReduction],2)) AS DECIMAL(18,2))
		FROM [RPA.ClaimStatements].[SITI].[BPSPEN]) BPSP
		ON BPS.[ClaimID]=BPSP.[ClaimID]) BPSN4
	JOIN
		(SELECT  GR.[ClaimID]
				,GR_Net4 = CASE WHEN (GreeningGross - GreeningReductions)<0 THEN 0
								ELSE GreeningGross - GreeningReductions
							END
		FROM
			(SELECT  [ClaimID]
					,GreeningGross = CAST((ROUND([NonSDATotal]+[SDATotal]+[MoorlandTotal],2)) AS DECIMAL(18,2))
			FROM [RPA.ClaimStatements].[SITI].[GR]) GR
			JOIN
			(SELECT  [ClaimID]
					,GreeningReductions = CAST((ROUND([CropDiversificationReduction]+[PermanentGrasslandReduction]+[EFAReduction]+[LateApplicationReduction]+[LateAmendmentReduction],2)) AS DECIMAL(18,2))
			FROM [RPA.ClaimStatements].[SITI].[GRPEN]) GRP
			ON GR.[ClaimID]=GRP.[ClaimID]) GRN4
	ON BPSN4.[ClaimID]=GRN4.[ClaimID]
	JOIN
		(SELECT  [ClaimID]
				,YF_Net4 = CAST(([TotalYF]+[FDMReduction]) AS DECIMAL(18,2))
			FROM [RPA.ClaimStatements].[SITI].[YF]) YF
	ON BPSN4.[ClaimID]=YF.[ClaimID]) Net4Val
ON Sums.[ClaimID]=Net4Val.[ClaimID]
JOIN
	(SELECT 
		 LEFT([RPA.ClaimStatements].[DAX].[AP].[Invoice],1) + 'ITI' + SUBSTRING([RPA.ClaimStatements].[DAX].[AP].[Invoice],2,7) AS [InvoiceNum]
		,[TransactionCurrency]
	FROM [RPA.ClaimStatements].[DAX].[AP]) Curr
ON Sums.[InvoiceNumber]=Curr.[InvoiceNum]
EXCEPT
	 SELECT  [scheme_year]
			,[business_name]
			,[sbi]
			,[frn]
			,[value_after_net4_reductions]
			,[cross_compliance_percent]
			,[country]
			,[claim_number]
			,[last_invoice_number]
			,[last_invoice_currency]
		FROM [RPA.ClaimStatements].[FDMR].[Extract]


IF (SELECT COUNT(*) FROM [RPA.ClaimStatements].[FDMR].[Extract] WHERE sent_to_fdmr IS NULL) <> 0
	BEGIN

		DECLARE @SQL VARCHAR(MAX) = 'SELECT * FROM [RPA.ClaimStatements].[FDMR].[Extract]';

		DECLARE @FileName VARCHAR(MAX) = 'I:\RPA.Finance.FDMR\DEV\Import\ClaimStatement_Extract_' + REPLACE(REPLACE(REPLACE(CONVERT(varchar(25),SYSDATETIME(),120),'-',''),' ',''),':','') + '.csv';

		DECLARE @CMD VARCHAR(2000) = 'bcp "' + @SQL + '" queryout "' + @FileName + '"  -T -c -t^| -S D1VMGEN001'

		EXEC xp_cmdshell @CMD
		

		UPDATE [RPA.ClaimStatements].[FDMR].[Extract] SET sent_to_fdmr = SYSDATETIME() WHERE sent_to_fdmr IS NULL
	END
	
END
