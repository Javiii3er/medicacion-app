# Reporte de Pruebas — Sprint 1
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Fecha:** septiembre 2026
**Sprint:** 1 — Backend base + Autenticación + Medicamentos + App Móvil

---

## 1. Resumen ejecutivo

El Sprint 1 del proyecto abarcó el desarrollo e integración del backend completo en ASP.NET Core con autenticación JWT, los seis controladores de la API REST, el servicio de verificación de incumplimientos y las pantallas principales de la aplicación móvil en .NET MAUI. Al cierre del sprint se ejecutaron pruebas de integración entre la app móvil, el backend y la base de datos SQL Server, verificando el funcionamiento correcto del flujo principal del sistema.

**Resultado general:** ✅ Sin errores críticos abiertos al cierre del Sprint 1.

---

## 2. Alcance del sprint

| Módulo | Descripción |
|---|---|
| Backend — Auth | Registro de usuarios, login con JWT, perfil |
| Backend — Medicamentos | CRUD completo con horarios |
| Backend — Confirmaciones | Registro, historial, verificación, panel |
| Backend — Alertas | Registro, actualización de estado |
| Backend — Horarios | CRUD completo |
| Backend — ContactoFamiliar | CRUD completo |
| VerificacionService | Detección automática de incumplimientos |
| App MAUI — Login | Pantalla de inicio de sesión conectada al backend |
| App MAUI — Panel familiar | Dashboard con estadísticas y lista de medicamentos |
| App MAUI — Agregar medicamento | Formulario de registro desde la app |

---

## 3. Resultados por módulo

### 3.1 Backend — Autenticación (RF01)

| Prueba | Resultado | Evidencia |
|---|---|---|
| POST /api/Usuario/registro | ✅ HTTP 200 + idUsuario: 1 | Swagger — respuesta verificada |
| POST /api/Usuario/login | ✅ HTTP 200 + token JWT generado | Swagger — token retornado |
| Contraseña almacenada como hash | ✅ Hash BCrypt en tabla Usuario | SSMS — columna contrasenaHash |
| Login con credenciales incorrectas | ✅ HTTP 401 retornado | Swagger — credenciales incorrectas |
| Endpoint protegido sin JWT | ✅ HTTP 401 retornado | Swagger — acceso denegado |

**Estado:** ✅ Completo sin errores

---

### 3.2 Backend — Medicamentos y Horarios (RF02, RF03, RF08)

| Prueba | Resultado | Evidencia |
|---|---|---|
| POST /api/Medicamento (Losartán 50mg) | ✅ HTTP 200 + idMedicamento: 1 | Swagger — respuesta verificada |
| GET /api/Medicamento/usuario/1 | ✅ Lista con medicamentos y horarios | Swagger — lista retornada |
| Registro con horario 08:00 | ✅ INSERT en Medicamento e Horario | Log de SQL Server |
| Registro desde app MAUI (Metformina) | ✅ INSERT confirmado | Log de consola del backend |
| Registro desde app MAUI (Aspirina) | ✅ INSERT confirmado | Log de consola del backend |

**Estado:** ✅ Completo sin errores

---

### 3.3 Backend — Confirmaciones (RF04, RF05, RF07)

| Prueba | Resultado | Evidencia |
|---|---|---|
| POST /api/Confirmacion | ✅ HTTP 200 + idConfirmacion + timestamp | Swagger — respuesta verificada |
| GET /api/Confirmacion/historial/1 | ✅ Lista cronológica retornada | Swagger — datos correctos |
| GET /api/Confirmacion/verificar/1 | ✅ confirmado: true/false según BD | Swagger — estado correcto |
| GET /api/Confirmacion/panel/1 | ✅ Porcentaje + totales retornados | Swagger — datos del panel |

**Estado:** ✅ Completo sin errores

---

### 3.4 Backend — VerificacionService (RF06, RF10)

| Prueba | Resultado | Evidencia |
|---|---|---|
| Servicio inicia al arrancar backend | ✅ Log: "VerificacionService iniciado" | Consola PowerShell |
| Detecta incumplimiento de Losartán | ✅ Alerta generada automáticamente | Log: "Alerta generada: Losartán, 08:00, Usuario 1" |
| Detecta incumplimiento de Metformina | ✅ Alerta generada automáticamente | Log: "Alerta generada: Metformina, 08:00, Usuario 1" |
| No duplica alertas del mismo día | ✅ Sin duplicados en BD | SSMS — tabla Alerta |
| No genera alerta con confirmación | ✅ Omite horario con confirmación | Comportamiento verificado |

**Estado:** ✅ Completo sin errores

---

### 3.5 Backend — Panel familiar (RF09)

| Prueba | Resultado | Evidencia |
|---|---|---|
| Panel retorna 0% cumplimiento | ✅ porcentaje: 0.0, colorIndicador: "rojo" | Swagger — respuesta verificada |
| Panel retorna medicamentos programados | ✅ totalProgramados: 3 | Swagger — conteo correcto |
| Panel retorna alertas generadas | ✅ totalAlertas: 2 | Swagger — conteo correcto |

**Estado:** ✅ Completo sin errores

---

### 3.6 App MAUI — Integración (RF01, RF02, RF09)

| Prueba | Resultado | Evidencia |
|---|---|---|
| Login desde emulador (admin@medicacion.com) | ✅ Navegación a PanelFamiliarPage | Captura de pantalla en docs/evidencias/ |
| Lista de medicamentos visible en panel | ✅ Losartán y otros visibles | Captura de pantalla en docs/evidencias/ |
| Agregar Metformina desde app | ✅ INSERT confirmado en log del backend | Log de consola |
| Cerrar sesión desde app | ✅ Regreso a LoginPage | Comportamiento verificado |
| Error de conexión cuando backend no corre | ✅ Mensaje visible en pantalla | Comportamiento verificado |
| Conexión app → backend → SQL Server | ✅ Flujo completo funcionando | Log de SQL en consola |

**Estado:** ✅ Completo sin errores críticos

---

## 4. Errores identificados y correcciones aplicadas

| # | Error | Causa | Corrección aplicada |
|---|---|---|---|
| E01 | 92 errores de compilación en HelloWorldMAUI | MedicacionAPI dentro de carpeta MAUI causaba conflictos | Exclusión de carpeta en HelloWorldMAUI.csproj |
| E02 | Error de SSL al conectar app con backend | Certificado autofirmado no aceptado por emulador | Cambio a HTTP en puerto 5271 con usesCleartextTraffic |
| E03 | "El archivo está siendo usado" al compilar | Backend corriendo bloqueaba archivos de compilación | Detener backend antes de compilar, reiniciar después |
| E04 | Propiedad duplicada en AlertaController | Estado = "alerta" duplicaba propiedad a.Estado | Renombrado a Tipo = "alerta" |
| E05 | Paquetes NuGet versión 10 incompatibles con .NET 8 | NuGet instaló versión más reciente automáticamente | Especificación de versión 8.0.8 en comandos de instalación |

---

## 5. Pendientes identificados para Sprint 2

| # | Pendiente | Prioridad |
|---|---|---|
| P01 | Integración real con SendGrid para envío de correos | Alta |
| P02 | Integración con Firebase Cloud Messaging (push) | Media |
| P03 | Pantalla de Registro de usuario en app MAUI | Alta |
| P04 | Pantallas Editar y Eliminar medicamento en app | Media |
| P05 | Configuración de Railway + BD en nube | Alta |
| P06 | Verificación formal WCAG 2.1 en pantallas Figma | Baja |

---

## 6. Estado final del Sprint 1

| Criterio | Estado |
|---|---|
| Sin errores críticos abiertos | ✅ Cumplido |
| Autenticación y pantallas base funcionando integradas | ✅ Cumplido |
| Reporte committeado en GitHub | ⏳ Pendiente commit |
| Trello actualizado | ⏳ Pendiente |

**Conclusión:** El Sprint 1 cierra con el sistema funcionando de extremo a extremo: la aplicación móvil se conecta correctamente al backend ASP.NET Core, el cual persiste los datos en SQL Server y detecta incumplimientos automáticamente mediante el VerificacionService. Los pendientes identificados corresponden a funcionalidades de Sprint 2 que no afectan el funcionamiento del prototipo Pre-Alpha.