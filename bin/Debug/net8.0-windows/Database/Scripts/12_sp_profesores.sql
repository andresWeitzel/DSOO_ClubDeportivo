USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - PROFESORES
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_profesor;

DELIMITER $$

CREATE PROCEDURE sp_crear_profesor(
    IN p_dni VARCHAR(20),
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_telefono VARCHAR(50),
    IN p_email VARCHAR(150),
    IN p_especialidad VARCHAR(100),
    IN p_sueldo_base DECIMAL(12,2),
    OUT p_profesor_id INT
)
BEGIN
    INSERT INTO profesores (dni, nombre, apellido, telefono, email, especialidad, sueldo_base)
    VALUES (p_dni, p_nombre, p_apellido, p_telefono, p_email, p_especialidad, p_sueldo_base);
    
    SET p_profesor_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_profesor_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_profesor_por_id(
    IN p_id_profesor INT
)
BEGIN
    SELECT 
        id_profesor,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        especialidad,
        sueldo_base
    FROM profesores
    WHERE id_profesor = p_id_profesor;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todos_profesores;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todos_profesores()
BEGIN
    SELECT 
        id_profesor,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        especialidad,
        sueldo_base
    FROM profesores
    ORDER BY nombre, apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_profesores_por_especialidad;

DELIMITER $$

CREATE PROCEDURE sp_obtener_profesores_por_especialidad(
    IN p_especialidad VARCHAR(100)
)
BEGIN
    SELECT 
        id_profesor,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        especialidad,
        sueldo_base
    FROM profesores
    WHERE especialidad LIKE CONCAT('%', p_especialidad, '%')
    ORDER BY nombre, apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_profesor;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_profesor(
    IN p_id_profesor INT,
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_telefono VARCHAR(50),
    IN p_email VARCHAR(150),
    IN p_especialidad VARCHAR(100),
    IN p_sueldo_base DECIMAL(12,2)
)
BEGIN
    UPDATE profesores
    SET 
        nombre = p_nombre,
        apellido = p_apellido,
        telefono = p_telefono,
        email = p_email,
        especialidad = p_especialidad,
        sueldo_base = p_sueldo_base
    WHERE id_profesor = p_id_profesor;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_profesor;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_profesor(
    IN p_id_profesor INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DECLARE v_tiene_rutinas INT;
    DECLARE v_tiene_horarios INT;
    DECLARE v_tiene_asistencias INT;
    DECLARE v_tiene_liquidaciones INT;
    
    SELECT COUNT(*) INTO v_tiene_rutinas FROM rutinas WHERE profesor_id = p_id_profesor;
    SELECT COUNT(*) INTO v_tiene_horarios FROM horarios_actividad WHERE profesor_id = p_id_profesor;
    SELECT COUNT(*) INTO v_tiene_asistencias FROM asistencias WHERE profesor_id = p_id_profesor;
    SELECT COUNT(*) INTO v_tiene_liquidaciones FROM liquidaciones WHERE profesor_id = p_id_profesor;
    
    IF v_tiene_rutinas > 0 OR v_tiene_horarios > 0 OR v_tiene_asistencias > 0 OR v_tiene_liquidaciones > 0 THEN
        SET p_mensaje = 'No se puede eliminar. El profesor tiene registros asociados.';
    ELSE
        DELETE FROM profesores WHERE id_profesor = p_id_profesor;
        SET p_mensaje = 'Profesor eliminado correctamente.';
    END IF;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_buscar_profesores;

DELIMITER $$

CREATE PROCEDURE sp_buscar_profesores(
    IN p_busqueda VARCHAR(100)
)
BEGIN
    SELECT 
        id_profesor,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        especialidad,
        sueldo_base
    FROM profesores
    WHERE nombre LIKE CONCAT('%', p_busqueda, '%')
        OR apellido LIKE CONCAT('%', p_busqueda, '%')
        OR dni LIKE CONCAT('%', p_busqueda, '%')
        OR especialidad LIKE CONCAT('%', p_busqueda, '%')
    ORDER BY nombre, apellido;
END$$

DELIMITER ;
