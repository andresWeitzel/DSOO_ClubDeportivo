USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - FICHAS MEDICAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_ficha_medica;

DELIMITER $$

CREATE PROCEDURE sp_crear_ficha_medica(
    IN p_socio_id INT,
    IN p_peso DECIMAL(5,2),
    IN p_altura DECIMAL(5,2),
    IN p_alergias TEXT,
    IN p_medicacion TEXT,
    IN p_observaciones TEXT,
    IN p_carga_permitida VARCHAR(200),
    OUT p_ficha_id INT
)
BEGIN
    INSERT INTO fichas_medicas (socio_id, peso, altura, alergias, medicacion, observaciones, carga_permitida)
    VALUES (p_socio_id, p_peso, p_altura, p_alergias, p_medicacion, p_observaciones, p_carga_permitida);
    
    SET p_ficha_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_ficha_medica_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_ficha_medica_por_id(
    IN p_id_ficha INT
)
BEGIN
    SELECT 
        id_ficha,
        socio_id,
        peso,
        altura,
        alergias,
        medicacion,
        observaciones,
        carga_permitida
    FROM fichas_medicas
    WHERE id_ficha = p_id_ficha;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_ficha_medica_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_ficha_medica_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        f.id_ficha,
        f.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        f.peso,
        f.altura,
        f.alergias,
        f.medicacion,
        f.observaciones,
        f.carga_permitida
    FROM fichas_medicas f
    INNER JOIN socios s ON f.socio_id = s.id_socio
    WHERE f.socio_id = p_socio_id;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todas_fichas_medicas;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todas_fichas_medicas()
BEGIN
    SELECT 
        f.id_ficha,
        f.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        f.peso,
        f.altura,
        f.alergias,
        f.medicacion,
        f.observaciones,
        f.carga_permitida
    FROM fichas_medicas f
    INNER JOIN socios s ON f.socio_id = s.id_socio
    ORDER BY s.nombre, s.apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_ficha_medica;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_ficha_medica(
    IN p_id_ficha INT,
    IN p_peso DECIMAL(5,2),
    IN p_altura DECIMAL(5,2),
    IN p_alergias TEXT,
    IN p_medicacion TEXT,
    IN p_observaciones TEXT,
    IN p_carga_permitida VARCHAR(200)
)
BEGIN
    UPDATE fichas_medicas
    SET 
        peso = p_peso,
        altura = p_altura,
        alergias = p_alergias,
        medicacion = p_medicacion,
        observaciones = p_observaciones,
        carga_permitida = p_carga_permitida
    WHERE id_ficha = p_id_ficha;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_ficha_medica;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_ficha_medica(
    IN p_id_ficha INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DELETE FROM fichas_medicas WHERE id_ficha = p_id_ficha;
    SET p_mensaje = 'Ficha médica eliminada correctamente.';
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_fichas_por_alergias;

DELIMITER $$

CREATE PROCEDURE sp_obtener_fichas_por_alergias(
    IN p_alergia VARCHAR(100)
)
BEGIN
    SELECT 
        f.id_ficha,
        f.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        f.peso,
        f.altura,
        f.alergias,
        f.medicacion,
        f.observaciones,
        f.carga_permitida
    FROM fichas_medicas f
    INNER JOIN socios s ON f.socio_id = s.id_socio
    WHERE f.alergias LIKE CONCAT('%', p_alergia, '%')
    ORDER BY s.nombre, s.apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_reporte_imc_socios;

DELIMITER $$

CREATE PROCEDURE sp_reporte_imc_socios()
BEGIN
    SELECT 
        f.socio_id,
        CONCAT(s.nombre, ' ', s.apellido) AS socio_nombre,
        f.peso,
        f.altura,
        ROUND((f.peso / (f.altura * f.altura)), 2) AS imc,
        CASE 
            WHEN (f.peso / (f.altura * f.altura)) < 18.5 THEN 'Bajo peso'
            WHEN (f.peso / (f.altura * f.altura)) BETWEEN 18.5 AND 24.9 THEN 'Peso normal'
            WHEN (f.peso / (f.altura * f.altura)) BETWEEN 25 AND 29.9 THEN 'Sobrepeso'
            WHEN (f.peso / (f.altura * f.altura)) >= 30 THEN 'Obesidad'
        END AS categoria_imc,
        f.carga_permitida
    FROM fichas_medicas f
    INNER JOIN socios s ON f.socio_id = s.id_socio
    ORDER BY imc DESC;
END$$

DELIMITER ;
