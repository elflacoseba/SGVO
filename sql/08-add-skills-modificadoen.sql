-- ============================================================
-- SGVO — Migración: Agregar ModificadoEn a Skills
-- La tabla Skills se creó sin ModificadoEn en 02-create-tables.sql
-- y 06-soft-delete-fields.sql no lo agregó.
-- ============================================================
USE sgvo;

ALTER TABLE Skills
    ADD COLUMN ModificadoEn DATETIME NULL AFTER CreadoEn;
