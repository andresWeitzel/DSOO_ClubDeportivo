USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - HORARIOS DE ACTIVIDAD
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_horario_actividad;

DELIMITER $$

CREATE PROCEDURE sp_crear_horario_actividad(
    IN p_profesor_id INT,
    IN p_dia_semana VARCHAR(30),
    IN p_hora_inicio TIME,
    IN p_hora_fin TIME,
    IN p_actividad VARCHAR(150),
    OUT p_horario_id INT
)
BEGIN
    INSERT INTO horarios_actividad (profesor_id, dia_semana, hora_inicio, hora_fin, actividad)
    VALUES (p_profesor_id, p_dia_semana, p_hora_inicio, p_hora_fin, p_actividad);
    
    SET p_horario_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_horario_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_horario_por_id(
    IN p_id_horario INT
)
BEGIN
    SELECT 
        id_horario,
        profesor_id,
        dia_semana,
        hora_inicio,
        hora_fin,
        actividad
    FROM horarios_actividad
    WHERE id_horario = p_id_horario;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_horarios_profesor;

DELIMITER $$

CREATE PROCEDURE sp_obtener_horarios_profesor(
    IN p_profesor_id INT
)
BEGIN
    SELECT 
        id_horario,
        profesor_id,
        dia_semana,
        hora_inicio,
        hora_fin,
        actividad
    FROM horarios_actividad
    WHERE profesor_id = p_profesor_id
    ORDER BY 
        CASE 
            WHEN dia_semana = 'Lunes' THEN 1
            WHEN dia_semana = 'Martes' THEN 2
            WHEN dia_semana = 'Miércoles' THEN 3
            WHEN dia_semana = 'Jueves' THEN 4
            WHEN dia_semana = 'Viernes' THEN 5
            WHEN dia_semana = 'Sábado' THEN 6
            WHEN dia_semana = 'Domingo' THEN 7
        END,
        hora_inicio;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_horarios_por_dia;

DELIMITER $$

CREATE PROCEDURE sp_obtener_horarios_por_dia(
    IN p_dia_semana VARCHAR(30)
)
BEGIN
    SELECT 
        h.id_horario,
        h.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        h.dia_semana,
        h.hora_inicio,
        h.hora_fin,
        h.actividad
    FROM horarios_actividad h
    INNER JOIN profesores p ON h.profesor_id = p.id_profesor
    WHERE h.dia_semana = p_dia_semana
    ORDER BY h.hora_inicio;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todos_horarios;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todos_horarios()
BEGIN
    SELECT 
        h.id_horario,
        h.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        h.dia_semana,
        h.hora_inicio,
        h.hora_fin,
        h.actividad
    FROM horarios_actividad h
    INNER JOIN profesores p ON h.profesor_id = p.id_profesor
    ORDER BY 
        CASE 
            WHEN h.dia_semana = 'Lunes' THEN 1
            WHEN h.dia_semana = 'Martes' THEN 2
            WHEN h.dia_semana = 'Miércoles' THEN 3
            WHEN h.dia_semana = 'Jueves' THEN 4
            WHEN h.dia_semana = 'Viernes' THEN 5
            WHEN h.dia_semana = 'Sábado' THEN 6
            WHEN h.dia_semana = 'Domingo' THEN 7
        END,
        h.hora_inicio;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_horario_actividad;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_horario_actividad(
    IN p_id_horario INT,
    IN p_dia_semana VARCHAR(30),
    IN p_hora_inicio TIME,
    IN p_hora_fin TIME,
    IN p_actividad VARCHAR(150)
)
BEGIN
    UPDATE horarios_actividad
    SET 
        dia_semana = p_dia_semana,
        hora_inicio = p_hora_inicio,
        hora_fin = p_hora_fin,
        actividad = p_actividad
    WHERE id_horario = p_id_horario;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_horario_actividad;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_horario_actividad(
    IN p_id_horario INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DELETE FROM horarios_actividad WHERE id_horario = p_id_horario;
    SET p_mensaje = 'Horario de actividad eliminado correctamente.';
END$$

DELIMITER ;
