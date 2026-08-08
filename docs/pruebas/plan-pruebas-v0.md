# Plan de Pruebas Preliminar v0 — Sistema Móvil de Monitoreo y Confirmación de Medicación

---

## 1. Objetivo del Plan de Pruebas
Definir los tipos de prueba, módulos a evaluar, herramientas y criterios de aceptación que guiarán el proceso de validación del Sistema Móvil de Monitoreo y Confirmación de Medicación durante todas las fases de desarrollo del proyecto.

## 2. Alcance
El plan de pruebas cubre los siete módulos del sistema: 
1. Autenticación
2. Gestión de medicamentos y horarios
3. Sistema de alarmas
4. Confirmación de toma
5. Notificaciones al familiar
6. Historial de medicación
7. Panel del familiar

---

## 3. Tipos de Prueba

### 3.1 Pruebas Funcionales
Verifican que cada módulo del sistema realiza correctamente las funciones definidas en los requerimientos funcionales de PG1. Se ejecutan sobre cada endpoint del backend y sobre cada pantalla de la app móvil.
- **Herramientas:** Postman para endpoints del backend. Emulador Android y dispositivo físico para la app móvil.

### 3.2 Pruebas de Integración
Verifican que los componentes del sistema se comunican correctamente entre sí: app móvil con backend vía API REST, backend con base de datos en Railway, y backend con servicios externos (SendGrid y Firebase).
- **Herramientas:** Postman para verificar respuestas de la API. Visual Studio para depuración de errores de integración.

### 3.3 Pruebas de Usabilidad
Verifican que la interfaz del sistema es accesible, intuitiva y funcional para adultos mayores con limitaciones en el uso de tecnología. Se aplican con usuarios reales representativos del grupo objetivo.
- **Herramientas:** Sesiones presenciales con adultos mayores del municipio de Chiquimulilla. Guía de tareas estructurada. Registro de observaciones.

---

## 4. Criterios de Aceptación por Módulo

### Módulo 1 — Autenticación
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Registro de usuario exitoso** | Usuario creado en base de datos con contraseña encriptada. |
| **Login con credenciales correctas** | Token JWT generado y sesión iniciada. |
| **Login con credenciales incorrectas** | Mensaje de error claro sin acceso al sistema. |
| **Cierre de sesión** | Token invalidado y redirección a pantalla de login. |

### Módulo 2 — Gestión de Medicamentos y Horarios
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Registro de medicamento** | Medicamento almacenado en BD con todos sus campos. |
| **Programación de horario** | Horario registrado y alarma local programada en el dispositivo. |
| **Edición de medicamento** | Datos actualizados en BD y alarma reprogramada. |
| **Eliminación de medicamento** | Registro eliminado y alarma cancelada. |

### Módulo 3 — Sistema de Alarmas y Recordatorios
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Alarma a la hora programada** | Notificación visible en el dispositivo con sonido y vibración. |
| **Alarma con app en segundo plano** | Alarma se activa correctamente sin necesidad de abrir la app. |
| **Alarma con app cerrada** | Alarma se activa correctamente aunque la app no esté en ejecución. |
| **Pantalla de confirmación** | Se abre automáticamente al tocar la notificación. |

### Módulo 4 — Confirmación de Toma
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Confirmación exitosa** | Registro almacenado en BD con fecha y hora exacta. |
| **Pantalla de éxito** | Mensaje verde de confirmación visible al adulto mayor. |
| **Cancelación del temporizador** | Evento de alerta cancelado tras registrar confirmación. |
| **Tiempo de respuesta** | Confirmación registrada en menos de 3 segundos. |

### Módulo 5 — Notificaciones al Familiar
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Detección de incumplimiento** | Sistema detecta ausencia de confirmación al vencer 30 minutos. |
| **Envío de correo al familiar** | Correo recibido en menos de 5 minutos tras vencer el plazo. |
| **Registro de alerta en BD** | Alerta almacenada con fecha, hora y medicamento asociado. |
| **Contenido del correo** | Incluye nombre del medicamento, dosis y hora programada. |

### Módulo 6 — Historial de Medicación
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Consulta por período** | Registros filtrados correctamente por semana o mes. |
| **Indicadores visuales** | Verde para confirmado, ámbar para pendiente, rojo para alerta. |
| **Datos correctos** | Historial refleja exactamente los registros de la base de datos. |
| **Tiempo de carga** | Historial carga en menos de 3 segundos. |

### Módulo 7 — Panel del Familiar
| Criterio | Resultado Esperado |
| :--- | :--- |
| **Porcentaje de cumplimiento** | Cálculo correcto basado en confirmaciones vs medicamentos programados. |
| **Indicador de color** | Verde (>80%), ámbar (50-80%), rojo (<50%). |
| **Últimas actividades** | Muestra correctamente las últimas confirmaciones y alertas. |
| **Acceso al historial completo** | Redirige correctamente al historial detallado. |

---

## 5. Herramientas Definidas

| Herramienta | Uso |
| :--- | :--- |
| **Postman** | Pruebas funcionales de endpoints de la API REST. |
| **Emulador Android (AVD)** | Pruebas de la app móvil durante el desarrollo. |
| **Dispositivo físico Android** | Pruebas de alarmas y notificaciones en condiciones reales. |
| **Visual Studio 2026** | Depuración de errores de backend y app móvil. |
| **Railway** | Verificación de datos almacenados en base de datos en nube. |
| **SendGrid** | Verificación de correos enviados al familiar. |

---

## 6. Cronograma de Pruebas

| Fase | Tipo de Prueba | Período |
| :--- | :--- | :--- |
| **Sprint 1** | Funcionales: autenticación y medicamentos | 5 de septiembre de 2026 |
| **Sprint 2** | Funcionales e integración: alarmas, confirmación, notificaciones | 19 de septiembre de 2026 |
| **Fase 8** | Usabilidad con adultos mayores | 30 de septiembre de 2026 |
| **Fase 8** | Pruebas técnicas integrales | 1 de octubre de 2026 |

---

## 7. Responsable
**Javier José Luis Rivera Pérez**  
Ingeniería en Sistemas de Información — Universidad Mariano Gálvez de Guatemala  
Proyecto de Graduación 2 — 2026
