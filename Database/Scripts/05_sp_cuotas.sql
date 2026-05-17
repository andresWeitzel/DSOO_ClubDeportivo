USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - CUOTAS
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_cuota;

DELIMITER $$

CREATE PROCEDURE sp_crear_cuota(
    IN p_socio_id INT,
    IN p_monto DECIMAL(10,2),
    IN p_fecha_vencimiento DATE,
    OUT p_cuota_id INT
)
BEGIN
    INSERT INTO cuotas (socio_id, monto, fecha_emision, fecha_vencimiento, estado, en_mora)
    VALUES (p_socio_id, p_monto, CURDATE(), p_fecha_vencimiento, 'AL_DIA', FALSE);
    
    SET p_cuota_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_cuotas_por_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_cuotas_por_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        id_cuota,
        socio_id,
        monto,
        fecha_emision,
        fecha_vencimiento,
        estado,
        en_mora
    FROM cuotas
    WHERE socio_id = p_socio_id
    ORDER BY fecha_vencimiento DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_ultima_cuota_socio;

DELIMITER $$

CREATE PROCEDURE sp_obtener_ultima_cuota_socio(
    IN p_socio_id INT
)
BEGIN
    SELECT 
        id_cuota,
        socio_id,
        monto,
        fecha_emision,
        fecha_vencimiento,
        estado,
        en_mora
    FROM cuotas
    WHERE socio_id = p_socio_id
    ORDER BY fecha_vencimiento DESC
    LIMIT 1;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_estado_cuota_mora;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_estado_cuota_mora(
    IN p_id_cuota INT,
    IN p_estado VARCHAR(50),
    IN p_en_mora BOOLEAN
)
BEGIN
    UPDATE cuotas 
    SET estado = p_estado, en_mora = p_en_mora
    WHERE id_cuota = p_id_cuota;
END$$

DELIMITER ;