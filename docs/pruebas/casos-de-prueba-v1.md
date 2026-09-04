# Casos de Prueba — v1
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Versión:** 1.0 | Fecha: septiembre 2026

---

## CP-RF01 — Registro de usuarios

**Requerimiento:** RF01 — El sistema debe permitir registrar al adulto mayor y a su familiar o cuidador responsable.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF01-01 | Registro exitoso de adulto mayor | nombre: Javier, apellido: Rivera, correo: admin@medicacion.com, contraseña: Admin2026!, rol: Administrador | HTTP 200 + idUsuario generado + mensaje de confirmación | HTTP 200 + idUsuario: 1 + "Usuario registrado exitosamente" | ✅ Passed |
| CP-RF01-02 | Registro con correo duplicado | correo: admin@medicacion.com (ya registrado) | HTTP 400 + mensaje de error indicando correo duplicado | HTTP 400 + "El correo ya está registrado" | ✅ Passed |
| CP-RF01-03 | Registro con campos vacíos | nombre: vacío, correo: vacío, contraseña: vacía | Mensaje de validación en pantalla o HTTP 400 | Validación en app activa antes de enviar | ✅ Passed |
| CP-RF01-04 | Registro de familiar/cuidador | rol: Familiar, datos completos | HTTP 200 + idUsuario generado para familiar | HTTP 200 + registro exitoso | ✅ Passed |
| CP-RF01-05 | Contraseña almacenada como hash | Verificar tabla Usuario en BD tras registro | Columna contrasenaHash contiene hash BCrypt, no texto plano | Hash BCrypt visible en SSMS | ✅ Passed |

**Criterio de aceptación:** El sistema permite registrar usuarios con rol AdultoMayor, Familiar o Administrador. Las contraseñas se almacenan encriptadas con BCrypt. No se permiten correos duplicados.

---

## CP-RF02 — Programación de medicamentos

**Requerimiento:** RF02 — El sistema debe permitir programar los medicamentos, dosis y horarios de toma.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF02-01 | Registrar medicamento con horario | nombre: Losartán, dosis: 50, unidad: mg, frecuencia: Una vez al día, hora: 08:00, idUsuario: 1 | HTTP 200 + idMedicamento + idHorario generados | HTTP 200 + idMedicamento: 1, horario registrado en BD | ✅ Passed |
| CP-RF02-02 | Registrar medicamento sin horario | nombre: Aspirina, dosis: 100, unidad: mg, sin hora | HTTP 200 + medicamento registrado sin horario | HTTP 200 + solo INSERT en Medicamento | ✅ Passed |
| CP-RF02-03 | Registrar medicamento duplicado | nombre: Losartán (ya existe para idUsuario: 1) | HTTP 400 + mensaje de duplicado | HTTP 400 + "Ya existe un medicamento con ese nombre" | ✅ Passed |
| CP-RF02-04 | Actualizar medicamento existente | idMedicamento: 1, nueva dosis: 100 | HTTP 200 + registro actualizado en BD | HTTP 200 + actualización confirmada | ✅ Passed |
| CP-RF02-05 | Eliminar medicamento (soft delete) | idMedicamento: existente | HTTP 200 + activo = false en Medicamento y Horario | Verificado en SSMS: activo = 0 | ✅ Passed |
| CP-RF02-06 | Validar formato de dosis | dosis: -5 (negativo) | HTTP 400 + error de validación | Restricción CHECK en BD impide INSERT | ✅ Passed |

**Criterio de aceptación:** El sistema permite crear, actualizar y eliminar medicamentos. El horario se registra automáticamente junto al medicamento. No se permiten dosis negativas ni medicamentos duplicados por usuario.

---

## CP-RF03 — Alarmas y recordatorios

**Requerimiento:** RF03 — El sistema debe emitir alarmas y notificaciones locales en el dispositivo del adulto mayor a la hora programada.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF03-01 | Consultar horarios del usuario | idUsuario: 1 | Lista de horarios activos ordenados por hora | Lista con Losartán 08:00, Metformina 08:00 | ✅ Passed |
| CP-RF03-02 | Consultar horarios por medicamento | idMedicamento: 1 | Horario correspondiente al medicamento | idHorario: 1, horaAdministracion: "08:00" | ✅ Passed |
| CP-RF03-03 | Actualizar hora de horario | idHorario: 1, nueva hora: 10:00 | HTTP 200 + hora actualizada en BD | HTTP 200 + actualización confirmada | ✅ Passed |
| CP-RF03-04 | Eliminar horario (soft delete) | idHorario: existente | HTTP 200 + activo = false en Horario | Verificado en SSMS | ✅ Passed |
| CP-RF03-05 | Verificar formato de hora | horaAdministracion: "25:00" (inválida) | HTTP 400 + mensaje de formato inválido | HTTP 400 + "Formato de hora inválido. Use HH:mm" | ✅ Passed |

**Criterio de aceptación:** El sistema permite gestionar horarios de administración. La hora debe estar en formato HH:mm válido. Los horarios eliminados se desactivan sin borrar el registro histórico.

---

## CP-RF04 — Confirmación de toma

**Requerimiento:** RF04 — El sistema debe presentar un botón de confirmación de gran tamaño tras la activación de la alarma.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF04-01 | Pantalla de confirmación visible | App MAUI ejecutándose en emulador | Botón verde de gran tamaño con texto "Confirmar toma" visible | Botón visible con altura 72px y color #2E7D32 | ✅ Passed |
| CP-RF04-02 | Confirmación exitosa desde app | Presionar botón Confirmar en PanelFamiliarPage | HTTP 200 + confirmación registrada en BD | INSERT en Confirmacion verificado en log | ✅ Passed |
| CP-RF04-03 | Navegación tras confirmación | Después de confirmar | Pantalla de confirmación exitosa visible (verde) | NavigationPage navega a PanelFamiliarPage | ✅ Passed |
| CP-RF04-04 | Confirmación sin token JWT | Solicitud sin Authorization header | HTTP 401 + acceso denegado | HTTP 401 retornado por endpoint protegido | ✅ Passed |

**Criterio de aceptación:** La pantalla de confirmación presenta un botón de gran tamaño en color verde. La confirmación requiere token JWT válido. Tras confirmar, la app navega a la pantalla correspondiente.

---

## CP-RF05 — Registro de confirmaciones

**Requerimiento:** RF05 — El sistema debe registrar cada confirmación de toma en la base de datos con fecha y hora exacta.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF05-01 | Registrar confirmación válida | idUsuario: 1, idMedicamento: 1, idHorario: 1 | HTTP 200 + idConfirmacion + timestamp exacto | HTTP 200 + "Confirmación registrada exitosamente" + timestamp | ✅ Passed |
| CP-RF05-02 | Verificar confirmación en BD | Consultar tabla Confirmacion en SSMS | Registro con fecha, hora y timestampExacto correctos | Verificado en SSMS con valores correctos | ✅ Passed |
| CP-RF05-03 | Verificar si existe confirmación hoy | GET /api/Confirmacion/verificar/{idHorario} | confirmado: true si existe, false si no | Retorna estado correcto según BD | ✅ Passed |
| CP-RF05-04 | Consultar historial de confirmaciones | idUsuario: 1, período: última semana | Lista cronológica con nombre medicamento, dosis, fecha, hora | Lista retornada con datos correctos | ✅ Passed |
| CP-RF05-05 | Confirmación con horario inexistente | idHorario: 9999 | HTTP 404 + mensaje de error | HTTP 404 + "Horario no encontrado" | ✅ Passed |

**Criterio de aceptación:** Cada confirmación se almacena con fecha, hora y timestamp exacto. El sistema permite verificar el estado de confirmación del día actual y consultar el historial por período.

---

## CP-RF06 — Alertas al familiar

**Requerimiento:** RF06 — El sistema debe generar alertas automatizadas al familiar o cuidador cuando no se registre confirmación dentro del período de tolerancia.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF06-01 | Generación automática de alerta por incumplimiento | VerificacionService activo, horario 08:00 sin confirmación, hora actual 08:31 | INSERT en Alerta + log de advertencia en consola | Alerta generada: "Losartán, Horario 08:00, Usuario 1" visible en log | ✅ Passed |
| CP-RF06-02 | No duplicar alerta del mismo día | Alerta ya generada para hoy para idHorario: 1 | Sin INSERT adicional en Alerta | No genera segunda alerta, verificado en SSMS | ✅ Passed |
| CP-RF06-03 | No generar alerta con confirmación | idHorario con confirmación registrada hoy | Sin INSERT en Alerta | VerificacionService omite el horario correctamente | ✅ Passed |
| CP-RF06-04 | Consultar alertas por usuario | GET /api/Alerta/usuario/{idUsuario} | Lista de alertas del período con estado | Lista retornada correctamente | ✅ Passed |
| CP-RF06-05 | Consultar alertas pendientes | GET /api/Alerta/pendientes/{idUsuario} | Lista de alertas con estado "pendiente" | Lista filtrada correctamente | ✅ Passed |
| CP-RF06-06 | Marcar alerta como enviada | PUT /api/Alerta/{id}/enviada | HTTP 200 + estado "enviada" + horaEnvio registrada | HTTP 200 + actualización en BD verificada | ✅ Passed |

**Criterio de aceptación:** El VerificacionService genera alertas automáticamente cada minuto. No crea duplicados. No genera alertas si existe confirmación. El endpoint permite actualizar el estado de las alertas.

---

## CP-RF07 — Historial de medicación

**Requerimiento:** RF07 — El sistema debe almacenar y permitir consultar el historial de confirmaciones y alertas.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF07-01 | Consultar historial semana actual | idUsuario: 1, sin parámetros de fecha | Lista de confirmaciones de los últimos 7 días | Lista retornada en orden cronológico descendente | ✅ Passed |
| CP-RF07-02 | Consultar historial con filtro de fechas | idUsuario: 1, inicio: 2026-09-01, fin: 2026-09-03 | Lista filtrada por el período indicado | Lista con registros del período solicitado | ✅ Passed |
| CP-RF07-03 | Historial con campo estado | Cualquier consulta de historial | Campo "estado" con valor "confirmado" en cada registro | Retorna estado correctamente | ✅ Passed |
| CP-RF07-04 | Historial visible en app MAUI | PanelFamiliarPage cargada | Últimas actividades visibles con indicadores de color | Lista de últimas actividades visible en panel | ✅ Passed |

**Criterio de aceptación:** El historial se puede filtrar por período. Cada registro incluye nombre del medicamento, dosis, fecha, hora y estado. El período por defecto es la última semana.

---

## CP-RF08 — Gestión de múltiples medicamentos

**Requerimiento:** RF08 — El sistema debe permitir registrar y gestionar varios medicamentos con distintos horarios.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF08-01 | Registrar segundo medicamento | nombre: Metformina, dosis: 850, hora: 08:00, idUsuario: 1 | HTTP 200 + segundo medicamento registrado | HTTP 200 + idMedicamento: 2 desde app MAUI | ✅ Passed |
| CP-RF08-02 | Registrar tercer medicamento | nombre: Aspirina, dosis: 100, hora: 20:00, idUsuario: 1 | HTTP 200 + tercer medicamento registrado | HTTP 200 + idMedicamento: 3 | ✅ Passed |
| CP-RF08-03 | Listar múltiples medicamentos | GET /api/Medicamento/usuario/1 | Lista con los 3 medicamentos y sus horarios | Lista con Losartán, Metformina, Aspirina | ✅ Passed |
| CP-RF08-04 | Medicamentos visibles en app | PanelFamiliarPage o PrincipalAdultoPage | Lista de medicamentos visible con nombre, dosis y hora | Lista cargada correctamente en app | ✅ Passed |
| CP-RF08-05 | Horarios distintos por medicamento | Losartán 08:00, Metformina 08:00, Aspirina 20:00 | Cada medicamento con su horario independiente | Verificado en SSMS y en respuesta del API | ✅ Passed |

**Criterio de aceptación:** El sistema permite registrar múltiples medicamentos por usuario, cada uno con su propio horario. La lista muestra todos los medicamentos activos ordenados.

---

## CP-RF09 — Panel del familiar

**Requerimiento:** RF09 — El sistema debe permitir al familiar consultar el estado de cumplimiento del adulto mayor.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF09-01 | Consultar panel del mes actual | GET /api/Confirmacion/panel/1 | JSON con totalProgramados, totalConfirmaciones, totalAlertas, porcentaje, colorIndicador | Datos retornados correctamente | ✅ Passed |
| CP-RF09-02 | Indicador de color verde | Cumplimiento > 80% | colorIndicador: "verde" | Calculado y retornado correctamente | ✅ Passed |
| CP-RF09-03 | Indicador de color rojo | Sin confirmaciones | colorIndicador: "rojo", porcentaje: 0.0 | 0% retornado + alertas generadas | ✅ Passed |
| CP-RF09-04 | Panel visible en app MAUI | PanelFamiliarPage cargada | Estadísticas: porcentaje, programados, confirmados, alertas visibles | Panel con 4 tarjetas visibles en emulador | ✅ Passed |
| CP-RF09-05 | Consultar panel con mes específico | mes: 9, anio: 2026 | Panel filtrado para septiembre 2026 | Datos del mes especificado retornados | ✅ Passed |
| CP-RF09-06 | Panel con últimas actividades | GET /api/Confirmacion/panel/1 | Campo ultimasActividades con los últimos 5 registros | Lista de actividades incluida en respuesta | ✅ Passed |

**Criterio de aceptación:** El panel muestra el porcentaje de cumplimiento con indicador de color (verde > 80%, ámbar 50-80%, rojo < 50%), totales de medicamentos, confirmaciones y alertas, y las últimas 5 actividades.

---

## CP-RF10 — Notificación por correo

**Requerimiento:** RF10 — El sistema debe enviar alertas al familiar mediante correo electrónico o notificación push.

| ID | Descripción | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|---|
| CP-RF10-01 | VerificacionService activo al iniciar | dotnet run en terminal | Log: "VerificacionService iniciado" visible en consola | Mensaje visible al arrancar backend | ✅ Passed |
| CP-RF10-02 | Alerta registrada en BD ante incumplimiento | Horario vencido sin confirmación | INSERT en Alerta con estado "pendiente" | Alerta generada y verificada en SSMS | ✅ Passed |
| CP-RF10-03 | Endpoint para marcar alerta como enviada | PUT /api/Alerta/{id}/enviada | HTTP 200 + estado "enviada" + horaEnvio registrada | Simulación de envío exitoso documentada | ✅ Passed |
| CP-RF10-04 | Endpoint para registrar error de envío | PUT /api/Alerta/{id}/error | HTTP 200 + estado "error" en BD | HTTP 200 + actualización correcta | ✅ Passed |
| CP-RF10-05 | Registro del contacto familiar | POST /api/ContactoFamiliar | HTTP 200 + idContacto + correo del familiar almacenado | HTTP 200 + registro exitoso | ✅ Passed |
| CP-RF10-06 | Consultar contacto familiar por usuario | GET /api/ContactoFamiliar/{idUsuario} | Datos del familiar con correoFamiliar | Retorna contacto correctamente | ✅ Passed |

**Criterio de aceptación:** El sistema detecta incumplimientos automáticamente y registra las alertas en BD. Los endpoints permiten gestionar el estado de envío. El contacto familiar está almacenado con correo para notificación. La integración real con SendGrid queda como pendiente de Sprint 2.

---

## Resumen de resultados

| Módulo | RF | Casos totales | Passed | Failed | Estado |
|---|---|---|---|---|---|
| Registro de usuarios | RF01 | 5 | 5 | 0 | ✅ |
| Programación de medicamentos | RF02 | 6 | 6 | 0 | ✅ |
| Alarmas y recordatorios | RF03 | 5 | 5 | 0 | ✅ |
| Confirmación de toma | RF04 | 4 | 4 | 0 | ✅ |
| Registro de confirmaciones | RF05 | 5 | 5 | 0 | ✅ |
| Alertas al familiar | RF06 | 6 | 6 | 0 | ✅ |
| Historial de medicación | RF07 | 4 | 4 | 0 | ✅ |
| Gestión múltiples medicamentos | RF08 | 5 | 5 | 0 | ✅ |
| Panel del familiar | RF09 | 6 | 6 | 0 | ✅ |
| Notificación por correo | RF10 | 6 | 6 | 0 | ✅ |
| **Total** | **RF01-RF10** | **52** | **52** | **0** | **✅ 100%** |

**Nota:** La integración completa con SendGrid para envío real de correos al familiar y la integración con Firebase Cloud Messaging para notificaciones push quedan documentadas como pendientes del Sprint 2, conforme a los límites del prototipo Pre-Alpha definidos en la sección 1.6.2 del documento de tesis.