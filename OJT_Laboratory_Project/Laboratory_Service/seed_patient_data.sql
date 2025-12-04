-- Seed data for Patients
INSERT INTO "Patients" 
("PatientId", "FullName", "IdentifyNumber", "DateOfBirth", "Gender", "PhoneNumber", "Email", "Address", "CreatedAt", "CreatedBy")
VALUES 
    (1, 'Nguyen Van A', '123456789001', '1990-05-15', 'Male', '0901234567', 'nguyenvana@github.com', '123 Le Loi, District 1, HCMC', CURRENT_TIMESTAMP, 'System'),
    (2, 'Tran Thi B', '123456789002', '1985-08-22', 'Female', '0912345678', 'tranthib@gmail.com', '456 Nguyen Hue, District 1, HCMC', CURRENT_TIMESTAMP, 'System'),
    (3, 'Le Van C', '123456789003', '1995-03-10', 'Male', '0923456789', 'levanc@example.com', '789 Ham Nghi, District 1, HCMC', CURRENT_TIMESTAMP, 'System'),
    (4, 'Pham Thi D', '123456789004', '1988-12-25', 'Female', '0934567890', 'phamthid@example.com', '321 Ly Tu Trong, District 1, HCMC', CURRENT_TIMESTAMP, 'System'),
    (5, 'Hoang Van E', '123456789005', '1992-07-30', 'Male', '0945678901', 'hoangvane@example.com', '654 Dong Khoi, District 1, HCMC', CURRENT_TIMESTAMP, 'System');

-- Seed data for Medical Records
INSERT INTO "MedicalRecords" 
("MedicalRecordId", "PatientId", "TestType", "TestResults", "ReferenceRanges", "Interpretation", "InstrumentUsed", 
"BatchNumber", "LotNumber", "ClinicalNotes", "ErrorMessages", "TestDate", "ResultsDate", "Status", "Priority", 
"CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy")
VALUES 
    -- Medical records for Nguyen Van A
    (1, 1, 'Blood Test', 'WBC: 7.5, RBC: 4.8, Hemoglobin: 14.2', 'WBC: 4.5-11.0, RBC: 4.5-5.5, Hemoglobin: 13.5-17.5', 
    'All values within normal range', 'Sysmex XN-1000', 'BAT2025101', 'LOT2025A101', 'Routine blood work, patient fasting', 
    '', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'Completed', 'Normal', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'System', 'System'),
    
    (2, 1, 'Allergy Test', 'Positive for dust mites and pollen', 'Negative control: <3mm, Positive control: >3mm', 
    'Patient shows allergic response to common allergens', 'Skin Prick Test Kit', 'BAT2025102', 'LOT2025A102', 
    'Patient reports seasonal allergy symptoms', '', CURRENT_TIMESTAMP - INTERVAL '30 days', 
    CURRENT_TIMESTAMP - INTERVAL '30 days', 'Completed', 'Normal', CURRENT_TIMESTAMP - INTERVAL '30 days', 
    CURRENT_TIMESTAMP - INTERVAL '30 days', 'System', 'System'),
    
    -- Medical records for Tran Thi B
    (3, 2, 'Blood Pressure', '140/90 mmHg', 'Systolic: <120, Diastolic: <80', 'Stage 1 Hypertension', 'Omron HEM-7320',
    'BAT2025103', 'LOT2025A103', 'Regular check-up, patient reports headaches', '', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP,
    'Completed', 'High', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'System', 'System'),
    
    (4, 2, 'HbA1c Test', '7.2%', '4.0-5.6%', 'Indicates diabetes - above normal range', 'Roche Cobas c513',
    'BAT2025104', 'LOT2025A104', 'Follow-up diabetes screening', '', CURRENT_TIMESTAMP - INTERVAL '60 days',
    CURRENT_TIMESTAMP - INTERVAL '60 days', 'Completed', 'High', CURRENT_TIMESTAMP - INTERVAL '60 days',
    CURRENT_TIMESTAMP - INTERVAL '60 days', 'System', 'System'),
    
    -- Medical records for Le Van C
    (5, 3, 'Chest X-Ray', 'Clear lung fields, no consolidation', 'No acute cardiopulmonary process', 'Normal chest radiograph',
    'Philips DigitalDiagnost', 'BAT2025105', 'LOT2025A105', 'Routine chest examination', '', CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP, 'Completed', 'Normal', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'System', 'System'),
    
    -- Medical records for Pham Thi D
    (6, 4, 'Neurological Exam', 'Normal reflexes, no focal deficits', 'Standard neurological parameters', 
    'No significant neurological findings', 'Clinical Assessment Tools', 'BAT2025106', 'LOT2025A106',
    'Patient reports frequent headaches', '', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'Completed', 'Normal',
    CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'System', 'System'),
    
    (7, 4, 'Iron Panel', 'Ferritin: 15 ng/mL, Iron: 45 μg/dL', 'Ferritin: 20-200 ng/mL, Iron: 60-170 μg/dL',
    'Results indicate iron deficiency', 'Beckman Coulter DxI 800', 'BAT2025107', 'LOT2025A107',
    'Patient presents with fatigue', '', CURRENT_TIMESTAMP - INTERVAL '45 days', CURRENT_TIMESTAMP - INTERVAL '45 days',
    'Completed', 'Normal', CURRENT_TIMESTAMP - INTERVAL '45 days', CURRENT_TIMESTAMP - INTERVAL '45 days', 'System', 'System'),
    
    -- Medical records for Hoang Van E
    (8, 5, 'MRI Scan', 'Mild L4-L5 disc herniation', 'No acute spinal cord compression', 'Lumbar disc herniation present',
    'Siemens MAGNETOM Aera', 'BAT2025108', 'LOT2025A108', 'Lower back pain investigation', '', 
    CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'Completed', 'Urgent', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'System', 'System'),
    
    (9, 5, 'Psychological Assessment', 'GAD-7 Score: 12', 'GAD-7: 0-21 (>10 indicates moderate anxiety)',
    'Moderate anxiety level indicated', 'Standardized Assessment Tools', 'BAT2025109', 'LOT2025A109',
    'Regular mental health screening', '', CURRENT_TIMESTAMP - INTERVAL '15 days', CURRENT_TIMESTAMP - INTERVAL '15 days',
    'Completed', 'Normal', CURRENT_TIMESTAMP - INTERVAL '15 days', CURRENT_TIMESTAMP - INTERVAL '15 days', 'System', 'System');