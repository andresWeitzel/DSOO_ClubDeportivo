USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - ASISTENCIAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_registrar_asistencia;

DELIMITER $$

CREATE PROCEDURE sp_registrar_asistencia(
    IN p_profesor_id INT,
    IN p_fecha DATE,
    IN p_presente BOOLEAN,
    IN p_firma VARCHAR(150),
    OUT p_asistencia_id INT
)
BEGIN
    INSERT INTO asistencias (profesor_id, fecha, presente, firma)
    VALUES (p_profesor_id, p_fecha, p_presente, p_firma);
    
    SET p_asistencia_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_asistencia_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_asistencia_por_id(
    IN p_id_asistencia INT
)
BEGIN
    SELECT 
        id_asistencia,
        profesor_id,
        fecha,
        presente,
        firma
    FROM asistencias
    WHERE id_asistencia = p_id_asistencia;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_asistencias_profesor;

DELIMITER $$

CREATE PROCEDURE sp_obtener_asistencias_profesor(
    IN p_profesor_id INT
)
BEGIN
    SELECT 
        id_asistencia,
        profesor_id,
        fecha,
        presente,
        firma
    FROM asistencias
    WHERE profesor_id = p_profesor_id
    ORDER BY fecha DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_asistencias_por_fecha;

DELIMITER $$

CREATE PROCEDURE sp_obtener_asistencias_por_fecha(
    IN p_fecha DATE
)
BEGIN
    SELECT 
        a.id_asistencia,
        a.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        a.fecha,
        a.presente,
        a.firma
    FROM asistencias a
    INNER JOIN profesores p ON a.profesor_id = p.id_profesor
    WHERE a.fecha = p_fecha
    ORDER BY p.nombre, p.apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_asistencias_rango;

DELIMITER $$

CREATE PROCEDURE sp_obtener_asistencias_rango(
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    SELECT 
        a.id_asistencia,
        a.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        a.fecha,
        a.presente,
        a.firma
    FROM asistencias a
    INNER JOIN profesores p ON a.profesor_id = p.id_profesor
    WHERE a.fecha BETWEEN p_fecha_inicio AND p_fecha_fin
    ORDER BY a.fecha DESC, p.nombre;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_asistencia;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_asistencia(
    IN p_id_asistencia INT,
    IN p_presente BOOLEAN,
    IN p_firma VARCHAR(150)
)
BEGIN
    UPDATE asistencias
    SET 
        presente = p_presente,
        firma = p_firma
    WHERE id_asistencia = p_id_asistencia;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_asistencia;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_asistencia(
    IN p_id_asistencia INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DELETE FROM asistencias WHERE id_asistencia = p_id_asistencia;
    SET p_mensaje = 'Asistencia eliminada correctamente.';
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_reporte_asistencias_profesor;

DELIMITER $$

CREATE PROCEDURE sp_reporte_asistencias_profesor(
    IN p_profesor_id INT,
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    SELECT 
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        COUNT(*) as total_registros,
        SUM(CASE WHEN a.presente = TRUE THEN 1 ELSE 0 END) as asistencias,
        SUM(CASE WHEN a.presente = FALSE THEN 1 ELSE 0 END) as inasistencias,
        ROUND(SUM(CASE WHEN a.presente = TRUE THEN 1 ELSE 0 END) / COUNT(*) * 100, 2) as porcentaje_asistencia
    FROM asistencias a
    INNER JOIN profesores p ON a.profesor_id = p.id_profesor
    WHERE a.profesor_id = p_profesor_id
        AND a.fecha BETWEEN p_fecha_inicio AND p_fecha_fin
    GROUP BY a.profesor_id;
END$$

DELIMITER ;
