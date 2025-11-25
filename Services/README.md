## NOTAS:

# Services
Este directorio contiene los servicios que implementan la lógica de negocio para la gestión de menús diarios en el restaurante. Cada servicio se encarga de una entidad específica, proporcionando métodos para crear, leer, actualizar y eliminar registros, así como para realizar consultas especializadas.


## Servicios

# BebidaService.cs

El archivo BebidaService.cs implementa la lógica de negocio relacionada con las bebidas del restaurante. Proporciona métodos para gestionar las operaciones CRUD y consultas específicas sobre las bebidas, interactuando con el repositorio correspondiente.

# ComboService.cs

El archivo ComboService.cs maneja la lógica de negocio para los combos de menú ofrecidos por el restaurante. Facilita la gestión de combos, incluyendo la creación, recuperación, actualización y eliminación de registros, interactuando con el repositorio correspondiente.

# MenuDiarioService.cs

El archivo MenuDiarioService.cs es responsable de la lógica de negocio para los menús diarios del restaurante. Proporciona métodos para gestionar los menús diarios, permitiendo realizar operaciones CRUD y consultas específicas basadas en fechas, interactuando con el repositorio correspondiente.

# PlatoPrincipalService.cs

El archivo PlatoPrincipalService.cs gestiona la lógica de negocio para los platos principales del restaurante. Permite realizar operaciones CRUD sobre las entidades de plato principal, facilitando la administración de los platos ofrecidos en el menú diario, interactuando con el repositorio correspondiente.

# PostreService.cs

El archivo PostreService.cs implementa la lógica de negocio relacionada con los postres del restaurante. Proporciona funcionalidades para crear, leer, actualizar y eliminar registros de postres, interactuando con el repositorio correspondiente.


## Interfaces

# IBebidaService.cs

El archivo IBebidaService.cs define la interfaz para el servicio de bebidas, especificando los métodos que deben implementarse para gestionar las operaciones CRUD y consultas relacionadas con las bebidas en el restaurante.

# IComboService.cs

El archivo IComboService.cs establece la interfaz para el servicio de combos, detallando los métodos necesarios para manejar las operaciones CRUD y consultas relacionadas con los combos de menú en el restaurante.

# IMenuDiarioService.cs

El archivo IMenuDiarioService.cs define la interfaz para el servicio de menús diarios, especificando los métodos que deben implementarse para gestionar las operaciones CRUD y consultas relacionadas con los menús diarios en el restaurante.

# IPlatoPrincipalService.cs

El archivo IPlatoPrincipalService.cs establece la interfaz para el servicio de platos principales, detallando los métodos necesarios para manejar las operaciones CRUD y consultas relacionadas con los platos principales en el restaurante.

# IPostreService.cs

El archivo IPostreService.cs define la interfaz para el servicio de postres, especificando los métodos que deben implementarse para gestionar las operaciones CRUD y consultas relacionadas con los postres en el restaurante.
