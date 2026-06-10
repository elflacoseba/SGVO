-- ============================================================
-- SGVO — Migración: Soft-delete completo
-- Agrega EliminadoEn y EliminadoPor a todas las tablas de negocio
-- ============================================================
USE sgvo;

ALTER TABLE UnidadesOrganizativas
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_UnidadesOrganizativas_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Cargos
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Cargos_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Skills
    ADD COLUMN EliminadoEn DATETIME NULL AFTER CreadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Skills_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Personas
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Personas_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE CargoSkills
    ADD COLUMN EliminadoEn DATETIME NULL AFTER Activo,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_CargoSkills_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE PersonaSkills
    ADD COLUMN EliminadoEn DATETIME NULL AFTER Activo,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_PersonaSkills_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Puestos
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Puestos_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Ocupaciones
    ADD COLUMN EliminadoEn DATETIME NULL AFTER CreadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Ocupaciones_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Vacantes
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Vacantes_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Postulantes
    ADD COLUMN EliminadoEn DATETIME NULL AFTER CreadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Postulantes_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Postulaciones
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Postulaciones_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Roles
    ADD COLUMN EliminadoEn DATETIME NULL AFTER CreadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Roles_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE Usuarios
    ADD COLUMN EliminadoEn DATETIME NULL AFTER ModificadoEn,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_Usuarios_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);

ALTER TABLE UsuarioRoles
    ADD COLUMN EliminadoEn DATETIME NULL AFTER Activo,
    ADD COLUMN EliminadoPor BIGINT UNSIGNED NULL AFTER EliminadoEn,
    ADD CONSTRAINT FK_UsuarioRoles_EliminadoPor
        FOREIGN KEY (EliminadoPor) REFERENCES Usuarios(Id);
