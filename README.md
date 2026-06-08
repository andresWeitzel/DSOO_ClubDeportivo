
# Documento de Análisis y Alcance

**Fuente oficial (versión 1.0 — Abril 2026):**  
[`doc/definiciones_club_deportivo/analisis_club_deportivo.docx`](doc/definiciones_club_deportivo/analisis_club_deportivo.docx)

Incluye diagrama de clases, modelo entidad-relación y casos de uso UML 2.0. El resumen siguiente condensa el alcance funcional del TP; para el detalle completo (tablas, flujos y excepciones), consultar el documento Word.

## 1. Introducción

El sistema informatiza las operaciones administrativas, deportivas y de salud de un club con actividades para **socios** (acceso continuo con carnet, ficha médica y cuota mensual) y **visitantes** (ingreso diario con pago). Este análisis es la base de diseño y desarrollo del proyecto.

## 2. Alcance del sistema

| Módulo | Contenido |
|--------|-----------|
| **Gestión de usuarios** | Socios (carnet, ficha médica, estado de cuota) y visitantes (pago diario, sin carnet). |
| **Cuotas y pagos** | Cuotas mensuales con vencimiento; mora suspende acceso hasta regularizar; pagos de visitantes. |
| **Personal (profesores)** | Horarios, especialidades, asistencia diaria, rutinas personalizadas, liquidación mensual (último día hábil). |
| **Nutrición** | Turnos semanales, fichas médicas y carga de actividad física permitida. |
| **Reportes y control** | Cuotas por vencer / vencidas, asistencia de profesores, seguimiento general. |

## 3. Requerimientos funcionales

| ID | Descripción | Módulo |
|----|-------------|--------|
| RF-01 | Registrar socios con datos personales, foto y carnet | Usuarios |
| RF-02 | Registrar visitantes con pago diario | Usuarios |
| RF-03 | Emitir y reimprimir carnets de socio | Usuarios |
| RF-04 | Registrar pago de cuota mensual con vencimiento | Cuotas |
| RF-05 | Bloquear acceso a socio con cuota vencida (mora) | Cuotas |
| RF-06 | Registrar pago de mora y reactivar acceso | Cuotas |
| RF-07 | Registrar pago diario de visitante | Pagos |
| RF-08 | ABM de profesores con especialidad y horarios | Personal |
| RF-09 | Registrar asistencia diaria de profesores | Personal |
| RF-10 | Confeccionar rutinas personalizadas por alumno | Personal |
| RF-11 | Liquidar haberes mensuales (último día hábil) | Personal |
| RF-12 | Gestionar turnos semanales de nutrición | Nutrición |
| RF-13 | Administrar fichas médicas de socios | Nutrición |
| RF-14 | Definir carga de actividad física permitida | Nutrición |
| RF-15 | Listado diario de cuotas próximas a vencer | Reportes |
| RF-16 | Listado de socios en mora | Reportes |
| RF-17 | Reporte de asistencia de profesores | Reportes |

## 4. Modelo conceptual (resumen)

**Clases principales:** `Persona` → `Socio` / `Visitante`; `Cuota`, `Pago`, `Carnet`, `FichaMedica`, `Profesor`, `HorarioClase`, `Asistencia`, `Rutina`, `TurnoNutricion`, `Liquidacion`, `Actividad`.

**Persistencia (ER):** tablas `socios`, `visitantes`, `actividades`, `cuotas`, `pagos`, `profesores`, `horarios_actividad`, `asistencias`, `rutinas`, `fichas_medicas`, `turnos_nutricion`, `liquidaciones`, `carnets` (ver `Database/Scripts/01_DDL.sql`).

## 5. Casos de uso implementados / planificados

| CU | Nombre | Actor | Pantalla / módulo en este repo |
|----|--------|-------|-------------------------------|
| CU-01 | Registrar socio | Administrador | `FormSocios` |
| CU-02 | Registrar visitante | Administrador | `FormVisitantes` |
| RF-03 | Emitir / renovar carnet | Administrador | `FormCarnets` |
| CU-03 | Cobrar cuota mensual | Administrador | `FormCobroCuota` |
| CU-04 | Controlar vencimiento de cuotas | Sistema | `FormReportes` + `sp_controlar_vencimiento_cuotas` |
| CU-05 | Firmar asistencia | Profesor | `FormAsistencias` |
| CU-06 | Confeccionar rutina | Profesor | `FormRutinas` |
| CU-07 | Gestionar turno de nutrición | Admin / Nutricionista | `FormTurnosNutricion` |
| CU-08 | Liquidar haberes | Administrador | `FormLiquidarHaberes` |
| CU-09 | Generar reportes | Administrador / Empleado | `FormReportes` (RF-15, RF-16; RF-17 solo Administrador) |

Detalle de flujos, precondiciones y excepciones (E1, etc.) en el [documento Word](doc/definiciones_club_deportivo/analisis_club_deportivo.docx).

### Control de acceso por rol

La UI filtra menú, panel principal y formularios según `Permisos.cs` y `Sesion.TieneRol`.

| Módulo | Administrador | Empleado | Profesor | Nutricionista | Socio / Visitante |
|--------|:-------------:|:--------:|:--------:|:-------------:|:-----------------:|
| Socios (CU-01) | ✓ | ✓ | — | — | — |
| Visitantes (CU-02) | ✓ | ✓ | — | — | — |
| Cobrar cuota (CU-03) | ✓ | ✓ | — | — | — |
| Carnets (RF-03) | ✓ | ✓ | — | — | — |
| Firmar asistencia (CU-05) | ✓ | — | ✓ | — | — |
| Rutinas (CU-06) | ✓ | — | ✓ | — | — |
| Turnos nutrición (CU-07) | ✓ | — | — | ✓ | — |
| Reportes cuotas (CU-09) | ✓ | ✓ | — | — | — |
| Reporte asistencia RF-17 | ✓ | — | — | — | — |
| Liquidar haberes (CU-08) | ✓ | — | — | — | — |

**Socio** y **Visitante** no ingresan a la app de gestión (solo personal interno). Usuarios de prueba en `02_DML.sql`:

| Usuario | Contraseña | Rol |
|---------|------------|-----|
| `admin` | `1234` | Administrador |
| `empleado1` | `emp123` | Empleado |
| `juan_prof` | `prof123` | Profesor |
| `maria_nutri` | `nutri123` | Nutricionista |

## 6. Glosario

| Término | Definición |
|---------|------------|
| Socio | Inscripto con carnet, cuota mensual y acceso continuo. |
| Visitante | Asistencia ocasional con pago diario. |
| Cuota | Cargo mensual para mantener el acceso activo. |
| Mora | Cuota vencida sin pago; acceso suspendido. |
| Carnet | Identificación del socio. |
| Rutina | Plan de ejercicios asignado por un profesor. |
| Ficha médica | Datos de salud y restricciones del socio. |
| Liquidación | Cálculo y pago de haberes a profesores. |

---

# Tecnologías utilizadas

- C#
- .NET
- Windows Forms
- MySQL
- DBeaver
- Visual Studio Code
- Git

---


# Ejecutable y entrega

Guía resumida. El detalle completo (módulos, solución de problemas, etc.) está en [`doc/manual_usuario.pdf`](doc/manual_usuario.pdf).

## Requisitos en la PC destino

Para **usar** la aplicación no hace falta Visual Studio, DBeaver ni ningún IDE.

| Requisito | Detalle |
|-----------|---------|
| Sistema operativo | Windows 10 o superior (64 bits) |
| Runtime | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Motor SQL | MySQL 8.x o MariaDB 11.x **en ejecución** |
| Red | Acceso al servidor MySQL/MariaDB (`localhost:3306` en instalación local) |

Instalación rápida del runtime (terminal):

```bash
winget install Microsoft.DotNet.DesktopRuntime.8
dotnet --list-runtimes
```

Debe aparecer `Microsoft.WindowsDesktop.App 8.0.x`.

MySQL/MariaDB: el servicio debe estar activo. Opcionalmente agregar al PATH la carpeta `bin` del motor (ej. `C:\Program Files\MariaDB 11.6\bin`) y verificar con `mysql --version`.

La base **`db_club_deportivo`** no se crea a mano: la aplicación la inicializa sola la primera vez, si el usuario MySQL tiene permisos para crear bases y procedimientos almacenados.

### Solo para compilar desde el código fuente

```bash
winget install Microsoft.DotNet.SDK.8
dotnet --list-sdks
```

## Cómo se obtiene y ejecuta el sistema

El ejecutable está en:

```text
bin/Release/net8.0-windows/
├── ClubDeportivo.exe
├── … (DLLs .NET)
└── Database/Scripts/       ← 19 archivos .sql
```

**Pasos:**

1. Copiar o descomprimir **toda** la carpeta `bin/Release/net8.0-windows/` (no solo el `.exe`).
2. Ejecutar `ClubDeportivo.exe`.
3. Verificar que exista `Database/Scripts/` junto al ejecutable.

El repositorio puede incluir esa carpeta para ejecutar sin compilar. Tras cambios en el código, recompilar y actualizar.

## Compilar para entrega

Doble clic en `build_release.bat`, o desde la raíz del proyecto:

```bash
dotnet clean -c Release
dotnet build -c Release
```

La salida queda en `bin/Release/net8.0-windows/`.

## Primera ejecución

### 1. Datos de instalación MySQL

Al abrir la app por primera vez (`FormConfiguracionConexion`):

| Campo | Valor habitual |
|-------|----------------|
| Servidor | `localhost` |
| Puerto | `3306` |
| Usuario | `root` (o usuario con permisos DDL/DML) |
| Contraseña | La de su instalación MySQL/MariaDB |
| Base de datos | `db_club_deportivo` (nombre fijo) |

Usar **Restablecer valores** si hace falta. Pulsar **Continuar al login**.

### 2. Inicialización automática de la base de datos

Si la base no existe o no está lista, la app ejecuta en orden `01_DDL.sql` … `19_sp_liquidaciones.sql` (`FormInicializacionBaseDatos`) y muestra el progreso en pantalla.

### 3. Inicio de sesión

Pantalla **Iniciar sesión** (`FormLogin`): usuario y contraseña; **Ver** (mostrar clave), **Recordar usuario**, **Ingresar** (SP `IngresoLogin`), **Probar conexión**, **Cambiar MySQL**.

Usuarios de prueba (`02_DML.sql`): `admin` / `1234`, `empleado1` / `emp123`, `juan_prof` / `prof123`, `maria_nutri` / `nutri123`.

### 4. Panel principal

Módulos según rol (`Permisos.cs`). Ver tabla de acceso en la sección 5 de este README.

## Documentación (entrega académica)

| Documento | Ubicación |
|-----------|-----------|
| Manual de usuario | `doc/manual_usuario.docx` / `manual_usuario.pdf` |
| Cuadro de referencia | `doc/cuadro_referencia.docx` / `cuadro_referencia.pdf` |
| Análisis y diagramas | `doc/definiciones_club_deportivo/` |

Fuente del manual: `doc/manual_usuario.md`. Regenerar Word/PDF: `py doc/generar_documentacion.py`.

---
