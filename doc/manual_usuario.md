# Manual de usuario — Club Deportivo

**Versión:** 1.0 · **Aplicación:** ClubDeportivo.exe · **Base de datos:** db_club_deportivo

---

## 1. Introducción

El **Sistema de Gestión Club Deportivo** es una aplicación de escritorio para Windows que informatiza las operaciones diarias de un club: gestión de socios y visitantes, cobro de cuotas, emisión de carnets, control del personal docente, turnos de nutrición y reportes administrativos.

Está desarrollado en **C#**, **.NET 8** y **Windows Forms**, con persistencia en **MySQL** o **MariaDB** mediante procedimientos almacenados y capa DAO.

El análisis formal del sistema (diagramas de clases, modelo entidad-relación y casos de uso UML) se encuentra en `doc/definiciones_club_deportivo/analisis_club_deportivo.docx`.

---

## 2. Instalación de herramientas y requisitos

Para **usar** la aplicación en una computadora con Windows, no hace falta instalar Visual Studio, DBeaver ni ningún IDE de desarrollo. Lo que sí se necesita es lo siguiente.

### 2.1 Sistema operativo

**Windows 10 o superior**, de 64 bits.

### 2.2 Runtime de .NET 8

La aplicación `ClubDeportivo.exe` está compilada para .NET 8. Si la PC no tiene el runtime instalado, descargue el **.NET 8 Desktop Runtime** desde el sitio oficial de Microsoft:

https://dotnet.microsoft.com/download/dotnet/8.0

También puede instalarlo desde una terminal con:

```
winget install Microsoft.DotNet.DesktopRuntime.8
```

Para verificar que quedó instalado, abra una terminal y ejecute:

```
dotnet --list-runtimes
```

Debe aparecer una línea similar a `Microsoft.WindowsDesktop.App 8.0.x`.

### 2.3 SDK de .NET 8 (solo para compilar el código fuente)

Si desea **compilar** el proyecto desde el repositorio, necesita además el SDK de .NET 8:

```
winget install Microsoft.DotNet.SDK.8
```

Verificación:

```
dotnet --list-sdks
```

Debe figurar la versión 8.0.

### 2.4 Motor de base de datos MySQL o MariaDB

Se requiere **MySQL 8** o **MariaDB 11** (en el grupo de desarrollo se usa MariaDB 11.6, compatible con MySQL).

Después de instalar el motor, conviene agregar su carpeta `bin` al **PATH** de Windows. Ejemplo:

```
C:\Program Files\MariaDB 11.6\bin
```

Así puede ejecutar comandos desde cualquier terminal. Para comprobar el cliente:

```
mysql --version
```

La terminal debe responder con la versión instalada (por ejemplo `mysql.exe from 11.x-MariaDB`).

Asegúrese de que el **servicio** de MySQL o MariaDB esté **en ejecución**. Puede verificarlo en el Administrador de servicios de Windows o intentando conectarse con el cliente.

### 2.5 Base de datos del sistema

La base de datos se llama **`db_club_deportivo`**. No hace falta crearla manualmente: la aplicación la crea e inicializa automáticamente la primera vez que se ejecuta, siempre que el usuario de MySQL tenga permisos para crear bases de datos y procedimientos almacenados.

### 2.6 Resumen de requisitos

| Requisito | Detalle |
|-----------|---------|
| Sistema operativo | Windows 10 o superior (64 bits) |
| Runtime | .NET 8 Desktop Runtime |
| Motor SQL | MySQL 8.x o MariaDB 11.x en ejecución |
| Red | Acceso al servidor MySQL/MariaDB (local o en red) |

---

## 3. Cómo se obtiene y ejecuta el sistema

El ejecutable y todos los archivos necesarios están en la carpeta:

```
bin/Release/net8.0-windows/
```

Ahí encontrará `ClubDeportivo.exe`, las librerías que necesita y, muy importante, la subcarpeta `Database/Scripts/` con los diecinueve archivos SQL. **Debe copiar o descargar toda esa carpeta**, no solo el archivo `.exe`, porque sin los scripts la inicialización de la base de datos no funciona.

La carpeta también puede obtenerse desde el repositorio de GitHub del grupo, junto con el código fuente y la documentación en `doc/`.

### 3.1 Pasos para iniciar la aplicación

1. Descomprimir o copiar **toda** la carpeta `bin/Release/net8.0-windows/`.
2. Ejecutar **`ClubDeportivo.exe`**.
3. Verificar que exista la subcarpeta **`Database/Scripts/`** junto al ejecutable.

### 3.2 Compilar desde el código fuente

Si tiene el proyecto completo, puede generar el ejecutable con el archivo `build_release.bat` en la raíz del repositorio, o desde una terminal en la raíz del proyecto:

```
dotnet clean -c Release
dotnet build -c Release
```

La salida queda en `bin/Release/net8.0-windows/`.

---

## 4. Primera ejecución

### 4.1 Configurar conexión MySQL

Al abrir `ClubDeportivo.exe` por primera vez, aparece la pantalla **Datos de instalación MySQL**:

| Campo | Valor habitual |
|-------|----------------|
| Servidor | localhost |
| Puerto | 3306 |
| Usuario | root (o un usuario con permisos DDL/DML) |
| Contraseña | La de su instalación MariaDB/MySQL |
| Base de datos | db_club_deportivo (nombre fijo, no editable) |

- **Restablecer valores:** vuelve a los datos por defecto.
- **Continuar al login:** guarda la configuración y sigue al siguiente paso.

### 4.2 Inicialización automática de la base de datos

Tras confirmar la conexión, la aplicación:

1. Verifica si `db_club_deportivo` existe y está lista para el login.
2. Si no lo está, ejecuta en orden los scripts `01_DDL.sql` … `19_sp_liquidaciones.sql`: primero crea las tablas, luego carga los datos de prueba y por último los procedimientos almacenados.
3. Muestra el progreso en pantalla (tablas, datos, procedimientos almacenados).

Si falla: comprobar que el servicio MySQL/MariaDB está activo y que el usuario puede crear bases y procedures.

### 4.3 Inicio de sesión

Pantalla **Iniciar sesión**:

| Control | Uso |
|---------|-----|
| Usuario / Contraseña | Credenciales del personal interno |
| Ver | Muestra u oculta la contraseña |
| Recordar usuario | Guarda el nombre de usuario en el equipo |
| Ingresar | Valida contra el procedimiento almacenado IngresoLogin |
| Probar conexión | Verifica acceso al servidor con los datos guardados |
| Cambiar MySQL | Vuelve a la pantalla de configuración |

Si hay error de conexión, use **Probar conexión** o **Cambiar MySQL**. Si la base no está disponible, el sistema puede ofrecer reinicializarla.

**Usuarios de prueba** (datos iniciales en `02_DML.sql`):

| Usuario | Contraseña | Rol | Acceso a la app |
|---------|------------|-----|-----------------|
| admin | 1234 | Administrador | Completo |
| empleado1 | emp123 | Empleado | Socios, visitantes, cuotas, carnets, reportes |
| juan_prof | prof123 | Profesor | Asistencias, rutinas |
| maria_nutri | nutri123 | Nutricionista | Turnos de nutrición |

Los roles **Socio** y **Visitante** no ingresan al módulo de gestión (se muestra acceso denegado).

---

## 5. Arquitectura del sistema

El sistema sigue el patrón visto en la materia:

**Pantallas** en Windows Forms → **DAOs** en C# → **procedimientos almacenados** en MySQL/MariaDB (mediante **MySqlConnector**).

Es decir: Formularios → DAO → Base de datos con Stored Procedures.

### 5.1 Módulos principales

- **Gestión de usuarios:** socios con carnet, ficha médica y cuota mensual; visitantes con pago diario.
- **Cuotas y pagos:** cobro mensual, control de mora, pagos de visitantes.
- **Personal:** profesores, asistencia diaria, rutinas de entrenamiento y liquidación de haberes.
- **Nutrición:** turnos semanales y actualización de fichas médicas.
- **Reportes:** cuotas por vencer, socios en mora y asistencia de profesores.

### 5.2 Base de datos

Tablas principales: socios, visitantes, cuotas, pagos, carnets, fichas_medicas, profesores, asistencias, rutinas, turnos_nutricion y liquidaciones. El script de creación está en `Database/Scripts/01_DDL.sql`.

La documentación de referencia (pantallas, controles y procedimientos) está en `doc/cuadro_referencia.pdf`.

---

## 6. Roles y permisos

El sistema maneja **seis roles** en la base de datos:

| Rol | Acceso |
|-----|--------|
| **Administrador** | Acceso completo a todos los módulos |
| **Empleado** | Socios, visitantes, cuotas, carnets y reportes de cuotas (no liquidar haberes ni reporte de asistencia de profesores) |
| **Profesor** | Firmar asistencia y confeccionar rutinas |
| **Nutricionista** | Módulo de turnos de nutrición |
| **Socio** | No puede ingresar a esta aplicación de gestión interna |
| **Visitante** | No puede ingresar a esta aplicación de gestión interna |

El menú lateral y las tarjetas del panel principal se muestran u ocultan según el rol, mediante la clase `Permisos.cs`.

---

## 7. Panel principal

Tras el login, el **panel principal** muestra tarjetas según el rol del usuario. El menú lateral repite los mismos módulos.

Cada módulo abre una ventana independiente (modal). Al cerrarla se vuelve al panel principal.

---

## 8. Módulos por caso de uso

### CU-01 — Gestión de socios (FormSocios)

**Actores:** Administrador, Empleado.

1. **Buscar por DNI** o **Actualizar** la lista.
2. **Alta:** botón **Nuevo**, completar DNI, nombre, apellido, teléfono, dirección, email y cuota inicial.
3. **Registrar socio** guarda socio, carnet, cuota y ficha médica básica.
4. **Edición:** seleccionar fila en la grilla; modificar datos y **Guardar cambios**.
5. **Eliminar:** baja lógica del socio seleccionado.

### CU-02 — Ingreso de visitantes (FormVisitantes)

**Actores:** Administrador, Empleado.

1. **Refrescar lista** para ver visitantes del día.
2. **Alta:** DNI, nombre, apellido, teléfono, actividad, monto diario y medio de pago.
3. **Registrar ingreso** crea visitante y registra el pago (valida cupo de la actividad).
4. **Registrar pago pendiente** aparece si el visitante no tenía pago al seleccionarlo.

### CU-03 — Cobro de cuota mensual (FormCobroCuota)

**Actores:** Administrador, Empleado.

1. Ingresar **DNI del socio** → **Buscar**.
2. Revisar cuotas en la grilla (estado AL DÍA, MORA, etc.).
3. Seleccionar cuota; completar monto, medio de pago y concepto.
4. Opcional: **Generar próxima cuota (+30 días)**.
5. **Registrar cobro** registra el pago y actualiza la cuota.

**Limpiar** reinicia la búsqueda.

### RF-03 — Gestión de carnets (FormCarnets)

**Actores:** Administrador, Empleado.

1. Buscar socio por DNI.
2. Ver número, emisión, vencimiento y estado (VIGENTE / VENCIDO).
3. **Emitir carnet** si no existe.
4. **Renovar (+1 año)** si está vencido o próximo a vencer.

### CU-05 — Firmar asistencia (FormAsistencias)

**Actores:** Administrador, Profesor.

1. Elegir **profesor** y **fecha** (hasta hoy).
2. **Buscar asistencia** carga la grilla del día.
3. Seleccionar fila; ingresar **firma** y marcar **Presente**.
4. **Registrar / firmar** guarda o actualiza la asistencia.

### CU-06 — Confeccionar rutina (FormRutinas)

**Actores:** Administrador, Profesor.

1. Buscar socio por DNI (muestra estado de cuota y ficha médica).
2. Revisar rutinas existentes en la grilla.
3. Elegir profesor, descripción y observaciones.
4. **Guardar rutina** (requiere socio con cuota al día y ficha médica).

### CU-07 — Turnos de nutrición (FormTurnosNutricion)

**Actores:** Administrador, Nutricionista.

1. Buscar socio por DNI.
2. En **Consulta — actualizar ficha:** peso, altura, alergias, medicación, observaciones y carga permitida → **Guardar ficha**.
3. En **Asignar turno nutrición:** nutricionista, fecha, hora disponible.
4. **Buscar próxima fecha** si no hay turnos en la fecha elegida.
5. **Asignar turno** confirma la reserva.

La grilla superior lista turnos del socio (consultas anteriores).

### CU-08 — Liquidar haberes (FormLiquidarHaberes)

**Actores:** Administrador.

1. Elegir **mes y año** del período.
2. **Consultar período** carga liquidaciones existentes.
3. **Liquidar mes** genera recibos según sueldos y asistencias (RF-11: aviso si no es último día hábil).
4. Seleccionar recibo **PENDIENTE** → **Registrar pago** con fecha de pago.
5. **Ver recibo** muestra el detalle del recibo seleccionado.

### CU-09 — Reportes (FormReportes)

**Actores:** Administrador, Empleado (RF-17 solo Administrador).

| Reporte | Descripción |
|---------|-------------|
| Cuotas por vencer | Socios con vencimiento próximo (RF-15) |
| Socios en mora | Cuotas vencidas sin pago (RF-16) |
| Asistencia profesores | Resumen por período (RF-17) |

Cada reporte permite **Exportar CSV** cuando está disponible.

---

## 9. Cierre de sesión

Desde el panel principal use **Cerrar sesión** en el menú lateral. La aplicación vuelve a la pantalla de login.

---

## 10. Solución de problemas frecuentes

| Problema | Qué hacer |
|----------|-----------|
| Error de conexión al login | **Probar conexión**; verificar servicio MySQL y credenciales en **Cambiar MySQL** |
| Base de datos no disponible | Aceptar reinicializar cuando lo ofrezca el login, o ejecutar `Database\Setup\init_db.bat` manualmente |
| Usuario o contraseña incorrectos | Verificar tabla usuario o usar admin / 1234 en datos de prueba |
| Botones o campos recortados | Usar resolución ≥ 1280×720 y escala de Windows 100–125 % |
| Falta Database\Scripts | Copiar la carpeta completa de entrega, no solo el .exe |
| .NET 8 no instalado | Instalar .NET 8 Desktop Runtime (sección 2.2) |
| mysql no reconocido en terminal | Agregar la carpeta bin de MySQL/MariaDB al PATH (sección 2.4) |

---

## 11. Contacto y soporte

Proyecto académico — **TP Desarrollo de Sistemas con Objetos** (IFTS 29).

Documentación de análisis: `doc/definiciones_club_deportivo/analisis_club_deportivo.docx`.
