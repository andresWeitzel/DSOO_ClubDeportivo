USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - TURNOS NUTRICION
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_turno_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_crear_turno_nutricion(
    IN p_socio_id INT,
    IN p_nutricionista_id INT,
    IN p_fecha DATE,
    IN p_hora TIME,
    IN p_estado VARCHAR(30),
    OUT p_turno_id INT
)
BEGIN
    IF EXISTS (
        SELECT 1
        FROM turnos_nutricion
        WHERE nutricionista_id = p_nutricionista_id
          AND fecha = p_fecha
          AND hora = p_hora
          AND estado <> 'CANCELADO'
    ) THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'El horario ya está ocupado para ese nutricionista.';
    END IF;

    INSERT INTO turnos_nutricion (socio_id, nutricionista_id, fecha, hora, estado)
    VALUES (p_socio_id, p_nutricionista_id, p_fecha, p_hora, p_estado);
    
    SET p_turno_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_turno_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_turno_por_id(
    IN p_id_turno INT
)
BEGIN
    SELECT 
        id_turno,
        socio_id,
        nutricionista_id,
        fecha,
        hora,
        estado
    FROM turnos_nutricion
    WHERE id_turno = p_id_turno;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_turnos_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_turnos_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        t.id_turno,
        t.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        t.nutricionista_id,
        CONCAT(n.nombre, ' ', n.apellido) AS nutricionista_nombre,
        t.fecha,
        t.hora,
        t.estado
    FROM turnos_nutricion t
    INNER JOIN socios s ON t.socio_id = s.id_socio
    INNER JOIN nutricionistas n ON t.nutricionista_id = n.id_nutricionista
    WHERE t.socio_id = p_socio_id
    ORDER BY t.fecha DESC, t.hora DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_turnos_nutricionista;

DELIMITER $$

CREATE PROCEDURE sp_obtener_turnos_nutricionista(
    IN p_nutricionista_id INT
)
BEGIN
    SELECT 
        t.id_turno,
        t.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        t.nutricionista_id,
        CONCAT(n.nombre, ' ', n.apellido) AS nutricionista_nombre,
        t.fecha,
        t.hora,
        t.estado
    FROM turnos_nutricion t
    INNER JOIN socios s ON t.socio_id = s.id_socio
    INNER JOIN nutricionistas n ON t.nutricionista_id = n.id_nutricionista
    WHERE t.nutricionista_id = p_nutricionista_id
    ORDER BY t.fecha ASC, t.hora ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_turnos_por_fecha;

DELIMITER $$

CREATE PROCEDURE sp_obtener_turnos_por_fecha(
    IN p_fecha DATE
)
BEGIN
    SELECT 
        t.id_turno,
        t.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        t.nutricionista_id,
        CONCAT(n.nombre, ' ', n.apellido) AS nutricionista_nombre,
        t.fecha,
        t.hora,
        t.estado
    FROM turnos_nutricion t
    INNER JOIN socios s ON t.socio_id = s.id_socio
    INNER JOIN nutricionistas n ON t.nutricionista_id = n.id_nutricionista
    WHERE t.fecha = p_fecha
    ORDER BY t.hora ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_turnos_disponibles;

DELIMITER $$

CREATE PROCEDURE sp_obtener_turnos_disponibles(
    IN p_fecha DATE,
    IN p_nutricionista_id INT
)
BEGIN
    SELECT 
        t.id_turno,
        t.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        t.fecha,
        t.hora
    FROM turnos_nutricion t
    INNER JOIN socios s ON t.socio_id = s.id_socio
    WHERE t.fecha = p_fecha
        AND t.nutricionista_id = p_nutricionista_id
        AND t.estado = 'DISPONIBLE'
    ORDER BY t.hora ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_horarios_disponibles_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_obtener_horarios_disponibles_nutricion(
    IN p_fecha DATE,
    IN p_nutricionista_id INT
)
BEGIN
    SELECT TIME_FORMAT(h.hora, '%H:%i:%s') AS hora_texto
    FROM (
        SELECT TIME('09:00:00') AS hora UNION ALL
        SELECT TIME('09:30:00') UNION ALL
        SELECT TIME('10:00:00') UNION ALL
        SELECT TIME('10:30:00') UNION ALL
        SELECT TIME('11:00:00') UNION ALL
        SELECT TIME('11:30:00') UNION ALL
        SELECT TIME('14:00:00') UNION ALL
        SELECT TIME('14:30:00') UNION ALL
        SELECT TIME('15:00:00') UNION ALL
        SELECT TIME('15:30:00') UNION ALL
        SELECT TIME('16:00:00') UNION ALL
        SELECT TIME('16:30:00') UNION ALL
        SELECT TIME('17:00:00')
    ) h
    WHERE p_fecha >= CURDATE()
      AND NOT EXISTS (
          SELECT 1
          FROM turnos_nutricion t
          WHERE t.nutricionista_id = p_nutricionista_id
            AND t.fecha = p_fecha
            AND t.hora = h.hora
            AND t.estado <> 'CANCELADO'
      )
    ORDER BY h.hora ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_horarios_ocupados_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_obtener_horarios_ocupados_nutricion(
    IN p_fecha DATE,
    IN p_nutricionista_id INT
)
BEGIN
    SELECT TIME_FORMAT(hora, '%H:%i:%s') AS hora_texto
    FROM turnos_nutricion
    WHERE nutricionista_id = p_nutricionista_id
      AND fecha = p_fecha
      AND estado <> 'CANCELADO'
    ORDER BY hora ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_turno_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_turno_nutricion(
    IN p_id_turno INT,
    IN p_fecha DATE,
    IN p_hora TIME,
    IN p_estado VARCHAR(30)
)
BEGIN
    UPDATE turnos_nutricion
    SET 
        fecha = p_fecha,
        hora = p_hora,
        estado = p_estado
    WHERE id_turno = p_id_turno;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_confirmar_turno_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_confirmar_turno_nutricion(
    IN p_id_turno INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    UPDATE turnos_nutricion
    SET estado = 'CONFIRMADO'
    WHERE id_turno = p_id_turno;
    
    SET p_mensaje = 'Turno confirmado correctamente.';
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_cancelar_turno_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_cancelar_turno_nutricion(
    IN p_id_turno INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    UPDATE turnos_nutricion
    SET estado = 'CANCELADO'
    WHERE id_turno = p_id_turno;
    
    SET p_mensaje = 'Turno cancelado correctamente.';
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_turno_nutricion;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_turno_nutricion(
    IN p_id_turno INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DELETE FROM turnos_nutricion WHERE id_turno = p_id_turno;
    SET p_mensaje = 'Turno eliminado correctamente.';
END$$

DELIMITER ;
