-- ============================================================
-- SGVO — Rollback: Restore Tipo (VARCHAR) from TipoUnidadOrganizativaId
-- Reverse of 07-migrate-tipo-to-fk.sql
-- ============================================================
USE sgvo;

-- Step 1: Restore Tipo column (nullable during restore)
ALTER TABLE UnidadesOrganizativas
    ADD COLUMN Tipo VARCHAR(50) NULL COMMENT 'Facultad, Secretaría, Dirección, Departamento, División, Área'
        AFTER Nombre;

-- Step 2: Populate Tipo from TiposUnidadOrganizativa
UPDATE UnidadesOrganizativas u
    INNER JOIN TiposUnidadOrganizativa t ON u.TipoUnidadOrganizativaId = t.Id
SET u.Tipo = t.Nombre
WHERE u.Tipo IS NULL;

-- Step 3: Make Tipo NOT NULL
ALTER TABLE UnidadesOrganizativas
    MODIFY COLUMN Tipo VARCHAR(50) NOT NULL;

-- Step 4: Restore Tipo index
ALTER TABLE UnidadesOrganizativas
    ADD INDEX IX_UnidadesOrganizativas_Tipo (Tipo);

-- Step 5: Drop FK constraint and column
ALTER TABLE UnidadesOrganizativas
    DROP FOREIGN KEY FK_UnidadesOrganizativas_Tipo,
    DROP COLUMN TipoUnidadOrganizativaId;

-- Step 6: Drop TiposUnidadOrganizativa table
DROP TABLE IF EXISTS TiposUnidadOrganizativa;
