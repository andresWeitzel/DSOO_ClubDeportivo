USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - CARNETS
-- ============================================

DROP PROCEDURE IF EXISTS sp_emitir_carnet;

DELIMITER $$

CREATE PROCEDURE sp_emitir_carnet(
    IN p_socio_id INT,
    IN p_numero VARCHAR(50),
    IN p_fecha_emision DATE,
    IN p_fecha_vencimiento DATE,
    OUT p_carnet_id INT
)
BEGIN
    INSERT INTO carnets (socio_id, numero, fecha_emision, fecha_vencimiento)
    VALUES (p_socio_id, p_numero, p_fecha_emision, p_fecha_vencimiento);
    
    SET p_carnet_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_carnet_por_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_carnet_por_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        id_carnet,
        socio_id,
        numero,
        fecha_emision,
        fecha_vencimiento,
        foto
    FROM carnets
    WHERE socio_id = p_socio_id
    LIMIT 1;
END$$

DELIMITER ;