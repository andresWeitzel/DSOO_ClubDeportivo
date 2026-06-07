USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - NUTRICIONISTAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_nutricionista;

DELIMITER $$

CREATE PROCEDURE sp_crear_nutricionista(
    IN p_dni VARCHAR(20),
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_telefono VARCHAR(50),
    IN p_email VARCHAR(150),
    IN p_matricula VARCHAR(100),
    OUT p_nutricionista_id INT
)
BEGIN
    INSERT INTO nutricionistas (dni, nombre, apellido, telefono, email, matricula)
    VALUES (p_dni, p_nombre, p_apellido, p_telefono, p_email, p_matricula);
    
    SET p_nutricionista_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_nutricionista_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_nutricionista_por_id(
    IN p_id_nutricionista INT
)
BEGIN
    SELECT 
        id_nutricionista,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        matricula
    FROM nutricionistas
    WHERE id_nutricionista = p_id_nutricionista;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todos_nutricionistas;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todos_nutricionistas()
BEGIN
    SELECT 
        id_nutricionista,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        matricula
    FROM nutricionistas
    ORDER BY nombre, apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_nutricionista;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_nutricionista(
    IN p_id_nutricionista INT,
    IN p_nombre VARCHAR(100),
    IN p_apellido VARCHAR(100),
    IN p_telefono VARCHAR(50),
    IN p_email VARCHAR(150),
    IN p_matricula VARCHAR(100)
)
BEGIN
    UPDATE nutricionistas
    SET 
        nombre = p_nombre,
        apellido = p_apellido,
        telefono = p_telefono,
        email = p_email,
        matricula = p_matricula
    WHERE id_nutricionista = p_id_nutricionista;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_nutricionista;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_nutricionista(
    IN p_id_nutricionista INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DECLARE v_tiene_turnos INT;
    
    SELECT COUNT(*) INTO v_tiene_turnos 
    FROM turnos_nutricion WHERE nutricionista_id = p_id_nutricionista;
    
    IF v_tiene_turnos > 0 THEN
        SET p_mensaje = 'No se puede eliminar. El nutricionista tiene turnos asociados.';
    ELSE
        DELETE FROM nutricionistas WHERE id_nutricionista = p_id_nutricionista;
        SET p_mensaje = 'Nutricionista eliminado correctamente.';
    END IF;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_buscar_nutricionistas;

DELIMITER $$

CREATE PROCEDURE sp_buscar_nutricionistas(
    IN p_busqueda VARCHAR(100)
)
BEGIN
    SELECT 
        id_nutricionista,
        dni,
        nombre,
        apellido,
        telefono,
        email,
        matricula
    FROM nutricionistas
    WHERE nombre LIKE CONCAT('%', p_busqueda, '%')
        OR apellido LIKE CONCAT('%', p_busqueda, '%')
        OR dni LIKE CONCAT('%', p_busqueda, '%')
        OR matricula LIKE CONCAT('%', p_busqueda, '%')
    ORDER BY nombre, apellido;
END$$

DELIMITER ;
