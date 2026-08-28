# Diccionario de Datos
## Sistema Móvil de Monitoreo y Confirmación de Medicación
**Universidad Mariano Gálvez de Guatemala**
**Autor:** Javier José Luis Rivera Pérez
**Versión:** 1.0 | Fecha: agosto 2026

---

## Entidad: Usuario

Entidad raíz del sistema. Almacena la información de todos los actores: adultos mayores, familiares y administradores.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idUsuario | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único del usuario |
| nombre | VARCHAR | 100 | NOT NULL | — | Nombre del usuario |
| apellido | VARCHAR | 100 | NOT NULL | — | Apellido del usuario |
| correo | VARCHAR | 150 | NOT NULL, UNIQUE | — | Correo electrónico utilizado para autenticación |
| contrasenaHash | VARCHAR | 255 | NOT NULL | — | Contraseña encriptada mediante bcrypt |
| rol | VARCHAR | 20 | NOT NULL, CHECK | — | Rol del usuario: AdultoMayor, Familiar o Administrador |
| activo | BIT | — | NOT NULL | 1 | Estado del usuario: 1 activo, 0 inactivo |
| fechaRegistro | DATETIME | — | NOT NULL | GETDATE() | Fecha y hora de registro en el sistema |

**Restricciones adicionales:**
- `CHECK (rol IN ('AdultoMayor', 'Familiar', 'Administrador'))`
- `UNIQUE (correo)`

---

## Entidad: Medicamento

Almacena los medicamentos registrados para cada adulto mayor con su información de dosificación.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idMedicamento | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único del medicamento |
| idUsuario | INT | — | FK → Usuario, NOT NULL | — | Referencia al adulto mayor propietario del medicamento |
| nombre | VARCHAR | 150 | NOT NULL | — | Nombre comercial o genérico del medicamento |
| dosis | DECIMAL | (8,2) | NOT NULL, CHECK | — | Cantidad de la dosis a administrar |
| unidad | VARCHAR | 30 | NOT NULL | — | Unidad de medida: mg, ml, tabletas, etc. |
| frecuencia | VARCHAR | 50 | NOT NULL | — | Frecuencia de administración: una vez al día, dos veces, etc. |
| notas | VARCHAR | 300 | NULL | NULL | Instrucciones adicionales opcionales |
| activo | BIT | — | NOT NULL | 1 | Estado del medicamento: 1 activo, 0 eliminado |
| fechaCreacion | DATETIME | — | NOT NULL | GETDATE() | Fecha y hora de registro del medicamento |

**Restricciones adicionales:**
- `CHECK (dosis > 0)`
- `FK: idUsuario REFERENCES Usuario(idUsuario) ON DELETE CASCADE`

---

## Entidad: Horario

Define los momentos exactos de administración de cada medicamento. Sirve como base para la programación de alarmas locales en el dispositivo del adulto mayor.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idHorario | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único del horario |
| idMedicamento | INT | — | FK → Medicamento, NOT NULL | — | Referencia al medicamento asociado al horario |
| horaAdministracion | TIME | — | NOT NULL | — | Hora exacta en que debe administrarse el medicamento |
| activo | BIT | — | NOT NULL | 1 | Estado del horario: 1 activo, 0 cancelado |
| fechaCreacion | DATETIME | — | NOT NULL | GETDATE() | Fecha y hora de creación del horario |

**Restricciones adicionales:**
- `FK: idMedicamento REFERENCES Medicamento(idMedicamento) ON DELETE CASCADE`

---

## Entidad: Confirmacion

Registra cada confirmación de toma realizada por el adulto mayor. Constituye el registro central del cumplimiento terapéutico del paciente.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idConfirmacion | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único de la confirmación |
| idUsuario | INT | — | FK → Usuario, NOT NULL | — | Referencia al adulto mayor que realizó la confirmación |
| idMedicamento | INT | — | FK → Medicamento, NOT NULL | — | Referencia al medicamento confirmado |
| idHorario | INT | — | FK → Horario, NOT NULL | — | Referencia al horario correspondiente a la confirmación |
| fechaConfirmacion | DATE | — | NOT NULL | — | Fecha en que se realizó la confirmación |
| horaConfirmacion | TIME | — | NOT NULL | — | Hora en que se realizó la confirmación |
| timestampExacto | DATETIME | — | NOT NULL | GETDATE() | Fecha y hora exacta del momento de la confirmación |

**Restricciones adicionales:**
- `FK: idUsuario REFERENCES Usuario(idUsuario)`
- `FK: idMedicamento REFERENCES Medicamento(idMedicamento)`
- `FK: idHorario REFERENCES Horario(idHorario)`
- Índice compuesto en `(idHorario, fechaConfirmacion)` para optimizar consultas de verificación

---

## Entidad: Alerta

Registra cada alerta generada por incumplimiento terapéutico y su estado de envío al familiar responsable.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idAlerta | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único de la alerta |
| idUsuario | INT | — | FK → Usuario, NOT NULL | — | Referencia al adulto mayor asociado al incumplimiento |
| idMedicamento | INT | — | FK → Medicamento, NOT NULL | — | Referencia al medicamento no confirmado |
| idHorario | INT | — | FK → Horario, NOT NULL | — | Referencia al horario del incumplimiento |
| horaProgramada | TIME | — | NOT NULL | — | Hora en que debía administrarse el medicamento |
| horaVencimiento | DATETIME | — | NOT NULL | — | Fecha y hora en que venció el período de tolerancia de 30 minutos |
| estado | VARCHAR | 20 | NOT NULL, CHECK | 'pendiente' | Estado de la alerta: pendiente, enviada o error |
| horaEnvio | DATETIME | — | NULL | NULL | Fecha y hora exacta en que se envió la alerta al familiar |
| fechaCreacion | DATETIME | — | NOT NULL | GETDATE() | Fecha y hora de creación del registro de alerta |

**Restricciones adicionales:**
- `CHECK (estado IN ('pendiente', 'enviada', 'error'))`
- `FK: idUsuario REFERENCES Usuario(idUsuario)`
- `FK: idMedicamento REFERENCES Medicamento(idMedicamento)`
- `FK: idHorario REFERENCES Horario(idHorario)`

---

## Entidad: ContactoFamiliar

Almacena la información del familiar o cuidador responsable vinculado a cada adulto mayor para el envío de alertas por correo electrónico.

| Campo | Tipo de dato | Longitud | Restricción | Valor por defecto | Descripción |
|---|---|---|---|---|---|
| idContacto | INT | — | PK, IDENTITY, NOT NULL | Auto-incremental | Identificador único del contacto familiar |
| idUsuario | INT | — | FK → Usuario, NOT NULL, UNIQUE | — | Referencia al adulto mayor al que pertenece el contacto |
| nombreFamiliar | VARCHAR | 150 | NOT NULL | — | Nombre completo del familiar o cuidador responsable |
| correoFamiliar | VARCHAR | 150 | NOT NULL | — | Correo electrónico del familiar para recibir alertas |
| telefonoFamiliar | VARCHAR | 20 | NULL | NULL | Número de teléfono de contacto del familiar |
| activo | BIT | — | NOT NULL | 1 | Estado del contacto: 1 activo, 0 inactivo |

**Restricciones adicionales:**
- `FK: idUsuario REFERENCES Usuario(idUsuario) ON DELETE CASCADE`
- `UNIQUE (idUsuario)` — cada adulto mayor tiene un único familiar responsable

---

## Relaciones entre entidades

| Entidad origen | Cardinalidad | Entidad destino | Descripción |
|---|---|---|---|
| Usuario | 1 — N | Medicamento | Un usuario puede tener registrados múltiples medicamentos |
| Usuario | 1 — 1 | ContactoFamiliar | Un usuario tiene un único familiar responsable vinculado |
| Usuario | 1 — N | Confirmacion | Un usuario puede tener múltiples confirmaciones registradas |
| Usuario | 1 — N | Alerta | Un usuario puede generar múltiples alertas de incumplimiento |
| Medicamento | 1 — N | Horario | Un medicamento puede tener múltiples horarios de administración |
| Horario | 1 — N | Confirmacion | Un horario puede generar múltiples confirmaciones a lo largo del tiempo |
| Horario | 1 — N | Alerta | Un horario puede originar múltiples alertas por incumplimiento |

---

## Índices de optimización

| Índice | Tabla | Campos | Propósito |
|---|---|---|---|
| IX_Medicamento_Usuario | Medicamento | idUsuario | Consultas de medicamentos por usuario |
| IX_Horario_Medicamento | Horario | idMedicamento | Consultas de horarios por medicamento |
| IX_Confirmacion_Usuario_Fecha | Confirmacion | idUsuario, fechaConfirmacion | Consultas de historial por usuario y período |
| IX_Confirmacion_Horario_Fecha | Confirmacion | idHorario, fechaConfirmacion | Verificación de confirmaciones al vencer el temporizador |
| IX_Alerta_Usuario_Estado | Alerta | idUsuario, estado | Consultas de alertas por usuario y estado |
| IX_Alerta_Horario | Alerta | idHorario | Consultas de alertas por horario |
