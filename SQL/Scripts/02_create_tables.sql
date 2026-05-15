USE db_club_deportivo;

CREATE TABLE usuarios (
    id_usuario INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(100) NOT NULL,
    rol VARCHAR(30),
    fecha_registro DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE socios (
    id_socio INT PRIMARY KEY AUTO_INCREMENT,
    dni VARCHAR(20) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    telefono VARCHAR(50),
    direccion VARCHAR(150),
    estado_cuota VARCHAR(30),
    fecha_alta DATETIME DEFAULT CURRENT_TIMESTAMP
);