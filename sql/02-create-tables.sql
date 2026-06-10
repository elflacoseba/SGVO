-- ============================================================
-- SGVO — Creación de tablas (ordenado por dependencias)
-- ============================================================
USE sgvo;

-- -----------------------------------------------------------
-- 1. UnidadesOrganizativas (sin FKs salvo self-reference)
-- -----------------------------------------------------------
CREATE TABLE UnidadesOrganizativas (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(200)    NOT NULL,
    Tipo              VARCHAR(50)     NOT NULL COMMENT 'Facultad, Secretaría, Dirección, Departamento, División, Área',
    PadreId           BIGINT UNSIGNED NULL,
    NivelJerarquico   TINYINT UNSIGNED NOT NULL DEFAULT 1,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_UnidadesOrganizativas_PadreId (PadreId),
    INDEX IX_UnidadesOrganizativas_Tipo (Tipo),
    CONSTRAINT FK_UnidadesOrganizativas_Padre
        FOREIGN KEY (PadreId) REFERENCES UnidadesOrganizativas(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 2. Cargos (sin dependencias)
-- -----------------------------------------------------------
CREATE TABLE Cargos (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(150)    NOT NULL,
    Descripcion       VARCHAR(500)    NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 3. Skills (sin dependencias)
-- -----------------------------------------------------------
CREATE TABLE Skills (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(150)    NOT NULL,
    Categoria         VARCHAR(100)    NULL COMMENT 'Técnica, Blanda, Gerencial, etc.',
    Descripcion       VARCHAR(500)    NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 4. Personas (sin dependencias)
-- -----------------------------------------------------------
CREATE TABLE Personas (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(150)    NOT NULL,
    Apellido          VARCHAR(150)    NOT NULL,
    Email             VARCHAR(200)    NOT NULL,
    Legajo            VARCHAR(50)     NULL,
    Documento         VARCHAR(50)     NOT NULL,
    FechaNacimiento   DATE            NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_Personas_Email (Email),
    UNIQUE INDEX IX_Personas_Documento (Documento),
    UNIQUE INDEX IX_Personas_Legajo (Legajo)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 5. CargoSkills (M:N entre Cargos y Skills)
-- -----------------------------------------------------------
CREATE TABLE CargoSkills (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    CargoId           BIGINT UNSIGNED NOT NULL,
    SkillId           BIGINT UNSIGNED NOT NULL,
    NivelImportancia  TINYINT UNSIGNED NOT NULL COMMENT '1=Crítico, 2=Importante, 3=Deseable, 4=Secundario',
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_CargoSkills_Unique (CargoId, SkillId),
    CONSTRAINT FK_CargoSkills_Cargo FOREIGN KEY (CargoId) REFERENCES Cargos(Id),
    CONSTRAINT FK_CargoSkills_Skill FOREIGN KEY (SkillId) REFERENCES Skills(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 6. PersonaSkills (M:N entre Personas y Skills)
-- -----------------------------------------------------------
CREATE TABLE PersonaSkills (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    PersonaId         BIGINT UNSIGNED NOT NULL,
    SkillId           BIGINT UNSIGNED NOT NULL,
    NivelDominio      TINYINT UNSIGNED NOT NULL COMMENT '1=Básico, 2=Intermedio, 3=Avanzado, 4=Experto',
    FechaCertificacion DATE           NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_PersonaSkills_Unique (PersonaId, SkillId),
    CONSTRAINT FK_PersonaSkills_Persona FOREIGN KEY (PersonaId) REFERENCES Personas(Id),
    CONSTRAINT FK_PersonaSkills_Skill FOREIGN KEY (SkillId) REFERENCES Skills(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 7. Puestos (depende de UnidadesOrganizativas y Cargos + self-reference)
-- -----------------------------------------------------------
CREATE TABLE Puestos (
    Id                     BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre                 VARCHAR(200)    NOT NULL,
    UnidadOrganizativaId   BIGINT UNSIGNED NOT NULL,
    CargoId                BIGINT UNSIGNED NOT NULL,
    SuperiorId             BIGINT UNSIGNED NULL,
    Codigo                 VARCHAR(50)     NULL,
    Activo                 BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn               DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn           DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Puestos_UnidadCargo (UnidadOrganizativaId, CargoId),
    UNIQUE INDEX IX_Puestos_Codigo (Codigo),
    INDEX IX_Puestos_SuperiorId (SuperiorId),
    CONSTRAINT FK_Puestos_Unidad FOREIGN KEY (UnidadOrganizativaId) REFERENCES UnidadesOrganizativas(Id),
    CONSTRAINT FK_Puestos_Cargo FOREIGN KEY (CargoId) REFERENCES Cargos(Id),
    CONSTRAINT FK_Puestos_Superior FOREIGN KEY (SuperiorId) REFERENCES Puestos(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 8. Ocupaciones (depende de Personas, Puestos)
-- -----------------------------------------------------------
CREATE TABLE Ocupaciones (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    PersonaId         BIGINT UNSIGNED NOT NULL,
    PuestoId          BIGINT UNSIGNED NOT NULL,
    FechaInicio       DATE            NOT NULL,
    FechaFin          DATE            NULL COMMENT 'NULL = vigente',
    TipoOcupacion     VARCHAR(50)     NOT NULL COMMENT 'Permanente, Interina, Suplente',
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Ocupaciones_PersonaVigente (PersonaId, FechaFin),
    INDEX IX_Ocupaciones_PuestoVigente (PuestoId, FechaFin),
    CONSTRAINT FK_Ocupaciones_Persona FOREIGN KEY (PersonaId) REFERENCES Personas(Id),
    CONSTRAINT FK_Ocupaciones_Puesto FOREIGN KEY (PuestoId) REFERENCES Puestos(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 9. Vacantes (depende de Puestos, Personas — ResponsableId)
-- -----------------------------------------------------------
CREATE TABLE Vacantes (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    PuestoId          BIGINT UNSIGNED NOT NULL,
    FechaApertura     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaCierre       DATETIME        NULL,
    Motivo            VARCHAR(500)    NOT NULL,
    Estado            VARCHAR(50)     NOT NULL DEFAULT 'Abierta' COMMENT 'Abierta, EnSeleccion, Cubierta, Cancelada',
    Observaciones     VARCHAR(1000)   NULL,
    ResponsableId     BIGINT UNSIGNED NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Vacantes_PuestoEstado (PuestoId, Estado),
    INDEX IX_Vacantes_Estado (Estado),
    CONSTRAINT FK_Vacantes_Puesto FOREIGN KEY (PuestoId) REFERENCES Puestos(Id),
    CONSTRAINT FK_Vacantes_Responsable FOREIGN KEY (ResponsableId) REFERENCES Personas(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 10. Postulantes (depende de Personas — opcional)
-- -----------------------------------------------------------
CREATE TABLE Postulantes (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    PersonaId         BIGINT UNSIGNED NULL COMMENT 'NULL si es externo',
    Nombre            VARCHAR(150)    NOT NULL,
    Apellido          VARCHAR(150)    NOT NULL,
    Email             VARCHAR(200)    NOT NULL,
    Telefono          VARCHAR(50)     NULL,
    Origen            VARCHAR(50)     NOT NULL DEFAULT 'Externo' COMMENT 'Interno, Externo, Recomendado',
    CurriculumUrl     VARCHAR(500)    NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    INDEX IX_Postulantes_PersonaId (PersonaId),
    CONSTRAINT FK_Postulantes_Persona FOREIGN KEY (PersonaId) REFERENCES Personas(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 11. Postulaciones (depende de Vacantes, Postulantes, Personas — EvaluadorId)
-- -----------------------------------------------------------
CREATE TABLE Postulaciones (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    VacanteId         BIGINT UNSIGNED NOT NULL,
    PostulanteId      BIGINT UNSIGNED NOT NULL,
    FechaPostulacion  DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Estado            VARCHAR(50)     NOT NULL DEFAULT 'Postulado' COMMENT 'Postulado, Preseleccionado, Entrevistado, Aprobado, Rechazado, Contratado',
    PuntajeMatch      DECIMAL(5,2)    NULL COMMENT '0.00 - 100.00',
    CumplePerfilTotal BOOLEAN         NULL,
    Observaciones     VARCHAR(2000)   NULL,
    FechaEvaluacion   DATETIME        NULL,
    EvaluadorId       BIGINT UNSIGNED NULL,
    Activo            BOOLEAN         NOT NULL DEFAULT TRUE,
    CreadoEn          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificadoEn      DATETIME        NULL ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (Id),
    UNIQUE INDEX IX_Postulaciones_VacantePostulante (VacanteId, PostulanteId),
    INDEX IX_Postulaciones_VacanteEstado (VacanteId, Estado),
    INDEX IX_Postulaciones_PuntajeMatch (PuntajeMatch),
    CONSTRAINT FK_Postulaciones_Vacante FOREIGN KEY (VacanteId) REFERENCES Vacantes(Id),
    CONSTRAINT FK_Postulaciones_Postulante FOREIGN KEY (PostulanteId) REFERENCES Postulantes(Id),
    CONSTRAINT FK_Postulaciones_Evaluador FOREIGN KEY (EvaluadorId) REFERENCES Personas(Id)
) ENGINE=InnoDB;

-- -----------------------------------------------------------
-- 12. Auditorias (tabla única transversal)
-- -----------------------------------------------------------
CREATE TABLE Auditorias (
    Id                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    Tabla             VARCHAR(100)    NOT NULL COMMENT 'Nombre de la tabla afectada',
    EntidadId         BIGINT UNSIGNED NOT NULL COMMENT 'PK de la entidad modificada',
    Operacion         VARCHAR(20)     NOT NULL COMMENT 'CREATE, UPDATE, DELETE',
    UsuarioId         BIGINT UNSIGNED NULL,
    FechaHora         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ValoresAnterior   JSON            NULL COMMENT 'Estado anterior en JSON',
    ValoresNuevo      JSON            NULL COMMENT 'Estado nuevo en JSON',
    IpAddress         VARCHAR(50)     NULL,
    UserAgent         VARCHAR(500)    NULL,
    PRIMARY KEY (Id),
    INDEX IX_Auditorias_TablaEntidad (Tabla, EntidadId),
    INDEX IX_Auditorias_FechaHora (FechaHora)
) ENGINE=InnoDB;
