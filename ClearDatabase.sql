-- Clear all data from database tables
-- Execute this in your PostgreSQL database

-- Delete all records from tables (order matters due to foreign keys)
DELETE FROM "Replies";
DELETE FROM "PrivateMessages";
DELETE FROM "Threads";
DELETE FROM "Users";

-- Reset sequences (auto-increment counters)
-- This is optional - only if you want IDs to start from 1 again
-- ALTER SEQUENCE "Users_Id_seq" RESTART WITH 1;
-- ALTER SEQUENCE "Threads_Id_seq" RESTART WITH 1;
-- ALTER SEQUENCE "Replies_Id_seq" RESTART WITH 1;
-- ALTER SEQUENCE "PrivateMessages_Id_seq" RESTART WITH 1;

-- Verify tables are empty
SELECT 'Users' as table_name, COUNT(*) as count FROM "Users"
UNION ALL
SELECT 'Threads', COUNT(*) FROM "Threads"
UNION ALL
SELECT 'Replies', COUNT(*) FROM "Replies"
UNION ALL
SELECT 'PrivateMessages', COUNT(*) FROM "PrivateMessages";
