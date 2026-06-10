-- ============================================================
-- SGVO — Migración: Usuarios, Roles y ajuste de Auditorías
-- ============================================================
USE sgvo;

-- -----------------------------------------------------------
-- 1. Roles (catálogo de roles del sistema)
-- -----------------------------------------------------------
CREATE TABLE Roles (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(100)    NOT NULL,
    Descripcion       VARCHAR(300)    NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_Roles_Nombre (Nombre)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 2. Usuarios (credenciales de acceso al sistema)
-- -----------------------------------------------------------
CREATE TABLE Usuarios (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Username          VARCHAR(100)    NOT NULL,
    Email             VARCHAR(200)    NOT NULL,
    PasswordHash      VARCHAR(500)    NOT NULL,
    PersonaId         BIGINT UNSIGNED NULL COMMENT 'Vincula con Personas si el usuario es empleado',
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    UltimoAcceso      DATETIME        NULL,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_Usuarios_Username (Username),
    UNIQUE INDEX IX_Usuarios_Email (Email),
    INDEX IX_Usuarios_PersonaId (PersonaId),
    CONSTRAINT FK_Usuarios_Persona FOREIGN KEY (PersonaId) REFERENCES Personas(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 3. UsuarioRoles (M:N entre Usuarios y Roles)
-- -----------------------------------------------------------
CREATE TABLE UsuarioRoles (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    UsuarioId         BIGINT UNSIGNED NOT NULL,
    RolId             BIGINT UNSIGNED NOT NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_UsuarioRoles_Unique (UsuarioId, RolId),
    CONSTRAINT FK_UsuarioRoles_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_UsuarioRoles_Rol FOREIGN KEY (RolId) REFERENCES Roles(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 4. Ajustar Auditorias: FK a Usuarios
-- -----------------------------------------------------------
ALTER TABLE Auditorias
    ADD CONSTRAINT FK_Auditorias_Usuario
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id);

-- -----------------------------------------------------------
-- 5. Datos semilla: Roles
-- -----------------------------------------------------------
INSERT INTO Roles (Id, Nombre, Descripcion) VALUES
(1, 'Administrador',     'Acceso total al sistema. Gestión de usuarios y configuración.'),
(2, 'Director',          'Gestión de estructura organizacional, cargos, puestos y vacantes.'),
(3, 'RRHH',              'Gestión de personas, postulantes y procesos de selección.'),
(4, 'Consulta',          'Acceso de solo lectura a toda la información.');

-- -----------------------------------------------------------
-- 6. Datos semilla: Usuarios
--    Contraseñas en texto plano (solo para desarrollo):
--      admin      → Admin123!
--      cgarcia    → Carlos123!
--      mrodriguez → Maria123!
--    Hash: SHA-256 aplicado sobre la contraseña.
-- -----------------------------------------------------------
INSERT INTO Usuarios (Id, Username, Email, PasswordHash, PersonaId) VALUES
(1, 'admin',      'admin@sgvo.edu',            '3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121', NULL),  -- Admin123!
(2, 'cgarcia',    'carlos.garcia@sgvo.edu',    '9e8499f37fe199da614bf45a1eff123670691923b2ff70dda217f9156a07dadd', 1),     -- Carlos123!
(3, 'mrodriguez', 'maria.rodriguez@sgvo.edu',  'd13832997abd60c1021ec4437e1144ca41293974141a8ebc15ebb45a3e289e71', 2);    -- Maria123!

-- -----------------------------------------------------------
-- 7. Datos semilla: UsuarioRoles
-- -----------------------------------------------------------
INSERT INTO UsuarioRoles (UsuarioId, RolId) VALUES
(1, 1),  -- admin → Administrador
(2, 2),  -- cgarcia → Director
(3, 3);  -- mrodriguez → RRHH
