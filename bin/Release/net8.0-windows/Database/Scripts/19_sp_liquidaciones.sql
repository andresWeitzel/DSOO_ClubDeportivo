USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - LIQUIDACIONES
-- ============================================

DROP PROCEDURE IF EXISTS sp_crear_liquidacion;

DELIMITER $$

CREATE PROCEDURE sp_crear_liquidacion(
    IN p_profesor_id INT,
    IN p_mes INT,
    IN p_anio INT,
    IN p_monto_bruto DECIMAL(12,2),
    IN p_descuentos DECIMAL(12,2),
    IN p_monto_neto DECIMAL(12,2),
    IN p_estado VARCHAR(30),
    OUT p_liquidacion_id INT
)
BEGIN
    INSERT INTO liquidaciones (profesor_id, mes, anio, monto_bruto, descuentos, monto_neto, estado)
    VALUES (p_profesor_id, p_mes, p_anio, p_monto_bruto, p_descuentos, p_monto_neto, p_estado);
    
    SET p_liquidacion_id = LAST_INSERT_ID();
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_liquidacion_por_id;

DELIMITER $$

CREATE PROCEDURE sp_obtener_liquidacion_por_id(
    IN p_id_liquidacion INT
)
BEGIN
    SELECT 
        id_liquidacion,
        profesor_id,
        mes,
        anio,
        monto_bruto,
        descuentos,
        monto_neto,
        fecha_pago,
        estado
    FROM liquidaciones
    WHERE id_liquidacion = p_id_liquidacion;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_liquidaciones_profesor;

DELIMITER $$

CREATE PROCEDURE sp_obtener_liquidaciones_profesor(
    IN p_profesor_id INT
)
BEGIN
    SELECT 
        l.id_liquidacion,
        l.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        l.mes,
        l.anio,
        l.monto_bruto,
        l.descuentos,
        l.monto_neto,
        l.fecha_pago,
        l.estado
    FROM liquidaciones l
    INNER JOIN profesores p ON l.profesor_id = p.id_profesor
    WHERE l.profesor_id = p_profesor_id
    ORDER BY l.anio DESC, l.mes DESC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_liquidaciones_periodo;

DELIMITER $$

CREATE PROCEDURE sp_obtener_liquidaciones_periodo(
    IN p_mes INT,
    IN p_anio INT
)
BEGIN
    SELECT 
        l.id_liquidacion,
        l.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        p.especialidad,
        l.mes,
        l.anio,
        l.monto_bruto,
        l.descuentos,
        l.monto_neto,
        l.fecha_pago,
        l.estado
    FROM liquidaciones l
    INNER JOIN profesores p ON l.profesor_id = p.id_profesor
    WHERE l.mes = p_mes AND l.anio = p_anio
    ORDER BY p.nombre, p.apellido;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_obtener_todas_liquidaciones;

DELIMITER $$

CREATE PROCEDURE sp_obtener_todas_liquidaciones()
BEGIN
    SELECT 
        l.id_liquidacion,
        l.profesor_id,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        l.mes,
        l.anio,
        l.monto_bruto,
        l.descuentos,
        l.monto_neto,
        l.fecha_pago,
        l.estado
    FROM liquidaciones l
    INNER JOIN profesores p ON l.profesor_id = p.id_profesor
    ORDER BY l.anio DESC, l.mes DESC, p.nombre;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_actualizar_liquidacion;

DELIMITER $$

CREATE PROCEDURE sp_actualizar_liquidacion(
    IN p_id_liquidacion INT,
    IN p_monto_bruto DECIMAL(12,2),
    IN p_descuentos DECIMAL(12,2),
    IN p_monto_neto DECIMAL(12,2)
)
BEGIN
    UPDATE liquidaciones
    SET 
        monto_bruto = p_monto_bruto,
        descuentos = p_descuentos,
        monto_neto = p_monto_neto
    WHERE id_liquidacion = p_id_liquidacion;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_pagar_liquidacion;

DELIMITER $$

CREATE PROCEDURE sp_pagar_liquidacion(
    IN p_id_liquidacion INT,
    IN p_fecha_pago DATE,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    UPDATE liquidaciones
    SET 
        estado = 'PAGADO',
        fecha_pago = p_fecha_pago
    WHERE id_liquidacion = p_id_liquidacion;
    
    SET p_mensaje = 'Liquidación pagada correctamente.';
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_eliminar_liquidacion;

DELIMITER $$

CREATE PROCEDURE sp_eliminar_liquidacion(
    IN p_id_liquidacion INT,
    OUT p_mensaje VARCHAR(255)
)
BEGIN
    DECLARE v_estado VARCHAR(30);
    
    SELECT estado INTO v_estado FROM liquidaciones WHERE id_liquidacion = p_id_liquidacion;
    
    IF v_estado = 'PAGADO' THEN
        SET p_mensaje = 'No se puede eliminar. La liquidación ya fue pagada.';
    ELSE
        DELETE FROM liquidaciones WHERE id_liquidacion = p_id_liquidacion;
        SET p_mensaje = 'Liquidación eliminada correctamente.';
    END IF;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_reporte_liquidaciones_periodo;

DELIMITER $$

CREATE PROCEDURE sp_reporte_liquidaciones_periodo(
    IN p_mes INT,
    IN p_anio INT
)
BEGIN
    SELECT 
        l.mes,
        l.anio,
        COUNT(*) as cantidad_profesores,
        SUM(l.monto_bruto) as total_bruto,
        SUM(l.descuentos) as total_descuentos,
        SUM(l.monto_neto) as total_neto,
        SUM(CASE WHEN l.estado = 'PAGADO' THEN 1 ELSE 0 END) as pagos_realizados,
        SUM(CASE WHEN l.estado = 'PENDIENTE' THEN 1 ELSE 0 END) as pagos_pendientes
    FROM liquidaciones l
    WHERE l.mes = p_mes AND l.anio = p_anio
    GROUP BY l.mes, l.anio;
END$$

DELIMITER ;
