-- ============================================================
-- MIGRATION: Google Auth + Email Verification
-- Run this ONCE against your learningPlatformDataBase
-- ============================================================

-- 1. Add email column to [User]
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('[dbo].[User]') AND name = 'email'
)
BEGIN
    ALTER TABLE [dbo].[User] ADD [email] NVARCHAR(255) NULL;
    PRINT 'Added email column to [User]';
END

-- 2. Add googleId column to [User]
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('[dbo].[User]') AND name = 'googleId'
)
BEGIN
    ALTER TABLE [dbo].[User] ADD [googleId] NVARCHAR(255) NULL;
    PRINT 'Added googleId column to [User]';
END

-- 3. Add emailVerified column to [User]
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('[dbo].[User]') AND name = 'emailVerified'
)
BEGIN
    ALTER TABLE [dbo].[User] ADD [emailVerified] BIT NOT NULL DEFAULT 0;
    PRINT 'Added emailVerified column to [User]';
END

-- 4. Create EmailVerificationCode table
IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = 'EmailVerificationCode'
)
BEGIN
    CREATE TABLE [dbo].[EmailVerificationCode] (
        [id]        INT            IDENTITY(1,1) NOT NULL,
        [email]     NVARCHAR(255)  NOT NULL,
        [code]      NVARCHAR(10)   NOT NULL,
        [expiresAt] DATETIME       NOT NULL,
        [used]      BIT            NOT NULL DEFAULT 0,
        PRIMARY KEY CLUSTERED ([id] ASC)
    );
    PRINT 'Created EmailVerificationCode table';
END

PRINT 'Migration complete.';
