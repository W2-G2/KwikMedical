-- Sample data for randomization
DECLARE @FirstNames TABLE (Name NVARCHAR(50));
DECLARE @LastNames TABLE (Name NVARCHAR(50));
DECLARE @Addresses TABLE (Address NVARCHAR(255), City NVARCHAR(50), Postcode NVARCHAR(10));
DECLARE @LaboratoryReports TABLE (Report NVARCHAR(255));
DECLARE @TelephoneCalls TABLE (Call NVARCHAR(255));
DECLARE @Xrays TABLE (Xray NVARCHAR(255));
DECLARE @Letters TABLE (Letter NVARCHAR(255));
DECLARE @PrescriptionCharts TABLE (Prescription NVARCHAR(255));
DECLARE @ClinicalNotes TABLE (Note NVARCHAR(255));

-- Populate sample data
INSERT INTO @FirstNames VALUES 
('John'), ('Jane'), ('Robert'), ('Emily'), ('Michael'), ('Sarah'), ('William'), ('Jessica'), ('Ewan'), ('Fiona'),
('Liam'), ('Anna'), ('Bruce'), ('Heather'), ('Stuart'), ('Isobel'), ('Duncan'), ('Elaine'), ('Ross'), ('Mairi'),
('Alasdair'), ('Morag'), ('Iain'), ('Kirsty'), ('Hamish'), ('Catriona'), ('Graeme'), ('Aileen'), ('Donald'), ('Sheena'),
('Rory'), ('Moira'), ('Douglas'), ('Fiona'), ('Kenneth'), ('Lorna'), ('Malcolm'), ('Rhona'), ('Neil'), ('Lesley'),
('Andrew'), ('Christine'), ('Gordon'), ('Margaret'), ('Colin'), ('Shona'), ('David'), ('Helen'), ('Alan'), ('Karen');

INSERT INTO @LastNames VALUES 
('Smith'), ('MacDonald'), ('Campbell'), ('Stewart'), ('Murray'), ('Fraser'), ('Ross'), ('MacKenzie'), ('Gordon'), ('Graham'),
('Robertson'), ('MacIntyre'), ('Wallace'), ('Ferguson'), ('MacLean'), ('Watson'), ('Paterson'), ('MacLeod'), ('Reid'), ('Cameron'),
('Young'), ('Grant'), ('Ritchie'), ('MacKay'), ('Hunter'), ('MacGregor'), ('Thomson'), ('Wilson'), ('MacPherson'), ('Clark'),
('Mitchell'), ('Morrison'), ('Wright'), ('Johnston'), ('Taylor'), ('MacRae'), ('Scott'), ('Sinclair'), ('White'), ('Russell'),
('Walker'), ('Brown'), ('Duncan'), ('Miller'), ('Crawford'), ('Henderson'), ('Martin'), ('Bell'), ('Kelly'), ('Murphy');

INSERT INTO @Addresses VALUES 
('10 High Street', 'Edinburgh', 'EH1 1TE'),
('15 Castle Road', 'Inverness', 'IV2 3EA'),
('20 Main Street', 'Aberdeen', 'AB10 1AB'),
('25 King Street', 'Glasgow', 'G1 2AA'),
('30 Queen Street', 'Dundee', 'DD1 2HQ'),
('35 George Street', 'Perth', 'PH1 5LB'),
('40 Union Street', 'Stirling', 'FK8 1AU'),
('45 Market Street', 'Paisley', 'PA1 1DN'),
('50 Church Street', 'Dunfermline', 'KY12 7AF'),
('55 Station Road', 'Kirkcaldy', 'KY1 1LR'),
('60 Elm Row', 'Edinburgh', 'EH7 4AQ'),
('65 High Street', 'Elgin', 'IV30 1EE'),
('70 Broad Street', 'Fraserburgh', 'AB43 9AH'),
('75 South Street', 'St Andrews', 'KY16 9QW'),
('80 Main Street', 'Ayr', 'KA8 8EF'),
('85 King Street', 'Kilmarnock', 'KA1 1PT'),
('90 High Street', 'Galashiels', 'TD1 1SQ'),
('95 Bridge Street', 'Wick', 'KW1 4HG'),
('100 Main Street', 'Oban', 'PA34 4NT'),
('105 High Street', 'Fort William', 'PH33 6DG'),
('110 Main Street', 'Dumfries', 'DG1 2SQ'),
('115 High Street', 'Hawick', 'TD9 9BW'),
('120 Castle Street', 'Dornoch', 'IV25 3SN'),
('125 High Street', 'Peebles', 'EH45 8AN'),
('130 Main Street', 'Troon', 'KA10 6AA'),
('135 High Street', 'Cumnock', 'KA18 1BZ'),
('140 Main Street', 'Stranraer', 'DG9 7LT'),
('145 King Street', 'Lerwick', 'ZE1 0EQ'),
('150 High Street', 'Alloa', 'FK10 1JE'),
('155 Main Street', 'Peterhead', 'AB42 1TU'),
('160 High Street', 'Dalkeith', 'EH22 1AY'),
('165 Main Street', 'Strathaven', 'ML10 6YT'),
('170 King Street', 'Dingwall', 'IV15 9JZ'),
('175 High Street', 'Selkirk', 'TD7 4BZ'),
('180 Main Street', 'Largs', 'KA30 8AN'),
('185 King Street', 'Helensburgh', 'G84 8BZ'),
('190 High Street', 'Dunbar', 'EH42 1EN'),
('195 Main Street', 'Clydebank', 'G81 1UE'),
('200 King Street', 'Lochgilphead', 'PA31 8JN'),
('205 High Street', 'Forfar', 'DD8 1BZ'),
('210 Main Street', 'Barrhead', 'G78 1SN'),
('215 King Street', 'Kirkwall', 'KW15 1GJ'),
('220 High Street', 'Linlithgow', 'EH49 7ER'),
('225 Main Street', 'Milngavie', 'G62 6BW'),
('230 King Street', 'Rothesay', 'PA20 0DE'),
('235 High Street', 'Campbeltown', 'PA28 6BJ'),
('240 Main Street', 'Dunoon', 'PA23 7DT'),
('245 King Street', 'Buckie', 'AB56 1AB'),
('250 High Street', 'Nairn', 'IV12 4AG'),
('255 Main Street', 'Musselburgh', 'EH21 7DA');

-- Populate sample medical records data
INSERT INTO @LaboratoryReports VALUES 
('Blood test normal'), 
('Elevated white blood cells'), 
('Low hemoglobin levels');

INSERT INTO @TelephoneCalls VALUES 
('Discussed recent blood test results'), 
('Patient reported side effects of medication'), 
('Scheduled follow-up appointment');

INSERT INTO @Xrays VALUES 
('Chest X-ray normal'), 
('Evidence of broken rib'), 
('Signs of pneumonia in left lung');

INSERT INTO @Letters VALUES 
('Referral to cardiologist'), 
('Follow-up after surgery'), 
('Recommendation for physiotherapy');

INSERT INTO @PrescriptionCharts VALUES 
('Prescribed Amoxicillin 500mg'), 
('Changed medication to Lisinopril 10mg'), 
('Advised to continue with current medication');

INSERT INTO @ClinicalNotes VALUES 
('Patient reported mild chest pain'), 
('Follow-up after recent surgery'), 
('No signs of infection, wound healing well');


-- Inserting 50 Random Patients
DECLARE @i INT = 1;
DECLARE @PatientId INT;
DECLARE @RandomFirstName NVARCHAR(50);
DECLARE @RandomLastName NVARCHAR(50);
DECLARE @RandomAddress NVARCHAR(255);
DECLARE @RandomCity NVARCHAR(50);
DECLARE @RandomPostcode NVARCHAR(10);
DECLARE @RandomNHSNumber NVARCHAR(10);
DECLARE @RandomLabReport NVARCHAR(255);
DECLARE @RandomCall NVARCHAR(255);
DECLARE @RandomXray NVARCHAR(255);
DECLARE @RandomLetter NVARCHAR(255);
DECLARE @RandomPrescription NVARCHAR(255);
DECLARE @RandomNote NVARCHAR(255);

WHILE @i <= 50
BEGIN
    -- Get random names, addresses, and NHS number
    SELECT TOP 1 @RandomFirstName = Name FROM @FirstNames ORDER BY NEWID();
    SELECT TOP 1 @RandomLastName = Name FROM @LastNames ORDER BY NEWID();
    SELECT TOP 1 @RandomAddress = Address, @RandomCity = City, @RandomPostcode = Postcode FROM @Addresses ORDER BY NEWID();
    SET @RandomNHSNumber = LEFT(CONVERT(NVARCHAR(10), ABS(CHECKSUM(NEWID())) % 9000000000 + 1000000000), 10);

    INSERT INTO [dbo].[Patients] ([FirstName], [LastName], [NHSNumber], [Address], [City], [Postcode])
    VALUES 
    (@RandomFirstName, @RandomLastName, @RandomNHSNumber, @RandomAddress, @RandomCity, @RandomPostcode);

    -- Capture the Id of the inserted patient
    SET @PatientId = SCOPE_IDENTITY();

    -- Generate random medical records or 'N/A' with 25% probability
    SELECT TOP 1 @RandomLabReport = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Report END FROM @LaboratoryReports ORDER BY NEWID();
SELECT TOP 1 @RandomCall = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Call END FROM @TelephoneCalls ORDER BY NEWID();
SELECT TOP 1 @RandomXray = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Xray END FROM @Xrays ORDER BY NEWID();
SELECT TOP 1 @RandomLetter = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Letter END FROM @Letters ORDER BY NEWID();
SELECT TOP 1 @RandomPrescription = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Prescription END FROM @PrescriptionCharts ORDER BY NEWID();
SELECT TOP 1 @RandomNote = CASE WHEN ABS(CHECKSUM(NEWID())) % 1000 / 1000.0 <= 0.25 THEN 'N/A' ELSE Note END FROM @ClinicalNotes ORDER BY NEWID();

    INSERT INTO [dbo].[MedicalRecords] ([LaboratoryReports], [TelephoneCalls], [Xrays], [Letters], [PrescriptionCharts], [ClinicalNotes], [PatientId])
    VALUES 
    (@RandomLabReport, @RandomCall, @RandomXray, @RandomLetter, @RandomPrescription, @RandomNote, @PatientId);

    SET @i = @i + 1;
END;
