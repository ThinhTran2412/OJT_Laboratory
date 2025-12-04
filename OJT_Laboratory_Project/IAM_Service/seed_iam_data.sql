-- First, create a role for patients if not exists
INSERT INTO "Roles" ("Name", "Code", "Description")
SELECT 'Patient', 'PATIENT', 'Regular patient role with access to personal medical records'
WHERE NOT EXISTS (
    SELECT 1 FROM "Roles" WHERE "Code" = 'PATIENT'
);

-- Insert users (matching with patients in Laboratory Service)
INSERT INTO "Users" 
("FullName", "Email", "PhoneNumber", "IdentifyNumber", "Gender", "Age", "Address", "DateOfBirth", "Password", "FailedLoginAttempts")
VALUES 
    ('Nguyen Van A', 'nguyenvana@example.com', '0901234567', '123456789001', 'Male', 33, '123 Le Loi, District 1, HCMC', '1990-05-15', 
    -- Password: Patient@123 (hashed)
    '9ED31D8D0EFC2D9CD5622D6FF1CC32B6F6D27DC186BDFF8FAA78492A4C49B336', 0),

    ('Tran Thi B', 'tranthib@example.com', '0912345678', '123456789002', 'Female', 38, '456 Nguyen Hue, District 1, HCMC', '1985-08-22',
    '9ED31D8D0EFC2D9CD5622D6FF1CC32B6F6D27DC186BDFF8FAA78492A4C49B336', 0),

    ('Le Van C', 'levanc@example.com', '0923456789', '123456789003', 'Male', 30, '789 Ham Nghi, District 1, HCMC', '1995-03-10',
    '9ED31D8D0EFC2D9CD5622D6FF1CC32B6F6D27DC186BDFF8FAA78492A4C49B336', 0),

    ('Pham Thi D', 'phamthid@example.com', '0934567890', '123456789004', 'Female', 35, '321 Ly Tu Trong, District 1, HCMC', '1988-12-25',
    '9ED31D8D0EFC2D9CD5622D6FF1CC32B6F6D27DC186BDFF8FAA78492A4C49B336', 0),

    ('Hoang Van E', 'hoangvane@example.com', '0945678901', '123456789005', 'Male', 33, '654 Dong Khoi, District 1, HCMC', '1992-07-30',
    '9ED31D8D0EFC2D9CD5622D6FF1CC32B6F6D27DC186BDFF8FAA78492A4C49B336', 0);

-- Assign Patient role to all new users
UPDATE "Users" 
SET "RoleId" = r."RoleId"
FROM "Roles" r
WHERE r."Code" = 'PATIENT'
AND "Users"."IdentifyNumber" IN ('123456789001', '123456789002', '123456789003', '123456789004', '123456789005');