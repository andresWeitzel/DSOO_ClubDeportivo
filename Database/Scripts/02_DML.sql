USE db_club_deportivo;

-- ============================================
-- INSERTAR ROLES
-- ============================================
INSERT INTO roles (RolUsu, NomRol) VALUES
(120, 'Administrador'),
(121, 'Empleado'),
(122, 'Profesor'),
(123, 'Nutricionista'),
(124, 'Socio'),
(125, 'Visitante');

-- ============================================
-- INSERTAR USUARIOS
-- ============================================
INSERT INTO usuario (NombreUsu, PassUsu, RolUsu) VALUES
('Mari2023', '123456', 120),
('admin', '1234', 120),
('juan_prof', 'prof123', 122),
('maria_nutri', 'nutri123', 123),
('empleado1', 'emp123', 121);

-- ============================================
-- INSERTAR SOCIOS
-- ============================================
INSERT INTO socios (dni, nombre, apellido, telefono, direccion, email, estado_cuota) VALUES
('12345678', 'Carlos', 'García', '1123456789', 'Av. Principal 100', 'carlos.garcia@email.com', 'AL_DIA'),
('87654321', 'María', 'López', '1198765432', 'Calle 45 200', 'maria.lopez@email.com', 'AL_DIA'),
('11111111', 'Juan', 'Martínez', '1111111111', 'Avenida Central 500', 'juan.martinez@email.com', 'AL_DIA'),
('22222222', 'Ana', 'Rodríguez', '2222222222', 'Calle B 300', 'ana.rodriguez@email.com', 'MORA'),
('33333333', 'Pedro', 'Sánchez', '3333333333', 'Pasaje 150', 'pedro.sanchez@email.com', 'AL_DIA'),
('44444444', 'Laura', 'Fernández', '4444444444', 'Ruta 7 km 10', 'laura.fernandez@email.com', 'AL_DIA'),
('55555555', 'Roberto', 'González', '5555555555', 'Calle 88 100', 'roberto.gonzalez@email.com', 'MORA'),
('66666666', 'Sofía', 'Torres', '6666666666', 'Av. Libertad 250', 'sofia.torres@email.com', 'AL_DIA'),
('77777777', 'Miguel', 'Ramírez', '7777777777', 'Callejón del Sur', 'miguel.ramirez@email.com', 'AL_DIA'),
('88888888', 'Claudia', 'Moreno', '8888888888', 'Plaza Mayor 10', 'claudia.moreno@email.com', 'AL_DIA');

-- ============================================
-- INSERTAR VISITANTES
-- ============================================
INSERT INTO visitantes (dni, nombre, apellido, telefono, actividad, pago_diario_monto) VALUES
('99999999', 'Andrés', 'Pérez', '9999999999', 'Musculación', 50.00),
('10101010', 'Beatriz', 'Díaz', '1010101010', 'Pilates', 40.00),
('20202020', 'Diego', 'Ruiz', '2020202020', 'Natación', 45.00),
('30303030', 'Elena', 'Vega', '3030303030', 'Yoga', 35.00),
('40404040', 'Felipe', 'Castro', '4040404040', 'Spinning', 55.00);

-- ============================================
-- INSERTAR PROFESORES
-- ============================================
INSERT INTO profesores (dni, nombre, apellido, telefono, email, especialidad, sueldo_base) VALUES
('50505050', 'Juan', 'Entrenador', '5050505050', 'juan.trainer@email.com', 'Musculación', 5000.00),
('60606060', 'Patricia', 'Instructora', '6060606060', 'patricia.instructor@email.com', 'Pilates', 4500.00),
('70707070', 'Marcos', 'Coach', '7070707070', 'marcos.coach@email.com', 'Spinning', 4800.00),
('80808080', 'Verónica', 'Entrenadora', '8080808080', 'veronica.trainer@email.com', 'Yoga', 4200.00);

-- ============================================
-- INSERTAR CARNETS
-- ============================================
INSERT INTO carnets (socio_id, numero, fecha_emision, fecha_vencimiento) VALUES
(1, 'CARNET-001', '2024-01-15', '2025-01-15'),
(2, 'CARNET-002', '2024-02-20', '2025-02-20'),
(3, 'CARNET-003', '2024-03-10', '2025-03-10'),
(4, 'CARNET-004', '2024-04-05', '2025-04-05'),
(5, 'CARNET-005', '2024-05-12', '2025-05-12'),
(6, 'CARNET-006', '2024-06-18', '2025-06-18'),
(7, 'CARNET-007', '2024-07-22', '2025-07-22'),
(8, 'CARNET-008', '2024-08-30', '2025-08-30'),
(9, 'CARNET-009', '2024-09-14', '2025-09-14'),
(10, 'CARNET-010', '2024-10-08', '2025-10-08');

-- ============================================
-- INSERTAR CUOTAS
-- ============================================
INSERT INTO cuotas (socio_id, monto, fecha_emision, fecha_vencimiento, estado, en_mora) VALUES
(1, 150.00, '2024-12-01', '2024-12-31', 'PAGADA', FALSE),
(1, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE),
(1, 150.00, '2025-02-01', '2025-02-28', 'AL_DIA', FALSE),
(2, 150.00, '2024-12-01', '2024-12-31', 'PAGADA', FALSE),
(2, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE),
(2, 150.00, '2025-02-01', '2025-02-28', 'AL_DIA', FALSE),
(3, 150.00, '2024-12-01', '2024-12-31', 'PAGADA', FALSE),
(3, 150.00, '2025-01-01', '2025-01-31', 'AL_DIA', FALSE),
(4, 150.00, '2024-11-01', '2024-11-30', 'VENCIDA', TRUE),
(4, 150.00, '2024-12-01', '2024-12-31', 'VENCIDA', TRUE),
(4, 150.00, '2025-01-01', '2025-01-31', 'AL_DIA', TRUE),
(5, 150.00, '2024-12-01', '2024-12-31', 'PAGADA', FALSE),
(5, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE),
(6, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE),
(6, 150.00, '2025-02-01', '2025-02-28', 'AL_DIA', FALSE),
(7, 150.00, '2024-10-01', '2024-10-31', 'VENCIDA', TRUE),
(7, 150.00, '2024-11-01', '2024-11-30', 'VENCIDA', TRUE),
(8, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE),
(8, 150.00, '2025-02-01', '2025-02-28', 'AL_DIA', FALSE),
(9, 150.00, '2025-01-01', '2025-01-31', 'AL_DIA', FALSE),
(10, 150.00, '2025-01-01', '2025-01-31', 'PAGADA', FALSE);

-- ============================================
-- INSERTAR PAGOS SOCIOS
-- ============================================
INSERT INTO pagos (tipo, socio_id, cuota_id, monto, fecha_pago, medio_pago, concepto) VALUES
('SOCIO', 1, 1, 150.00, '2024-12-05', 'Tarjeta Débito', 'Cuota Diciembre 2024'),
('SOCIO', 1, 2, 150.00, '2025-01-03', 'Tarjeta Débito', 'Cuota Enero 2025'),
('SOCIO', 2, 4, 150.00, '2024-12-10', 'Efectivo', 'Cuota Diciembre 2024'),
('SOCIO', 2, 5, 150.00, '2025-01-08', 'Transferencia', 'Cuota Enero 2025'),
('SOCIO', 3, 7, 150.00, '2024-12-15', 'Tarjeta Crédito', 'Cuota Diciembre 2024'),
('SOCIO', 5, 12, 150.00, '2024-12-20', 'Efectivo', 'Cuota Diciembre 2024'),
('SOCIO', 5, 13, 150.00, '2025-01-15', 'Tarjeta Débito', 'Cuota Enero 2025'),
('SOCIO', 6, 14, 150.00, '2025-01-10', 'Transferencia', 'Cuota Enero 2025'),
('SOCIO', 8, 18, 150.00, '2025-01-12', 'Efectivo', 'Cuota Enero 2025'),
('SOCIO', 10, 21, 150.00, '2025-01-20', 'Tarjeta Débito', 'Cuota Enero 2025');

-- ============================================
-- INSERTAR PAGOS VISITANTES
-- ============================================
INSERT INTO pagos (tipo, visitante_id, monto, fecha_pago, medio_pago, concepto) VALUES
('VISITANTE', 1, 50.00, '2025-02-01 09:30:00', 'Efectivo', 'Entrada Diaria Musculación'),
('VISITANTE', 2, 40.00, '2025-02-01 10:15:00', 'Tarjeta Débito', 'Entrada Diaria Pilates'),
('VISITANTE', 3, 45.00, '2025-02-01 15:45:00', 'Efectivo', 'Entrada Diaria Natación'),
('VISITANTE', 1, 50.00, '2025-02-02 09:00:00', 'Efectivo', 'Entrada Diaria Musculación'),
('VISITANTE', 4, 35.00, '2025-02-02 18:00:00', 'Tarjeta Débito', 'Entrada Diaria Yoga'),
('VISITANTE', 5, 55.00, '2025-02-02 19:30:00', 'Efectivo', 'Entrada Diaria Spinning');

-- ============================================
-- INSERTAR FICHAS MEDICAS
-- ============================================
INSERT INTO fichas_medicas (socio_id, peso, altura, alergias, medicacion, observaciones, carga_permitida) VALUES
(1, 75.50, 1.75, 'Penicilina', 'Ninguna', 'Buena salud general', 'Sin restricciones'),
(2, 62.00, 1.62, 'Ninguna', 'Vitaminas B12', 'Histórico de lumbalgia', 'Evitar pesos superiores a 50kg en espalda'),
(3, 88.00, 1.82, 'Ninguna', 'Ninguna', 'Deportista activo', 'Sin restricciones'),
(4, 58.00, 1.60, 'Ibuprofeno', 'Levotiroxina', 'Hipotiroidismo controlado', 'Sin restricciones'),
(5, 72.00, 1.78, 'Ninguna', 'Ninguna', 'Excelente estado físico', 'Sin restricciones'),
(6, 65.00, 1.68, 'Ninguna', 'Antihistamínicos', 'Alergia estacional', 'Sin restricciones'),
(8, 95.00, 1.85, 'Ninguna', 'Ninguna', 'Sobrepeso moderado', 'Entrenamiento controlado'),
(9, 70.00, 1.75, 'Ninguna', 'Ninguna', 'Buen estado físico', 'Sin restricciones');

-- ============================================
-- INSERTAR NUTRICIONISTAS
-- ============================================
INSERT INTO nutricionistas (dni, nombre, apellido, telefono, email, matricula) VALUES
('90909090', 'Daniela', 'Salas', '9090909090', 'daniela.salas@email.com', 'MAT-NUTRI-001'),
('91919191', 'Rodrigo', 'Vargas', '9191919191', 'rodrigo.vargas@email.com', 'MAT-NUTRI-002');

-- ============================================
-- INSERTAR HORARIOS CLASE
-- ============================================
INSERT INTO horarios_clase (profesor_id, dia_semana, hora_inicio, hora_fin, actividad) VALUES
(1, 'Lunes', '09:00:00', '10:00:00', 'Musculación Básica'),
(1, 'Miércoles', '16:00:00', '17:00:00', 'Musculación Avanzada'),
(1, 'Viernes', '18:00:00', '19:00:00', 'Musculación Intermedia'),
(2, 'Martes', '10:30:00', '11:30:00', 'Pilates Mat'),
(2, 'Jueves', '19:00:00', '20:00:00', 'Pilates Reformer'),
(3, 'Lunes', '19:30:00', '20:30:00', 'Spinning Intenso'),
(3, 'Miércoles', '20:00:00', '21:00:00', 'Spinning Moderado'),
(4, 'Martes', '08:00:00', '09:00:00', 'Yoga Matutino'),
(4, 'Jueves', '18:30:00', '19:30:00', 'Yoga Vespertino');

-- ============================================
-- INSERTAR RUTINAS
-- ============================================
INSERT INTO rutinas (socio_id, profesor_id, descripcion, observaciones) VALUES
(1, 1, 'Rutina hipertrofia: Press banca, Sentadillas, Peso muerto, Remo', 'Realizar 4 series de 8-10 repeticiones por ejercicio'),
(2, 2, 'Rutina Pilates: Tabla, Puente, Círculos con piernas', 'Enfoque en core, 3 series de 12-15 repeticiones'),
(3, 1, 'Rutina fuerza: Sentadillas profundas, Peso muerto, Press militar', 'Aumentar carga cada semana un 2.5%'),
(5, 1, 'Rutina principiante: Máquinas guiadas, Cardio ligero', 'Familiarización con equipos, 3 series de 10 repeticiones'),
(6, 2, 'Rutina Pilates avanzada: Ejercicios de flexibilidad', 'Flexibilidad y tonificación, 2-3 series');

-- ============================================
-- INSERTAR ASISTENCIAS PROFESORES
-- ============================================
INSERT INTO asistencias (profesor_id, fecha, presente, firma) VALUES
(1, '2025-02-01', TRUE, 'Firma Juan Entrenador'),
(1, '2025-02-03', TRUE, 'Firma Juan Entrenador'),
(1, '2025-02-05', TRUE, 'Firma Juan Entrenador'),
(2, '2025-02-02', TRUE, 'Firma Patricia Instructora'),
(2, '2025-02-04', TRUE, 'Firma Patricia Instructora'),
(3, '2025-02-01', FALSE, NULL),
(3, '2025-02-03', TRUE, 'Firma Marcos Coach'),
(4, '2025-02-02', TRUE, 'Firma Verónica Entrenadora');

-- ============================================
-- INSERTAR TURNOS NUTRICION
-- ============================================
INSERT INTO turnos_nutricion (socio_id, nutricionista_id, fecha, hora, estado) VALUES
(1, 1, '2025-02-10', '10:00:00', 'CONFIRMADO'),
(2, 2, '2025-02-12', '14:30:00', 'DISPONIBLE'),
(3, 1, '2025-02-15', '11:00:00', 'CONFIRMADO'),
(5, 2, '2025-02-18', '15:00:00', 'DISPONIBLE'),
(6, 1, '2025-02-20', '09:30:00', 'CANCELADO');

-- ============================================
-- INSERTAR LIQUIDACIONES
-- ============================================
INSERT INTO liquidaciones (profesor_id, mes, anio, monto_bruto, descuentos, monto_neto, fecha_pago, estado) VALUES
(1, 1, 2025, 5000.00, 500.00, 4500.00, '2025-02-01', 'PAGADO'),
(2, 1, 2025, 4500.00, 450.00, 4050.00, '2025-02-01', 'PAGADO'),
(3, 1, 2025, 4800.00, 480.00, 4320.00, NULL, 'PENDIENTE'),
(4, 1, 2025, 4200.00, 420.00, 3780.00, NULL, 'PENDIENTE');


