-- Migration 07: Fix RefreshTokens column types
-- Date: 2026-06-10
-- Issue: Pomelo auto-maps CHAR(36) to Guid, causing runtime cast errors
--        when the C# property is string.
-- Fix: Change CHAR columns to VARCHAR to prevent Pomelo's Guid detection.

ALTER TABLE RefreshTokens
    MODIFY COLUMN TokenHash varchar(64) NOT NULL,
    MODIFY COLUMN FamilyId varchar(36) NOT NULL;
