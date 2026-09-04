# Plan de Pruebas — Casos de Prueba
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Versión:** 1.0 | Fecha: septiembre 2026

---

## CP01 — Autenticación de usuario

| Campo | Detalle |
|---|---|
| ID | CP01 |
| Módulo | Gestión de acceso (P01) |
| Tipo | Funcional |
| Prioridad | Alta |

**Precondiciones:** El sistema está activo y el usuario existe en la BD.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Login exitoso adulto mayor | correo: admin@medicacion.com, contraseña: Admin2026! | HTTP 200 + token JWT + rol: Administrador | HTTP 200 + token JWT generado | ✅ Passed |
| 2 | Login con contraseña incorrecta | correo: admin@medicacion.com, contraseña: incorrecta | HTTP 401 + mensaje: Credenciales incorrectas | HTTP 401 retornado | ✅ Passed |
| 3 | Login con correo inexistente | correo: noexiste@correo.com, contraseña: cualquiera | HTTP 401 + mensaje: Credenciales incorrectas | HTTP 401 retornado | ✅ Passed |
| 4 | Login con campos vacíos | correo: vacío, contraseña: vacía | Mensaje de validación en pantalla | Mensaje visible en app | ✅ Passed |
| 5 | Cierre de sesión | Token JWT activo | Token eliminado + redirección a login | Preferencias borradas | ✅ Passed |

---

## CP02 — Gestión de medicamentos

| Campo | Detalle |
|---|---|
| ID | CP02 |
| Módulo | Gestión de medicamentos (P02) |
| Tipo | Funcional |
| Prioridad | Alta |

**Precondiciones:** Usuario autenticado con token JWT válido.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Registrar medicamento válido | nombre: Losartán, dosis: 50, unidad: mg, frecuencia: Una vez al día, hora: 08:00 | HTTP 200 + idMedicamento generado | HTTP 200 + idMedicamento: 1 | ✅ Passed |
| 2 | Registrar medicamento duplicado | nombre: Losartán (ya existe para el mismo usuario) | HTTP 400 + mensaje de duplicado | HTTP 400 retornado | ✅ Passed |
| 3 | Registrar sin campos obligatorios | nombre: vacío | Mensaje de validación | Validación en app activa | ✅ Passed |
| 4 | Consultar medicamentos por usuario | idUsuario: 1 | Lista con medicamentos y horarios | Lista retornada correctamente | ✅ Passed |
| 5 | Eliminar medicamento | idMedicamento: existente | HTTP 200 + activo = false en BD | Soft delete aplicado | ✅ Passed |
| 6 | Registrar con horario | hora: 08:00 | Medicamento + horario registrado en BD | INSERT en Medicamento y Horario | ✅ Passed |

---

## CP03 — Monitoreo y confirmación de toma

| Campo | Detalle |
|---|---|
| ID | CP03 |
| Módulo | Confirmación de toma (P04) |
| Tipo | Funcional |
| Prioridad | Alta |

**Precondiciones:** Medicamento y horario registrados. Token JWT válido.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Registrar confirmación válida | idUsuario: 1, idMedicamento: 1, idHorario: 1 | HTTP 200 + idConfirmacion + timestamp | HTTP 200 + confirmación registrada | ✅ Passed |
| 2 | Registrar confirmación sin token | Sin Authorization header | HTTP 401 | HTTP 401 retornado | ✅ Passed |
| 3 | Verificar confirmación existente hoy | idHorario: 1 | confirmado: true | Retorna confirmado: true | ✅ Passed |
| 4 | Verificar sin confirmación hoy | idHorario: sin confirmar hoy | confirmado: false | Retorna confirmado: false | ✅ Passed |
| 5 | Consultar historial por período | idUsuario: 1, período: última semana | Lista cronológica de confirmaciones | Lista retornada con timestamps | ✅ Passed |

---

## CP04 — Generación de alertas por incumplimiento

| Campo | Detalle |
|---|---|
| ID | CP04 |
| Módulo | Gestión de alertas (P05) |
| Tipo | Funcional |
| Prioridad | Alta |

**Precondiciones:** Horario registrado sin confirmación. VerificacionService activo.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Detección automática de incumplimiento | Horario 08:00 sin confirmación, hora actual 08:31 | INSERT INTO Alerta + log de advertencia | Alerta generada: Losartán, Metformina | ✅ Passed |
| 2 | No duplicar alerta del mismo día | Alerta ya generada para hoy | No genera segunda alerta | Sin duplicados en BD | ✅ Passed |
| 3 | No generar alerta si hay confirmación | Horario con confirmación registrada | No genera alerta | Validación correcta | ✅ Passed |
| 4 | Consultar alertas por usuario | idUsuario: 1 | Lista de alertas del período | Lista retornada correctamente | ✅ Passed |
| 5 | Actualizar estado de alerta | idAlerta: 1 → estado: enviada | HTTP 200 + horaEnvio registrada | Estado actualizado en BD | ✅ Passed |

---

## CP05 — Panel de seguimiento familiar

| Campo | Detalle |
|---|---|
| ID | CP05 |
| Módulo | Consultar seguimiento (P07) |
| Tipo | Funcional |
| Prioridad | Media |

**Precondiciones:** Usuario con medicamentos, confirmaciones y alertas registradas.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Consultar panel del familiar | idUsuario: 1, mes: actual | Porcentaje + totales + últimas actividades | Datos retornados correctamente | ✅ Passed |
| 2 | Indicador verde | Cumplimiento > 80% | colorIndicador: verde | Calculado correctamente | ✅ Passed |
| 3 | Indicador rojo | Cumplimiento < 50% | colorIndicador: rojo | Calculado correctamente | ✅ Passed |
| 4 | Panel sin confirmaciones | Usuario sin confirmar nada | 0% cumplimiento + alertas generadas | 0% retornado + alertas en BD | ✅ Passed |

---

## CP06 — Pruebas de integración app MAUI

| Campo | Detalle |
|---|---|
| ID | CP06 |
| Módulo | Integración app móvil — backend |
| Tipo | Integración |
| Prioridad | Alta |

**Precondiciones:** Backend corriendo en localhost:5271. Emulador Android activo.

| # | Caso | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Login desde emulador | correo: admin@medicacion.com, contraseña: Admin2026! | Navegación a PanelFamiliarPage | Navegación exitosa | ✅ Passed |
| 2 | Carga de medicamentos en panel | idUsuario autenticado | Lista de medicamentos visible | Losartán y otros visibles | ✅ Passed |
| 3 | Agregar medicamento desde app | Formulario completo con Metformina | Medicamento guardado en BD | INSERT confirmado en log | ✅ Passed |
| 4 | Cerrar sesión desde app | Botón cerrar sesión | Regreso a LoginPage | Preferencias eliminadas | ✅ Passed |
| 5 | Error de conexión | Backend detenido | Mensaje de error en pantalla | Mensaje visible en app | ✅ Passed |

---

## CP07 — Pruebas no funcionales

| Campo | Detalle |
|---|---|
| ID | CP07 |
| Módulo | Rendimiento y seguridad |
| Tipo | No funcional |
| Prioridad | Media |

| # | Caso | Criterio | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| 1 | Tiempo de respuesta login | Menor a 2 segundos | HTTP 200 en menos de 2s | 429ms en BD + overhead de red | ✅ Passed |
| 2 | Seguridad JWT | Solicitud sin token a endpoint protegido | HTTP 401 | HTTP 401 en todos los endpoints | ✅ Passed |
| 3 | Encriptación de contraseña | Contraseña almacenada en BD | Hash BCrypt visible en tabla | Hash almacenado correctamente | ✅ Passed |
| 4 | Detección de incumplimiento | Servicio ejecuta verificación periódica | Log cada minuto | Log visible en consola | ✅ Passed |
| 5 | Soft delete | Eliminar medicamento | Activo = false, registro no eliminado | Verificado en SSMS | ✅ Passed |

---

## Resumen de resultados

| Módulo | Total casos | Passed | Failed |
|---|---|---|---|
| CP01 — Autenticación | 5 | 5 | 0 |
| CP02 — Medicamentos | 6 | 6 | 0 |
| CP03 — Confirmaciones | 5 | 5 | 0 |
| CP04 — Alertas | 5 | 5 | 0 |
| CP05 — Panel familiar | 4 | 4 | 0 |
| CP06 — Integración MAUI | 5 | 5 | 0 |
| CP07 — No funcionales | 5 | 5 | 0 |
| **Total** | **35** | **35** | **0** |

**Porcentaje de éxito: 100%**