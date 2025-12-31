-- SQL Script to Set a User as Administrator
-- Instructions:
-- 1. Connect to your PostgreSQL database
-- 2. Replace 'YourUsername' with the actual username
-- 3. Execute this script

-- View all users first
SELECT "Username", "Email", "IsAdmin", "CreatedAt" 
FROM "Users"
ORDER BY "CreatedAt";

-- Set a specific user as admin (replace 'YourUsername' with actual username)
UPDATE "Users"
SET "IsAdmin" = true
WHERE "Username" = 'YourUsername';

-- Verify the change
SELECT "Username", "Email", "IsAdmin", "CreatedAt" 
FROM "Users"
WHERE "Username" = 'YourUsername';

-- ALTERNATIVE: Set admin by ID
-- UPDATE "Users"
-- SET "IsAdmin" = true
-- WHERE "Id" = 'your-user-id-guid-here';

-- Remove admin rights from a user
-- UPDATE "Users"
-- SET "IsAdmin" = false
-- WHERE "Username" = 'YourUsername';
