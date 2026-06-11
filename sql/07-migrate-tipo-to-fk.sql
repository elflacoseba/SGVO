-- ============================================================
-- SGVO — Migration: Replace Tipo (VARCHAR) with TipoUnidadOrganizativaId (FK)
-- 1. Create TiposUnidadOrganizativa reference table
-- 2. Seed existing Tipo values
-- 3. Add FK column, populate, make NOT NULL, drop old column
-- ============================================================
USE sgvo;

-- Step 1: Create TiposUnidadOrganizativa table
CREATE TABLE TiposUnidadOrganizativa (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(100)    NOT NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    EliminadoEn       DATETIME        NULL,
    EliminadoPor      BIGINT UNSIGNED NULL,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_TiposUnidadOrganizativa_Nombre (Nombre),
    CONSTRAINT FK_TiposUnidadOrganizativa_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id)
) ENGINE=InnoDB;

-- Step 2: Seed existing Tipo values from UnidadesOrganizativas
INSERT INTO TiposUnidadOrganizativa (Nombre)
SELECT DISTINCT Tipo
FROM UnidadesOrganizativas
WHERE Tipo IS NOT NULL AND Tipo <> ''
ORDER BY Tipo;

-- Step 3: Add TipoUnidadOrganizativaId column (nullable during migration)
ALTER TABLE UnidadesOrganizativas
    ADD COLUMN TipoUnidadOrganizativaId BIGINT UNSIGNED NULL AFTER Tipo;

-- Step 4: Populate FK from matching Tipo values
UPDATE UnidadesOrganizativas u
    INNER JOIN TiposUnidadOrganizativa t ON u.Tipo = t.Nombre
SET u.TipoUnidadOrganizativaId = t.Id
WHERE u.TipoUnidadOrganizativaId IS NULL;

-- Step 5: Verify no NULLs remain (safety check)
-- If this fails, there are Tipo values not covered by the seed
-- SELECT COUNT(*) FROM UnidadesOrganizativas WHERE TipoUnidadOrganizativaId IS NULL;

-- Step 6: Make NOT NULL and add FK constraint
ALTER TABLE UnidadesOrganizativas
    MODIFY COLUMN TipoUnidadOrganizativaId BIGINT UNSIGNED NOT NULL,
    ADD CONSTRAINT FK_UnidadesOrganizativas_Tipo
        FOREIGN KEY (TipoUnidadOrganizativaId) REFERENCES TiposUnidadOrganizativa(Id);

-- Step 7: Drop old Tipo column and its index
ALTER TABLE UnidadesOrganizativas
    DROP INDEX IX_UnidadesOrganizativas_Tipo,
    DROP COLUMN Tipo;
