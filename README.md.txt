Sistema de Venta de Entradas

Descripción

Sistema de venta de entradas desarrollado para gestionar eventos, selección de asientos y compras, garantizando la integridad de los datos bajo concurrencia.

---

Tecnologías utilizadas

* Backend: ASP.NET Core + Entity Framework Core
* Base de datos: SQLite
* Frontend: HTML, CSS, JavaScript (Vanilla)
* Documentación: Swagger (OpenAPI)

---

Funcionalidades (Entrega 1)

* Listado de eventos
* Visualización de mapa de asientos
* Reserva de asientos (bloqueo temporal)
* Cancelación de reservas
* Compra de entradas
* Auditoría de acciones
* Sistema de login / registro
* Roles (Admin / Usuario)

---

Reglas de negocio implementadas

* Un asiento solo puede reservarse si está disponible
* Un asiento reservado no puede ser comprado por otro usuario
* Las reservas expiran automáticamente luego de 5 minutos
* Todas las acciones quedan registradas en auditoría

---

Concurrencia

Se valida el estado del asiento antes de reservar para evitar doble asignación.
Si un asiento ya no está disponible, el sistema devuelve error.

---

Usuarios de prueba

* Usuario: Martin / 123
* Usuario: Joaquin / 123
* Usuario: Ivanna / 123
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

Notas

* La logica de negocio se encuentra actualmente en los controladores.
* Se implementará control de concurrencia con Optimistic Locking.

---

## 👨‍💻 Autor

Proyecto desarrollado por Martin Piaggi para la materia Proyecto de Software
