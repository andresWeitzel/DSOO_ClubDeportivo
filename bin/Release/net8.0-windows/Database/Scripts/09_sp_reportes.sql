USE db_club_deportivo;

-- ============================================
-- STORED PROCEDURES - REPORTES
-- ============================================

DROP PROCEDURE IF EXISTS sp_cuotas_por_vencer;

DELIMITER $$

CREATE PROCEDURE sp_cuotas_por_vencer(
    IN p_dias INT
)
BEGIN
    SELECT 
        s.id_socio,
        s.dni,
        s.nombre,
        s.apellido,
        s.estado_cuota,
        c.id_cuota,
        c.monto,
        c.fecha_vencimiento,
        DATEDIFF(c.fecha_vencimiento, CURDATE()) as dias_para_vencer
    FROM socios s
    INNER JOIN cuotas c ON s.id_socio = c.socio_id
    WHERE c.fecha_vencimiento BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL p_dias DAY)
    AND c.estado != 'PAGADA'
    ORDER BY c.fecha_vencimiento ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_cuotas_vencidas;

DELIMITER $$

CREATE PROCEDURE sp_cuotas_vencidas()
BEGIN
    SELECT 
        s.id_socio,
        s.dni,
        s.nombre,
        s.apellido,
        s.estado_cuota,
        c.id_cuota,
        c.monto,
        c.fecha_vencimiento,
        DATEDIFF(CURDATE(), c.fecha_vencimiento) as dias_vencidos
    FROM socios s
    INNER JOIN cuotas c ON s.id_socio = c.socio_id
    WHERE c.fecha_vencimiento < CURDATE()
    AND c.estado != 'PAGADA'
    ORDER BY c.fecha_vencimiento ASC;
END$$

DELIMITER ;

DROP PROCEDURE IF EXISTS sp_reporte_asistencias_profesores;

DELIMITER $$

CREATE PROCEDURE sp_reporte_asistencias_profesores(
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE,
    IN p_profesor_id INT
)
BEGIN
    SELECT 
        p.id_profesor,
        CONCAT(p.nombre, ' ', p.apellido) AS profesor_nombre,
        COALESCE(p.especialidad, '') AS especialidad,
        COUNT(a.id_asistencia) AS total_registros,
        COALESCE(SUM(CASE WHEN a.presente = TRUE THEN 1 ELSE 0 END), 0) AS asistencias,
        COALESCE(SUM(CASE WHEN a.presente = FALSE THEN 1 ELSE 0 END), 0) AS inasistencias,
        CASE 
            WHEN COUNT(a.id_asistencia) = 0 THEN 0
            ELSE ROUND(SUM(CASE WHEN a.presente = TRUE THEN 1 ELSE 0 END) / COUNT(a.id_asistencia) * 100, 2)
        END AS porcentaje_asistencia
    FROM profesores p
    LEFT JOIN asistencias a 
        ON a.profesor_id = p.id_profesor
        AND a.fecha BETWEEN p_fecha_inicio AND p_fecha_fin
    WHERE p_profesor_id IS NULL OR p_profesor_id = 0 OR p.id_profesor = p_profesor_id
    GROUP BY p.id_profesor, p.nombre, p.apellido, p.especialidad
    ORDER BY p.nombre, p.apellido;
END$$

DELIMITER ;
