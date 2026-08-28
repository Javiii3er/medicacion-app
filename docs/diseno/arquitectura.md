# Arquitectura Técnica
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Versión:** 1.0 | Fecha: agosto 2026

---

## Descripción general

El sistema adopta una arquitectura cliente-servidor organizada en tres capas claramente diferenciadas: capa de presentación, capa de negocio y capa de datos. Cada capa tiene responsabilidades específicas y se comunica con las demás a través de interfaces bien definidas, garantizando la separación de responsabilidades y facilitando el mantenimiento y la escalabilidad del sistema.

---

## Capas de la arquitectura

### Capa de presentación — Cliente (.NET MAUI)

La capa de presentación corresponde a la aplicación móvil desarrollada en .NET MAUI con C#, instalada en el dispositivo Android del adulto mayor y del familiar o cuidador. Esta capa es responsable de presentar la interfaz de usuario, capturar las interacciones del usuario y comunicarse con el backend mediante solicitudes HTTP a la API REST.

**Responsabilidades:**
- Mostrar las pantallas de la aplicación según el rol del usuario autenticado.
- Programar y gestionar las alarmas locales en el dispositivo Android mediante Plugin.LocalNotification.
- Enviar solicitudes HTTP al backend con el token JWT en cada petición.
- Almacenar el token JWT en SecureStorage del dispositivo.
- Presentar indicadores visuales de cumplimiento terapéutico.

**Tecnologías:**
- .NET MAUI con C#
- Plugin.LocalNotification para alarmas locales
- HttpClient para consumo de la API REST
- SecureStorage para almacenamiento seguro del token JWT

---

### Capa de negocio — Backend (ASP.NET Core)

La capa de negocio corresponde al backend desarrollado en ASP.NET Core, desplegado en Railway como infraestructura de nube. Esta capa gestiona toda la lógica del sistema: autenticación, registro de confirmaciones, generación de alertas y envío de notificaciones.

**Responsabilidades:**
- Autenticar usuarios mediante JWT y validar cada solicitud recibida.
- Registrar confirmaciones de toma en la base de datos.
- Detectar incumplimientos al vencer el período de tolerancia de 30 minutos.
- Generar y registrar alertas en la base de datos.
- Enviar correos electrónicos de alerta al familiar mediante SendGrid.
- Enviar notificaciones push al dispositivo del familiar mediante Firebase Cloud Messaging.
- Calcular estadísticas de cumplimiento terapéutico para el panel del familiar.

**Controladores:**
| Controlador | Endpoints principales |
|---|---|
| UsuarioController | POST /api/auth/login, POST /api/auth/registro, GET /api/usuarios/perfil |
| MedicamentoController | GET, POST, PUT, DELETE /api/medicamentos |
| HorarioController | GET, POST, PUT, DELETE /api/horarios |
| ConfirmacionController | POST /api/confirmaciones, GET /api/historial, GET /api/panel |
| AlertaController | GET /api/alertas |
| ContactoFamiliarController | GET, POST, PUT /api/contactofamiliar |

**Servicios internos:**
| Servicio | Responsabilidad |
|---|---|
| JWTService | Generación y validación de tokens JWT |
| VerificacionService | Verificación periódica de confirmaciones al vencer el temporizador |
| NotificacionService | Envío de correos mediante SendGrid y notificaciones push mediante FCM |

**Tecnologías:**
- ASP.NET Core 8
- Entity Framework Core
- JWT Bearer Authentication
- SendGrid para correos electrónicos
- Firebase Cloud Messaging para notificaciones push

---

### Capa de datos — Base de datos (SQL Server)

La capa de datos corresponde a la base de datos Microsoft SQL Server alojada en Railway. Esta capa almacena toda la información del sistema de manera persistente y estructurada.

**Tablas principales:**
| Tabla | Descripción |
|---|---|
| Usuario | Información de todos los actores del sistema |
| Medicamento | Medicamentos registrados por cada adulto mayor |
| Horario | Horarios de administración de cada medicamento |
| Confirmacion | Registros de confirmaciones de toma |
| Alerta | Registros de alertas generadas por incumplimiento |
| ContactoFamiliar | Datos del familiar responsable vinculado al adulto mayor |

**Tecnologías:**
- Microsoft SQL Server Express (local) y Railway (nube)
- Entity Framework Core como ORM

---

## Flujo de comunicación

### Flujo de autenticación
```
App Móvil
  → POST /api/auth/login (correo, contraseña)
  → Backend valida credenciales en BD
  → Backend genera token JWT
  → App almacena token en SecureStorage
  → App redirige al usuario según su rol
```

### Flujo de confirmación de toma
```
Sistema Android activa alarma local
  → App muestra pantalla de confirmación
  → Adulto mayor presiona botón Confirmar
  → App → POST /api/confirmaciones (JWT + datos)
  → Backend valida JWT
  → Backend registra confirmación en BD
  → Backend cancela temporizador de alerta
  → App muestra pantalla de confirmación exitosa
```

### Flujo de alerta por incumplimiento
```
Temporizador de 30 minutos vence sin confirmación
  → VerificacionService consulta Confirmacion en BD
  → Sin confirmación detectada
  → Backend registra alerta en BD (estado: pendiente)
  → Backend consulta ContactoFamiliar
  → NotificacionService → SendGrid → correo al familiar
  → Backend actualiza estado alerta (enviada)
  → App muestra pantalla de toma no confirmada
```

### Flujo de consulta del panel familiar
```
Familiar accede al panel
  → App → GET /api/panel (JWT + mes)
  → Backend consulta Horario, Confirmacion y Alerta en BD (paralelo)
  → Backend calcula porcentaje de cumplimiento
  → Backend retorna estadísticas
  → App muestra panel con indicador circular y últimas actividades
```

---

## Servicios externos

### SendGrid
Servicio de envío de correos electrónicos transaccionales de Twilio. Se utiliza para notificar al familiar responsable cuando el adulto mayor no confirma la toma de su medicamento dentro del período de tolerancia de 30 minutos. El tiempo de entrega es menor a 5 minutos tras generarse la alerta.

### Firebase Cloud Messaging (FCM)
Servicio de mensajería en la nube de Google. Se utiliza de manera complementaria a SendGrid para enviar notificaciones push al dispositivo del familiar, garantizando que reciba la alerta incluso si no tiene acceso al correo electrónico en ese momento. Su implementación es opcional en el prototipo actual.

---

## Diagrama de arquitectura

Ver imagen: `docs/diseno/arquitectura.png`
