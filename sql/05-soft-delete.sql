-- ============================================================
-- SGVO — Migración: Soft-delete en tablas puente
-- Agrega Activo a las tablas de relación que no lo tenían
-- ============================================================
USE sgvo;

-- -----------------------------------------------------------
-- 1. CargoSkills: agregar Activo
-- -----------------------------------------------------------
ALTER TABLE CargoSkills
    ADD COLUMN Activo BOOLEAN NOT NULL DEFAULT TRUE AFTER NivelImportancia;

-- -----------------------------------------------------------
-- 2. PersonaSkills: agregar Activo
-- -----------------------------------------------------------
ALTER TABLE PersonaSkills
    ADD COLUMN Activo BOOLEAN NOT NULL DEFAULT TRUE AFTER FechaCertificacion;

-- -----------------------------------------------------------
-- 3. UsuarioRoles: agregar Activo
-- -----------------------------------------------------------
ALTER TABLE UsuarioRoles
    ADD COLUMN Activo BOOLEAN NOT NULL DEFAULT TRUE AFTER RolId;

-- -----------------------------------------------------------
-- 4. Verificación: tablas CON Activo (todas las entidades principales)
-- -----------------------------------------------------------
-- UnidadesOrganizativas  ✓ (ya existía)
-- Cargos                 ✓ (ya existía)
-- Skills                 ✓ (ya existía)
-- Personas               ✓ (ya existía)
-- CargoSkills            ✓ (esta migración)
-- PersonaSkills          ✓ (esta migración)
-- Puestos                ✓ (ya existía)
-- Ocupaciones            ✓ (ya existía)
-- Vacantes               ✓ (ya existía)
-- Postulantes            ✓ (ya existía)
-- Postulaciones          ✓ (ya existía)
-- Roles                  ✓ (ya existía)
-- Usuarios               ✓ (ya existía)
-- UsuarioRoles           ✓ (esta migración)

-- Auditorias NO tiene Activo por diseño: es append-only inmutable.
