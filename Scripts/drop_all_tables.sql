-- ====================================================================
-- Script to DROP all tables in all service schemas
-- WARNING: This will DELETE all data in all tables!
-- ====================================================================
-- This script is used when you need to force recreate all tables
-- Run this before applying migrations if tables already exist
-- ====================================================================

-- Drop all tables in iam_service schema
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'iam_service') 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS iam_service.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

-- Drop all tables in laboratory_service schema
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'laboratory_service') 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS laboratory_service.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

-- Drop all tables in monitoring_service schema
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'monitoring_service') 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS monitoring_service.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

-- Drop all tables in simulator_service schema
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'simulator_service') 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS simulator_service.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

-- Drop all tables in warehouse_service schema
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'warehouse_service') 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS warehouse_service.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

-- Also drop tables in public schema that might belong to services (if any)
DO $$
DECLARE
    r RECORD;
BEGIN
    FOR r IN (
        SELECT tablename FROM pg_tables 
        WHERE schemaname = 'public' 
        AND tablename IN ('Instruments', 'AuditLogs', 'EventLogs', 'RawResults', 'RawTestResults', '__EFMigrationsHistory')
    ) 
    LOOP
        EXECUTE 'DROP TABLE IF EXISTS public.' || quote_ident(r.tablename) || ' CASCADE';
    END LOOP;
END $$;

SELECT 'All tables dropped successfully!' AS result;

