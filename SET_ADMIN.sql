-- SQL Script to Set a User as Administrator
-- Instructions:
-- 1. Open SQL Server Management Studio (SSMS) or Azure Data Studio
-- 2. Connect to: (localdb)\mssqllocaldb
-- 3. Select database: ForumDb
-- 4. Replace 'YourUsername' with the actual username
-- 5. Execute this script

-- View all users first
SELECT Username, Email, IsAdmin, CreatedAt 
FROM Users
ORDER BY CreatedAt;

-- Set a specific user as admin (replace 'YourUsername' with actual username)
UPDATE Users
SET IsAdmin = 1
WHERE Username = 'YourUsername';

-- Verify the change
SELECT Username, Email, IsAdmin, CreatedAt 
FROM Users
WHERE Username = 'YourUsername';

-- ALTERNATIVE: Set admin by ID
-- UPDATE Users
-- SET IsAdmin = 1
-- WHERE Id = 'your-user-id-guid-here';

-- Remove admin rights from a user
-- UPDATE Users
-- SET IsAdmin = 0
-- WHERE Username = 'YourUsername';
