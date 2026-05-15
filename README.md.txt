Sistema de Venta de Entradas

Descripción

Sistema web de venta de entradas desarrollado para gestionar eventos, selección de asientos y compra de tickets, garantizando la integridad de los datos bajo concurrencia.

El sistema permite a usuarios registrarse, iniciar sesión, reservar asientos temporalmente y realizar compras, mientras que los administradores pueden gestionar eventos y visualizar auditorías del sistema.

---

Tecnologías utilizadas

Backend: ASP.NET Core Web API
ORM: Entity Framework Core
Base de datos: SQLite
Frontend: HTML, CSS y JavaScript Vanilla
Documentación: Swagger / OpenAPI

---

Funcionalidades implementadas

Usuarios:
Registro de usuarios
Inicio de sesión
Validación de credenciales
Roles (Admin / Usuario)
Validación de usuarios y correos duplicados

Eventos:
Creación de eventos
Edición de eventos
Eliminación de eventos
Carga de imágenes
Visualización de eventos disponibles
Visualización de stock disponible

Asientos:
Visualización de mapa de asientos
Reserva temporal de asientos
Liberación automática luego de 5 minutos
Cancelación de reservas
Compra de entradas
Prevención de doble compra

Tickets:
Generación de ticket visual
QR simulado
Historial de entradas por usuario
Visualización de tickets comprados

Auditoría:
Registro de reservas
Registro de compras
Registro de cancelaciones
Registro de liberaciones automáticas
Filtros por usuario, acción y evento

---

Reglas de negocio implementadas

Un asiento solo puede reservarse si está disponible.
Un asiento reservado no puede ser comprado por otro usuario.
Las reservas expiran automáticamente luego de 5 minutos.
Todas las acciones quedan registradas en auditoría.
No se permiten usuarios duplicados.
No se permiten correos electrónicos repetidos.
Los usuarios solo pueden visualizar sus propias entradas.
---

Concurrencia

El sistema valida el estado actual del asiento antes de reservar o comprar para evitar conflictos de concurrencia y doble asignación.

Si un asiento ya fue reservado o vendido por otro usuario, el sistema devuelve un error y evita la operación.

También se implementó un BackgroundService encargado de liberar reservas vencidas automáticamente.

---

Usuarios de prueba

* Usuario: martin / 123
* Usuario: joaquin / 123
* Usuario: ivanna / 123
* Admin: admin / 1212

---

Como ejecutar el proyecto

1. Clonar repositorio
2. Ejecutar:

dotnet run


3. Abrir en navegador:

http://localhost:5123


4. Swagger:

http://localhost:5123/swagger

---

Responsive Design

La interfaz fue adaptada para dispositivos móviles mediante media queries, optimizando:

cards
tickets
panel administrador
mapa de asientos

---

## Autor

Proyecto desarrollado por Martin Piaggi para la materia Proyecto de Software
