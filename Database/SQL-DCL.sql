-- SQL Server DCL permissions
-- Assumptions:
-- - Database roles are used for application access
-- - Fine-grained member ownership checks should still be enforced in the API or with row-level security later

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_admin' AND type = 'R')
	CREATE ROLE app_admin;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_staff' AND type = 'R')
	CREATE ROLE app_staff;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_member' AND type = 'R')
	CREATE ROLE app_member;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'app_readonly' AND type = 'R')
	CREATE ROLE app_readonly;

-- Admin role: full control over the schema objects used by the app
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.athlete TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.location TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.room TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.workout TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.workout_equipment TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.subscriptionPlan TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.subscription TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipment TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentRoom TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentSpot TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.schedule TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.lesson TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.reservation TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.payment TO app_admin;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentSpot_reservation TO app_admin;

-- Staff role: manage the operational data used by the Blazor admin app
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.athlete TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.location TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.room TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.workout TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.workout_equipment TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.subscriptionPlan TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.subscription TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipment TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentRoom TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentSpot TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.schedule TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.lesson TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.reservation TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.payment TO app_staff;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.equipmentSpot_reservation TO app_staff;

-- Member role: read public schedules and use the API for own reservation flows
GRANT SELECT ON dbo.location TO app_member;
GRANT SELECT ON dbo.room TO app_member;
GRANT SELECT ON dbo.workout TO app_member;
GRANT SELECT ON dbo.workout_equipment TO app_member;
GRANT SELECT ON dbo.subscriptionPlan TO app_member;
GRANT SELECT ON dbo.subscription TO app_member;
GRANT SELECT ON dbo.equipment TO app_member;
GRANT SELECT ON dbo.equipmentRoom TO app_member;
GRANT SELECT ON dbo.equipmentSpot TO app_member;
GRANT SELECT ON dbo.schedule TO app_member;
GRANT SELECT ON dbo.lesson TO app_member;

-- Read-only role for reporting and demo access
GRANT SELECT ON dbo.location TO app_readonly;
GRANT SELECT ON dbo.room TO app_readonly;
GRANT SELECT ON dbo.workout TO app_readonly;
GRANT SELECT ON dbo.workout_equipment TO app_readonly;
GRANT SELECT ON dbo.subscriptionPlan TO app_readonly;
GRANT SELECT ON dbo.subscription TO app_readonly;
GRANT SELECT ON dbo.equipment TO app_readonly;
GRANT SELECT ON dbo.equipmentRoom TO app_readonly;
GRANT SELECT ON dbo.equipmentSpot TO app_readonly;
GRANT SELECT ON dbo.schedule TO app_readonly;
GRANT SELECT ON dbo.lesson TO app_readonly;

-- Create a database user and assign to the appropriate role(s)
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'dbuser')
BEGIN
	CREATE USER dbuser WITHOUT LOGIN;
END;

IF NOT EXISTS (
	SELECT 1
	FROM sys.database_role_members drm
	JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
	JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
	WHERE r.name = N'app_admin' AND m.name = N'dbuser'
)
BEGIN
	ALTER ROLE app_admin ADD MEMBER dbuser;
END;