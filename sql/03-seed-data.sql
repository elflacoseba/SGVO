-- ============================================================
-- SGVO — Datos semilla
-- ============================================================
USE sgvo;

-- -----------------------------------------------------------
-- Unidades Organizativas (jerarquía)
-- -----------------------------------------------------------
INSERT INTO UnidadesOrganizativas (Id, Nombre, Tipo, PadreId, NivelJerarquico) VALUES
(1, 'Universidad',                                          'Facultad',     NULL, 1),
(2, 'Facultad de Ingeniería',                               'Facultad',     1,    2),
(3, 'Facultad de Ciencias Económicas',                      'Facultad',     1,    2),
(4, 'Secretaría Académica',                                 'Secretaría',   1,    2),
(5, 'Dirección de Sistemas',                                'Dirección',    4,    3),
(6, 'Departamento de Desarrollo',                           'Departamento', 5,    4),
(7, 'División de Infraestructura',                          'División',     5,    4),
(8, 'Área de Soporte Técnico',                              'Área',         7,    5);

-- -----------------------------------------------------------
-- Cargos
-- -----------------------------------------------------------
INSERT INTO Cargos (Id, Nombre, Descripcion) VALUES
(1, 'Rector',                    'Máxima autoridad académica'),
(2, 'Decano',                    'Máxima autoridad de facultad'),
(3, 'Secretario Académico',      'Responsable de la gestión académica'),
(4, 'Director',                  'Responsable de dirección/departamento'),
(5, 'Jefe de División',          'Responsable de división'),
(6, 'Responsable de Área',       'Responsable de área operativa'),
(7, 'Analista Senior',           'Analista con experiencia'),
(8, 'Desarrollador',             'Programador de sistemas'),
(9, 'Soporte Técnico',           'Atención de incidencias');

-- -----------------------------------------------------------
-- Skills
-- -----------------------------------------------------------
INSERT INTO Skills (Id, Nombre, Categoria) VALUES
(1,  'Liderazgo',              'Blanda'),
(2,  'Comunicación',           'Blanda'),
(3,  'Trabajo en equipo',      'Blanda'),
(4,  'SQL Server',             'Técnica'),
(5,  '.NET',                   'Técnica'),
(6,  'MySQL',                  'Técnica'),
(7,  'JavaScript',             'Técnica'),
(8,  'Gestión de proyectos',   'Gerencial'),
(9,  'Presupuestos',           'Gerencial'),
(10, 'Docencia Universitaria', 'Académica'),
(11, 'Administración Pública',  'Gerencial');

-- -----------------------------------------------------------
-- Personas (datos de ejemplo para desarrollo)
-- -----------------------------------------------------------
INSERT INTO Personas (Id, Nombre, Apellido, Email, Legajo, Documento, FechaNacimiento) VALUES
(1, 'Carlos',    'García',     'carlos.garcia@sgvo.edu',     'L-1001', '20123456', '1970-03-15'),
(2, 'María',     'Rodríguez',  'maria.rodriguez@sgvo.edu',   'L-1002', '22123457', '1975-07-22'),
(3, 'Juan',      'López',      'juan.lopez@sgvo.edu',        'L-1003', '23123458', '1980-11-08'),
(4, 'Ana',       'Martínez',   'ana.martinez@sgvo.edu',      'L-1004', '24123459', '1985-01-30'),
(5, 'Pedro',     'Fernández',  'pedro.fernandez@sgvo.edu',   'L-1005', '25123460', '1988-05-18');

-- -----------------------------------------------------------
-- Skills a Personas (ejemplo)
-- -----------------------------------------------------------
INSERT INTO PersonaSkills (PersonaId, SkillId, NivelDominio) VALUES
(1, 1,  4),  -- Carlos: Liderazgo Experto
(1, 2,  3),  -- Carlos: Comunicación Avanzado
(1, 8,  4),  -- Carlos: Gestión de proyectos Experto
(2, 5,  4),  -- María: .NET Experto
(2, 6,  3),  -- María: MySQL Avanzado
(2, 3,  2),  -- María: Trabajo en equipo Intermedio
(3, 4,  3),  -- Juan: SQL Server Avanzado
(3, 7,  3),  -- Juan: JavaScript Avanzado
(4, 10, 4),  -- Ana: Docencia Experto
(4, 2,  3),  -- Ana: Comunicación Avanzado
(5, 6,  2),  -- Pedro: MySQL Intermedio
(5, 5,  2);  -- Pedro: .NET Intermedio

-- -----------------------------------------------------------
-- Skills a Cargos (requerimientos)
-- -----------------------------------------------------------
-- Director
INSERT INTO CargoSkills (CargoId, SkillId, NivelImportancia) VALUES
(4, 1, 1),   -- Liderazgo Crítico
(4, 2, 2),   -- Comunicación Importante
(4, 8, 2),   -- Gestión de proyectos Importante
(4, 11, 3);   -- Adm. Pública Deseable

-- Jefe de División
INSERT INTO CargoSkills (CargoId, SkillId, NivelImportancia) VALUES
(5, 1, 2),   -- Liderazgo Importante
(5, 2, 2),   -- Comunicación Importante
(5, 8, 3);   -- Gestión de proyectos Deseable

-- Desarrollador
INSERT INTO CargoSkills (CargoId, SkillId, NivelImportancia) VALUES
(8, 5, 1),   -- .NET Crítico
(8, 6, 1),   -- MySQL Crítico
(8, 3, 3),   -- Trabajo en equipo Deseable
(8, 7, 4);   -- JavaScript Secundario

-- Analista Senior
INSERT INTO CargoSkills (CargoId, SkillId, NivelImportancia) VALUES
(7, 4, 1),   -- SQL Server Crítico
(7, 5, 2),   -- .NET Importante
(7, 2, 3),   -- Comunicación Deseable
(7, 8, 3);   -- Gestión de proyectos Deseable

-- Responsable de Área
INSERT INTO CargoSkills (CargoId, SkillId, NivelImportancia) VALUES
(6, 1, 2),   -- Liderazgo Importante
(6, 2, 2),   -- Comunicación Importante
(6, 3, 3);   -- Trabajo en equipo Deseable

-- -----------------------------------------------------------
-- Puestos (ocupando la estructura org)
-- -----------------------------------------------------------
INSERT INTO Puestos (Id, Nombre, UnidadOrganizativaId, CargoId, SuperiorId, Codigo) VALUES
(1,  'Rector',                         1,  1, NULL, 'P-001'),
(2,  'Decano de Ingeniería',           2,  2, 1,    'P-002'),
(3,  'Secretario Académico',           4,  3, 1,    'P-003'),
(4,  'Director de Sistemas',           5,  4, 3,    'P-004'),
(5,  'Jefe de Desarrollo',             6,  5, 4,    'P-005'),
(6,  'Jefe de Infraestructura',        7,  5, 4,    'P-006'),
(7,  'Responsable de Soporte',         8,  6, 6,    'P-007'),
(8,  'Analista Senior de Desarrollo',  6,  7, 5,    'P-008'),
(9,  'Desarrollador .NET Senior',      6,  8, 5,    'P-009'),
(10, 'Soporte Técnico',                8,  9, 7,    'P-010');

-- -----------------------------------------------------------
-- Ocupaciones (asignaciones actuales)
-- -----------------------------------------------------------
INSERT INTO Ocupaciones (PersonaId, PuestoId, FechaInicio, TipoOcupacion) VALUES
(1, 4,  '2020-01-01', 'Permanente'),   -- Carlos → Director de Sistemas
(2, 8,  '2021-06-01', 'Permanente'),   -- María → Analista Senior
(3, 9,  '2022-03-15', 'Permanente'),   -- Juan → Desarrollador .NET
(4, 2,  '2019-08-01', 'Permanente'),   -- Ana → Decano de Ingeniería
(5, 10, '2023-01-10', 'Permanente');   -- Pedro → Soporte Técnico

-- -----------------------------------------------------------
-- Vacantes (puestos sin ocupante)
-- -----------------------------------------------------------
INSERT INTO Vacantes (PuestoId, Motivo, Estado, ResponsableId) VALUES
(5, 'Renuncia del titular',   'Abierta',     1),
(7, 'Nuevo puesto creado',    'EnSeleccion', 1);
