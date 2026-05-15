USE db_club_deportivo;

DROP PROCEDURE IF EXISTS sp_login;

DELIMITER $$

CREATE PROCEDURE sp_login(
    IN p_username VARCHAR(50),
    IN p_password VARCHAR(100)
)
BEGIN

    SELECT *
    FROM usuarios
    WHERE username = p_username
      AND password = p_password;

END $$

DELIMITER ;

CALL sp_login('admin', '1234');
