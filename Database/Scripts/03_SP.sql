USE db_club_deportivo;


-- ============================================
-- STORED PROCEDURES - FICHAS MEDICAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_ficha_medica;

DELIMITER $$

CREATE PROCEDURE sp_crear_ficha_medica(
    IN p_socio_id INT
)
BEGIN
    INSERT INTO fichas_medicas (socio_id) VALUES (p_socio_id);
END$$

DELIMITER ;

-- ============================================
-- STORED PROCEDURES - REPORTES
-- ============================================

DROP PROCEDURE IF EXISTS sp_cuotas_por_vencer;

DELIMITER $$

CREATE PROCEDURE sp_cuotas_por_vencer(
    IN p_dias INT
)
BEGIN
    SELECT 
        s.id_socio,
        s.dni,
        s.nombre,
        s.apellido,
        c.id_cuota,
        c.monto,
        c.fecha_vencimiento,
        DATEDIFF(c.fecha_vencimiento, CURDATE()) as dias_para_vencer
    FROM socios s
    INNER JOIN cuotas c ON s.id_socio = c.socio_id
    WHERE c.fecha_vencimiento BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL p_dias DAY)
    AND c.estado != 'PAGADA'
    ORDER BY c.fecha_vencimiento ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_cuotas_vencidas;

DELIMITER $$

CREATE PROCEDURE sp_cuotas_vencidas()
BEGIN
    SELECT 
        s.id_socio,
        s.dni,
        s.nombre,
        s.apellido,
        c.id_cuota,
        c.monto,
        c.fecha_vencimiento,
        DATEDIFF(CURDATE(), c.fecha_vencimiento) as dias_vencidos
    FROM socios s
    INNER JOIN cuotas c ON s.id_socio = c.socio_id
    WHERE c.fecha_vencimiento < CURDATE()
    AND c.estado != 'PAGADA'
    ORDER BY c.fecha_vencimiento ASC;
END$$

DELIMITER ;



-- ============================================
-- STORED PROCEDURES - VISITANTES
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_visitante;

DELIMITER $$

CREATE PROCEDURE sp_crear_visitante(
    IN p_dni VARCHAR(20),
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_telefono VARCHAR(50),
    IN p_actividad VARCHAR(100),
    IN p_pago_diario_monto DECIMAL(10,2),
    OUT p_visitante_id INT
)
BEGIN
    INSERT INTO visitantes (dni, nombre, apellido, telefono, actividad, fecha_ingreso, pago_diario_monto)
    VALUES (p_dni, p_nombre, p_apellido, p_telefono, p_actividad, NOW(), p_pago_diario_monto);
    
    SET p_visitante_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_visitante_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_visitante_por_id(
    IN p_id_visitante INT
)
BEGIN
    SELECT 
        id_visitante,
        dni,
        nombre,
        apellido,
        telefono,
        actividad,
        fecha_ingreso,
        pago_diario_monto
    FROM visitantes
    WHERE id_visitante = p_id_visitante
    LIMIT 1;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_visitantes;

DELIMITER $$

CREATE PROCEDURE sp_obtener_visitantes()
BEGIN
    SELECT 
        id_visitante,
        dni,
        nombre,
        apellido,
        telefono,
        actividad,
        fecha_ingreso,
        pago_diario_monto
    FROM visitantes
    ORDER BY fecha_ingreso DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_usuario_por_username;

DELIMITER $$

CREATE PROCEDURE sp_obtener_usuario_por_username(
    IN p_username VARCHAR(50)
)
BEGIN
    SELECT
        u.CodUsu AS id_usuario,
        u.NombreUsu AS username,
        u.PassUsu AS password,
        r.NomRol AS rol,
        u.FechaRegistro AS fecha_registro
    FROM usuario u
    INNER JOIN roles r ON u.RolUsu = r.RolUsu
    WHERE u.NombreUsu = p_username
    LIMIT 1;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS IngresoLogin;

DELIMITER $$

CREATE PROCEDURE IngresoLogin(
    IN Usu VARCHAR(20),
    IN Pass VARCHAR(15)
)
BEGIN
    SELECT
        u.CodUsu AS id_usuario,
        u.NombreUsu AS username,
        u.PassUsu AS password,
        r.NomRol AS rol,
        u.FechaRegistro AS fecha_registro
    FROM usuario u
    INNER JOIN roles r ON u.RolUsu = r.RolUsu
    WHERE u.NombreUsu = Usu
      AND u.PassUsu = Pass
      AND u.Activo = 1;
END$$

DELIMITER ;
