-- Seed data for FlaggingConfig table
-- Based on standard blood test parameter reference ranges

-- WBC (White Blood Cell Count) - No gender distinction
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('WBC', 'White Blood Cell Count', 'The number of white blood cells (leukocytes) in the blood, which help fight infection.', 'cells/µL', NULL, 4000.0, 10000.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- RBC (Red Blood Cell Count) - Gender-specific
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('RBC', 'Red Blood Cell Count', 'The number of red blood cells (erythrocytes) in the blood, which carry oxygen throughout the body.', 'million/µL', 'Male', 4.7, 6.1, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('RBC', 'Red Blood Cell Count', 'The number of red blood cells (erythrocytes) in the blood, which carry oxygen throughout the body.', 'million/µL', 'Female', 4.2, 5.4, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Hb/HGB (Hemoglobin) - Gender-specific
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('Hb', 'Hemoglobin', 'The protein in red blood cells that carries oxygen from the lungs to the rest of the body.', 'g/dL', 'Male', 14.0, 18.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('Hb', 'Hemoglobin', 'The protein in red blood cells that carries oxygen from the lungs to the rest of the body.', 'g/dL', 'Female', 12.0, 16.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- HCT (Hematocrit) - Gender-specific
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('HCT', 'Hematocrit', 'The percentage of red blood cells in the total blood volume.', '%', 'Male', 42.0, 52.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
('HCT', 'Hematocrit', 'The percentage of red blood cells in the total blood volume.', '%', 'Female', 37.0, 47.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- PLT (Platelet Count) - No gender distinction
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('PLT', 'Platelet Count', 'The number of platelets (thrombocytes) in the blood, which help with blood clotting.', 'cells/µL', NULL, 150000.0, 350000.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- MCV (Mean Corpuscular Volume) - No gender distinction
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('MCV', 'Mean Corpuscular Volume', 'The average size of red blood cells.', 'fL', NULL, 80.0, 100.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- MCH (Mean Corpuscular Haemoglobin) - No gender distinction
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('MCH', 'Mean Corpuscular Haemoglobin', 'The average amount of hemoglobin in each red blood cell.', 'pg', NULL, 27.0, 33.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- MCHC (Mean Corpuscular Haemoglobin Concentration) - No gender distinction
INSERT INTO "FlaggingConfigs" 
("TestCode", "ParameterName", "Description", "Unit", "Gender", "Min", "Max", "Version", "IsActive", "EffectiveDate", "CreatedAt")
VALUES 
('MCHC', 'Mean Corpuscular Haemoglobin Concentration', 'The average concentration of hemoglobin in red blood cells.', 'g/dL', NULL, 32.0, 36.0, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

