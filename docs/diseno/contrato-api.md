# Contrato de API REST
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Versión:** 1.0 | Fecha: septiembre 2026
**Base URL:** `http://localhost:5271` (desarrollo) | Railway (producción)
**Autenticación:** Bearer JWT en header `Authorization`

---

## Convenciones

- Todos los endpoints retornan `Content-Type: application/json`
- Los endpoints protegidos requieren header: `Authorization: Bearer {token}`
- Formato de fechas: `yyyy-MM-dd`
- Formato de horas: `HH:mm`
- Formato de timestamps: `yyyy-MM-ddTHH:mm:ss`

---

## 1. UsuarioController — /api/Usuario

### POST /api/Usuario/registro
Registra un nuevo usuario en el sistema.

**Autenticación:** No requerida

**Request body:**
```json
{
  "nombre": "string",
  "apellido": "string",
  "correo": "string",
  "contrasena": "string",
  "rol": "AdultoMayor | Familiar | Administrador"
}
```

**Response 200:**
```json
{
  "mensaje": "Usuario registrado exitosamente.",
  "idUsuario": 1
}
```

**Response 400:**
```json
{
  "mensaje": "El correo ya está registrado."
}
```

---

### POST /api/Usuario/login
Autentica un usuario y retorna el token JWT.

**Autenticación:** No requerida

**Request body:**
```json
{
  "correo": "string",
  "contrasena": "string"
}
```

**Response 200:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "idUsuario": 1,
  "nombre": "string",
  "apellido": "string",
  "correo": "string",
  "rol": "string"
}
```

**Response 401:**
```json
{
  "mensaje": "Credenciales incorrectas."
}
```

---

### GET /api/Usuario/perfil/{id}
Retorna el perfil del usuario autenticado.

**Autenticación:** Requerida

**Response 200:**
```json
{
  "idUsuario": 1,
  "nombre": "string",
  "apellido": "string",
  "correo": "string",
  "rol": "string"
}
```

**Response 404:**
```json
{
  "mensaje": "Usuario no encontrado."
}
```

---

## 2. MedicamentoController — /api/Medicamento

### GET /api/Medicamento/usuario/{idUsuario}
Retorna todos los medicamentos activos de un usuario con sus horarios.

**Autenticación:** Requerida

**Response 200:**
```json
[
  {
    "idMedicamento": 1,
    "nombre": "Losartán",
    "dosis": 50.00,
    "unidad": "mg",
    "frecuencia": "Una vez al día",
    "notas": "string",
    "horarios": [
      {
        "idHorario": 1,
        "horaAdministracion": "08:00"
      }
    ]
  }
]
```

---

### GET /api/Medicamento/{id}
Retorna un medicamento específico por ID.

**Autenticación:** Requerida

**Response 200:** Objeto medicamento con horarios.
**Response 404:** `{ "mensaje": "Medicamento no encontrado." }`

---

### POST /api/Medicamento
Registra un nuevo medicamento con su horario.

**Autenticación:** Requerida

**Request body:**
```json
{
  "idUsuario": 1,
  "nombre": "Losartán",
  "dosis": 50,
  "unidad": "mg",
  "frecuencia": "Una vez al día",
  "notas": "string (opcional)",
  "horaAdministracion": "08:00"
}
```

**Response 200:**
```json
{
  "mensaje": "Medicamento registrado exitosamente.",
  "idMedicamento": 1
}
```

**Response 400:**
```json
{
  "mensaje": "Ya existe un medicamento con ese nombre para este usuario."
}
```

---

### PUT /api/Medicamento/{id}
Actualiza un medicamento existente y reprograma el horario si cambió.

**Autenticación:** Requerida

**Request body:** Mismo esquema que POST.

**Response 200:** `{ "mensaje": "Medicamento actualizado exitosamente." }`
**Response 404:** `{ "mensaje": "Medicamento no encontrado." }`

---

### DELETE /api/Medicamento/{id}
Elimina un medicamento (soft delete — activo = false).

**Autenticación:** Requerida

**Response 200:** `{ "mensaje": "Medicamento eliminado exitosamente." }`
**Response 404:** `{ "mensaje": "Medicamento no encontrado." }`

---

## 3. HorarioController — /api/Horario

### GET /api/Horario/medicamento/{idMedicamento}
Retorna los horarios activos de un medicamento.

**Autenticación:** Requerida

**Response 200:**
```json
[
  {
    "idHorario": 1,
    "idMedicamento": 1,
    "horaAdministracion": "08:00",
    "activo": true
  }
]
```

---

### GET /api/Horario/usuario/{idUsuario}
Retorna todos los horarios activos del usuario ordenados por hora.

**Autenticación:** Requerida

**Response 200:**
```json
[
  {
    "idHorario": 1,
    "idMedicamento": 1,
    "nombreMedicamento": "Losartán",
    "dosis": "50.00 mg",
    "horaAdministracion": "08:00",
    "activo": true
  }
]
```

---

### POST /api/Horario
Registra un nuevo horario para un medicamento.

**Autenticación:** Requerida

**Request body:**
```json
{
  "idMedicamento": 1,
  "horaAdministracion": "08:00"
}
```

**Response 200:**
```json
{
  "mensaje": "Horario registrado exitosamente.",
  "idHorario": 1,
  "horaAdministracion": "08:00"
}
```

---

### PUT /api/Horario/{id}
Actualiza la hora de administración de un horario existente.

**Autenticación:** Requerida

**Request body:** `{ "idMedicamento": 1, "horaAdministracion": "10:00" }`

**Response 200:** `{ "mensaje": "Horario actualizado exitosamente.", "horaAdministracion": "10:00" }`

---

### DELETE /api/Horario/{id}
Desactiva un horario (soft delete).

**Autenticación:** Requerida

**Response 200:** `{ "mensaje": "Horario eliminado exitosamente." }`

---

## 4. ConfirmacionController — /api/Confirmacion

### POST /api/Confirmacion
Registra la confirmación de toma de un medicamento.

**Autenticación:** Requerida

**Request body:**
```json
{
  "idUsuario": 1,
  "idMedicamento": 1,
  "idHorario": 1
}
```

**Response 200:**
```json
{
  "mensaje": "Confirmación registrada exitosamente.",
  "idConfirmacion": 1,
  "timestamp": "2026-09-03T08:05:00"
}
```

---

### GET /api/Confirmacion/historial/{idUsuario}
Retorna el historial de confirmaciones filtrado por período.

**Autenticación:** Requerida

**Query params:** `inicio` (fecha), `fin` (fecha) — opcionales, default: última semana.

**Response 200:**
```json
[
  {
    "idConfirmacion": 1,
    "idMedicamento": 1,
    "nombreMedicamento": "Losartán",
    "dosis": "50.00 mg",
    "fecha": "2026-09-03",
    "hora": "08:05",
    "timestamp": "2026-09-03T08:05:00",
    "estado": "confirmado"
  }
]
```

---

### GET /api/Confirmacion/verificar/{idHorario}
Verifica si existe confirmación para un horario en la fecha actual.

**Autenticación:** Requerida

**Response 200:**
```json
{
  "confirmado": true,
  "fecha": "2026-09-03"
}
```

---

### GET /api/Confirmacion/panel/{idUsuario}
Retorna el resumen estadístico del panel del familiar.

**Autenticación:** Requerida

**Query params:** `mes` (int), `anio` (int) — opcionales, default: mes actual.

**Response 200:**
```json
{
  "mes": 9,
  "anio": 2026,
  "totalProgramados": 3,
  "totalConfirmaciones": 0,
  "totalAlertas": 2,
  "porcentaje": 0.0,
  "colorIndicador": "rojo",
  "ultimasActividades": []
}
```

---

## 5. AlertaController — /api/Alerta

### POST /api/Alerta
Registra una nueva alerta de incumplimiento.

**Autenticación:** Requerida

**Request body:**
```json
{
  "idUsuario": 1,
  "idMedicamento": 1,
  "idHorario": 1,
  "horaProgramada": "08:00",
  "horaVencimiento": "2026-09-03T08:30:00"
}
```

**Response 200:**
```json
{
  "mensaje": "Alerta registrada exitosamente.",
  "idAlerta": 1
}
```

---

### PUT /api/Alerta/{id}/enviada
Marca una alerta como enviada al familiar.

**Autenticación:** Requerida

**Response 200:**
```json
{
  "mensaje": "Alerta marcada como enviada.",
  "horaEnvio": "2026-09-03T08:32:00"
}
```

---

### PUT /api/Alerta/{id}/error
Marca una alerta con error de envío.

**Autenticación:** Requerida

**Response 200:** `{ "mensaje": "Alerta marcada con error." }`

---

### GET /api/Alerta/usuario/{idUsuario}
Retorna las alertas de un usuario filtradas por período.

**Autenticación:** Requerida

**Query params:** `inicio`, `fin` — opcionales, default: última semana.

**Response 200:**
```json
[
  {
    "idAlerta": 1,
    "idMedicamento": 1,
    "nombreMedicamento": "Losartán",
    "dosis": "50.00 mg",
    "horaProgramada": "08:00",
    "horaVencimiento": "2026-09-03T08:30:00",
    "estado": "pendiente",
    "horaEnvio": null,
    "fechaCreacion": "2026-09-03T08:31:00",
    "tipo": "alerta"
  }
]
```

---

### GET /api/Alerta/pendientes/{idUsuario}
Retorna las alertas pendientes de envío de un usuario.

**Autenticación:** Requerida

**Response 200:** Lista de alertas con estado pendiente.

---

## 6. ContactoFamiliarController — /api/ContactoFamiliar

### GET /api/ContactoFamiliar/{idUsuario}
Retorna el contacto familiar del adulto mayor.

**Autenticación:** Requerida

**Response 200:**
```json
{
  "idContacto": 1,
  "idUsuario": 1,
  "nombreFamiliar": "string",
  "correoFamiliar": "string",
  "telefonoFamiliar": "string",
  "activo": true
}
```

---

### POST /api/ContactoFamiliar
Registra el contacto familiar de un adulto mayor.

**Autenticación:** Requerida

**Request body:**
```json
{
  "idUsuario": 1,
  "nombreFamiliar": "string",
  "correoFamiliar": "string",
  "telefonoFamiliar": "string (opcional)"
}
```

**Response 200:**
```json
{
  "mensaje": "Contacto familiar registrado exitosamente.",
  "idContacto": 1
}
```

**Response 400:** `{ "mensaje": "El usuario ya tiene un contacto familiar registrado. Use PUT para actualizar." }`

---

### PUT /api/ContactoFamiliar/{idUsuario}
Actualiza el contacto familiar de un adulto mayor.

**Autenticación:** Requerida

**Request body:** Mismo esquema que POST.

**Response 200:** `{ "mensaje": "Contacto familiar actualizado exitosamente." }`

---

### DELETE /api/ContactoFamiliar/{idUsuario}
Desactiva el contacto familiar (soft delete).

**Autenticación:** Requerida

**Response 200:** `{ "mensaje": "Contacto familiar eliminado exitosamente." }`

---

## Resumen de endpoints

| Método | Endpoint | Autenticación | Descripción |
|---|---|---|---|
| POST | /api/Usuario/registro | No | Registrar usuario |
| POST | /api/Usuario/login | No | Iniciar sesión |
| GET | /api/Usuario/perfil/{id} | Sí | Perfil del usuario |
| GET | /api/Medicamento/usuario/{id} | Sí | Medicamentos por usuario |
| GET | /api/Medicamento/{id} | Sí | Medicamento por ID |
| POST | /api/Medicamento | Sí | Crear medicamento |
| PUT | /api/Medicamento/{id} | Sí | Actualizar medicamento |
| DELETE | /api/Medicamento/{id} | Sí | Eliminar medicamento |
| GET | /api/Horario/medicamento/{id} | Sí | Horarios por medicamento |
| GET | /api/Horario/usuario/{id} | Sí | Horarios por usuario |
| POST | /api/Horario | Sí | Crear horario |
| PUT | /api/Horario/{id} | Sí | Actualizar horario |
| DELETE | /api/Horario/{id} | Sí | Eliminar horario |
| POST | /api/Confirmacion | Sí | Registrar confirmación |
| GET | /api/Confirmacion/historial/{id} | Sí | Historial de confirmaciones |
| GET | /api/Confirmacion/verificar/{id} | Sí | Verificar confirmación hoy |
| GET | /api/Confirmacion/panel/{id} | Sí | Panel del familiar |
| POST | /api/Alerta | Sí | Crear alerta |
| PUT | /api/Alerta/{id}/enviada | Sí | Marcar alerta enviada |
| PUT | /api/Alerta/{id}/error | Sí | Marcar alerta con error |
| GET | /api/Alerta/usuario/{id} | Sí | Alertas por usuario |
| GET | /api/Alerta/pendientes/{id} | Sí | Alertas pendientes |
| GET | /api/ContactoFamiliar/{id} | Sí | Contacto familiar |
| POST | /api/ContactoFamiliar | Sí | Crear contacto familiar |
| PUT | /api/ContactoFamiliar/{id} | Sí | Actualizar contacto familiar |
| DELETE | /api/ContactoFamiliar/{id} | Sí | Eliminar contacto familiar |

**Total: 26 endpoints**