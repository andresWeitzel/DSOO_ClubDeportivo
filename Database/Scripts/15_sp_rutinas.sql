USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - RUTINAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_rutina;

DELIMITER $$

CREATE PROCEDURE sp_crear_rutina(
    IN p_socio_id INT,
    IN p_profesor_id INT,
    IN p_descripcion TEXT,
    IN p_observaciones TEXT,
    OUT p_rutina_id INT
)
BEGIN
    INSERT INTO rutinas (socio_id, profesor_id, descripcion, observaciones)
    VALUES (p_socio_id, p_profesor_id, p_descripcion, p_observaciones);
    
    SET p_rutina_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_rutina_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_rutina_por_id(
    IN p_id_rutina INT
)
BEGIN
    SELECT 
        id_rutina,
        socio_id,
        profesor_id,
        descripcion,
        fecha_creacion,
        observaciones
    FROM rutinas
    WHERE id_rutina = p_id_rutina;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_rutinas_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_rutinas_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        r.id_rutina,
        r.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        r.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        r.descripcion,
        r.fecha_creacion,
        r.observaciones
    FROM rutinas r
    INNER JOIN socios s ON r.socio_id = s.id_socio
    INNER JOIN profesores p ON r.profesor_id = p.id_profesor
    WHERE r.socio_id = p_socio_id
    ORDER BY r.fecha_creacion DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_rutinas_profesor;

DELIMITER $$

CREATE PROCEDURE sp_obtener_rutinas_profesor(
    IN p_profesor_id INT
)
BEGIN
    SELECT 
        r.id_rutina,
        r.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        r.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        r.descripcion,
        r.fecha_creacion,
        r.observaciones
    FROM rutinas r
    INNER JOIN socios s ON r.socio_id = s.id_socio
    INNER JOIN profesores p ON r.profesor_id = p.id_profesor
    WHERE r.profesor_id = p_profesor_id
    ORDER BY s.nombre, s.apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todas_rutinas;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todas_rutinas()
BEGIN
    SELECT 
        r.id_rutina,
        r.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        r.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        r.descripcion,
        r.fecha_creacion,
        r.observaciones
    FROM rutinas r
    INNER JOIN socios s ON r.socio_id = s.id_socio
    INNER JOIN profesores p ON r.profesor_id = p.id_profesor
    ORDER BY r.fecha_creacion DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_rutina;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_rutina(
    IN p_id_rutina INT,
    IN p_descripcion TEXT,
    IN p_observaciones TEXT
)
BEGIN
    UPDATE rutinas
    SET 
        descripcion = p_descripcion,
        observaciones = p_observaciones
    WHERE id_rutina = p_id_rutina;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_rutina;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_rutina(
    IN p_id_rutina INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DELETE FROM rutinas WHERE id_rutina = p_id_rutina;
    SET p_mensaje = 'Rutina eliminada correctamente.';
END$$

DELIMITER ;
