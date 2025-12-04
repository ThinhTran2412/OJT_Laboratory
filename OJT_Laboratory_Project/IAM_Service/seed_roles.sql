-- Insert default roles
INSERT INTO "Roles" ("Name", "Code", "Description") VALUES 
('Read Only', 'READ_ONLY', 'Default role with read-only access'),
('Administrator', 'ADMIN', 'Full system access'),
('Manager', 'MANAGER', 'Management level access'),
('Employee', 'EMPLOYEE', 'Standard employee access');

-- Insert role-privilege relationships
-- READ_ONLY role gets READ_ONLY privilege (PrivilegeId = 1)
INSERT INTO "RolePrivileges" ("RoleId", "PrivilegeId") VALUES (1, 1);
