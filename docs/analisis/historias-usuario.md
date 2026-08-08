# Historias de Usuario — Sistema Móvil de Monitoreo y Confirmación de Medicación


---

## Actor 1: Adulto mayor

### HU-01: Alarma sonora y visual
- **Como** adulto mayor, 
- **Quiero** recibir una alarma sonora y visual a la hora programada, 
- **Para** recordar que debo tomar mi medicamento sin depender de mi memoria.

### HU-02: Botón de confirmación claro
- **Como** adulto mayor, 
- **Quiero** ver un botón grande y claro en mi pantalla al momento de la alarma, 
- **Para** confirmar fácilmente que ya tomé mi medicamento con una sola acción.

### HU-03: Mensaje de confirmación registrada
- **Como** adulto mayor, 
- **Quiero** ver una confirmación visual en pantalla después de presionar el botón, 
- **Para** tener la certeza de que el sistema registró correctamente mi toma.

### HU-04: Consulta de historial propio
- **Como** adulto mayor, 
- **Quiero** consultar el historial de mis medicamentos tomados, 
- **Para** saber cuáles confirmé y cuáles no en los últimos días.

### HU-05: Funcionamiento en segundo plano
- **Como** adulto mayor, 
- **Quiero** que el sistema funcione aunque la app esté cerrada, 
- **Para** no tener que recordar abrirla antes de cada toma.

---

##  Actor 2: Familiar / Cuidador

### HU-06: Notificación por correo por omisión
- **Como** familiar, 
- **Quiero** recibir una notificación automática por correo electrónico cuando el adulto mayor no confirme su medicamento a tiempo, 
- **Para** poder contactarlo y apoyarlo de forma oportuna.

### HU-07: Monitoreo remoto de historial
- **Como** familiar, 
- **Quiero** consultar el historial de medicación del adulto mayor desde mi dispositivo, 
- **Para** supervisar su cumplimiento terapéutico sin necesidad de estar presente.

### HU-08: Porcentaje de cumplimiento mensual
- **Como** familiar, 
- **Quiero** ver el porcentaje de cumplimiento mensual del adulto mayor en el panel, 
- **Para** identificar rápidamente si está siguiendo correctamente su tratamiento.

### HU-09: Panel de alertas y tomas recientes
- **Como** familiar, 
- **Quiero** ver las últimas confirmaciones y alertas en el panel principal, 
- **Para** tener una visión rápida del estado reciente del adulto mayor sin revisar el historial completo.

---

##  Actor 3: Administrador

### HU-10: Vinculación de usuarios
- **Como** administrador, 
- **Quiero** registrar al adulto mayor y a su familiar responsable en el sistema, 
- **Para** vincularlos y habilitar el mecanismo de monitoreo y notificaciones.

### HU-11: Programación de tratamiento
- **Como** administrador, 
- **Quiero** registrar los medicamentos del adulto mayor con nombre, dosis y horario, 
- **Para** que el sistema programe automáticamente las alarmas correspondientes.

### HU-12: Gestión y edición de prescripciones
- **Como** administrador, 
- **Quiero** editar o eliminar medicamentos y horarios registrados, 
- **Para** mantener actualizado el tratamiento del adulto mayor cuando el médico realice cambios.

### HU-13: Automatización de alertas
- **Como** administrador, 
- **Quiero** que el sistema genere alertas automáticas sin intervención manual, 
- **Para** garantizar que el familiar sea notificado oportunamente ante cualquier incumplimiento.

---

##  Resumen de cobertura

| Historia | Actor | Módulo cubierto |
| :--- | :--- | :--- |
| **HU-01** | Adulto mayor | Alarmas y recordatorios |
| **HU-02** | Adulto mayor | Confirmación de toma |
| **HU-03** | Adulto mayor | Confirmación de toma |
| **HU-04** | Adulto mayor | Historial de medicación |
| **HU-05** | Adulto mayor | Alarmas y recordatorios |
| **HU-06** | Familiar | Notificaciones al familiar |
| **HU-07** | Familiar | Historial de medicación |
| **HU-08** | Familiar | Panel del familiar |
| **HU-09** | Familiar | Panel del familiar |
| **HU-10** | Administrador | Autenticación y registro |
| **HU-11** | Administrador | Gestión de medicamentos |
| **HU-12** | Administrador | Gestión de medicamentos |
| **HU-13** | Administrador | Notificaciones al familiar |

- **Total:** 13 historias de usuario
- **Actores cubiertos:** 3
- **Módulos cubiertos:** 6 de 7
