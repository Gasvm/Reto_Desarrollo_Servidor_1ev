-- Crear base de datos
CREATE DATABASE SistemaPedidosDB;

-- Verificar que la base de datos se creó correctamente
SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'SistemaPedidosDB';

-- Usar la base de datos
USE SistemaPedidosDB;

-- Tabla tb_Clientes
CREATE TABLE tb_Clientes (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    apellidos NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    telefono NVARCHAR(20),
    activo BIT DEFAULT 1
);

-- Tabla tb_Tarjetas_de_Credito
CREATE TABLE tb_Tarjetas_de_Credito (
    id_tarjeta_credito INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    numero NVARCHAR(19) NOT NULL,
    fecha_caducidad DATE NOT NULL,
    id_cliente INT NOT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1,
    FOREIGN KEY (id_cliente) REFERENCES tb_Clientes(id_cliente)
);

-- Tabla tb_Medios_de_pago
CREATE TABLE tb_Medios_de_pago (
    id_medio_pago INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(100) NOT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Tabla tb_Tipos_IVA
CREATE TABLE tb_Tipos_IVA (
    id_tipo_IVA INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(50) NOT NULL,
    tasa DECIMAL(5,2) NOT NULL CHECK (tasa >= 0),
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Tabla tb_Productos
CREATE TABLE tb_Productos (
    id_producto INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(200) NOT NULL,
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    tipo_IVA INT NOT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1,
    FOREIGN KEY (tipo_IVA) REFERENCES tb_Tipos_IVA(id_tipo_IVA)
);

-- Tabla tb_Pedidos
CREATE TABLE tb_Pedidos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL,
    fecha DATE NOT NULL,
    id_medio_pago INT NOT NULL,
    id_tarjeta_credito INT,
    activo BIT DEFAULT 1,
    FOREIGN KEY (id_cliente) REFERENCES tb_Clientes(id_cliente),
    FOREIGN KEY (id_medio_pago) REFERENCES tb_Medios_de_pago(id_medio_pago),
    FOREIGN KEY (id_tarjeta_credito) REFERENCES tb_Tarjetas_de_Credito(id_tarjeta_credito)
);

-- Tabla tb_Lineas_Pedido
CREATE TABLE tb_Lineas_Pedido (
    id_linea_pedido INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido INT NOT NULL,
    id_Producto INT NOT NULL,
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    descuento DECIMAL(5,2) DEFAULT 0 CHECK (descuento >= 0),
    tipo_IVA INT NOT NULL,
    cantidad INT NOT NULL CHECK (cantidad > 0),
    total DECIMAL(10,2) NOT NULL CHECK (total >= 0),
    activo BIT DEFAULT 1,
    FOREIGN KEY (id_pedido) REFERENCES tb_Pedidos(id),
    FOREIGN KEY (id_Producto) REFERENCES tb_Productos(id_producto),
    FOREIGN KEY (tipo_IVA) REFERENCES tb_Tipos_IVA(id_tipo_IVA)
);

--------------------------------
-- INSERTAR DATOS DE EJEMPLO

-- Insertar Clientes
INSERT INTO tb_Clientes (nombre, apellidos, email, password, telefono, activo)
VALUES 
('Juan', 'García López', 'juan.garcia@email.com', 'hash123', '600123456', 1),
('María', 'Martínez Ruiz', 'maria.martinez@email.com', 'hash456', '600789012', 1),
('Pedro', 'Sánchez Gil', 'pedro.sanchez@email.com', 'hash789', '600345678', 1);

-- Insertar Medios de Pago
INSERT INTO tb_Medios_de_pago (descripcion, activo)
VALUES 
('Tarjeta de Crédito', 1),
('Efectivo', 1);

-- Insertar Tipos de IVA
INSERT INTO tb_Tipos_IVA (descripcion, tasa, activo)
VALUES 
('IVA General', 21.00, 1),
('Sin IVA', 0.00, 1);

-- Insertar Tarjetas de Crédito
INSERT INTO tb_Tarjetas_de_Credito (nombre, numero, fecha_caducidad, id_cliente, activo)
VALUES 
('Visa Gold', '4532123456789012', '2026-12-31', 1, 1),
('MasterCard', '5425233430109903', '2027-06-30', 2, 1),
('American Express', '374245455400126', '2025-09-30', 1, 1);

-- Insertar Productos
INSERT INTO tb_Productos (descripcion, precio, tipo_IVA, activo)
VALUES 
('Laptop HP', 599.99, 1, 1),
('Mouse Inalámbrico', 25.50, 1, 1),
('Teclado Mecánico', 89.99, 1, 1),
('Monitor Full HD', 179.99, 1, 1),
('Webcam HD', 45.00, 1, 1),
('Auriculares', 65.50, 1, 1);

-- Insertar Pedidos
INSERT INTO tb_Pedidos (id_cliente, fecha, id_medio_pago, id_tarjeta_credito, activo)
VALUES 
(1, '2024-11-20', 1, 1, 1),
(2, '2024-11-22', 1, 2, 1),
(1, '2024-11-23', 2, NULL, 1);

-- Insertar Líneas de Pedido
INSERT INTO tb_Lineas_Pedido (id_pedido, id_Producto, precio, descuento, tipo_IVA, cantidad, total, activo)
VALUES 
(1, 1, 599.99, 0.10, 1, 1, 539.99, 1),
(1, 2, 25.50, 0.00, 1, 2, 51.00, 1),
(2, 3, 89.99, 0.05, 1, 1, 85.49, 1),
(2, 4, 179.99, 0.00, 1, 1, 179.99, 1),
(3, 5, 45.00, 0.15, 1, 1, 38.25, 1);

--------------------------------
-- CONSULTAS DE EJEMPLO

-- Ver todos los clientes
SELECT * FROM tb_Clientes;

-- Ver todos los productos
SELECT * FROM tb_Productos;

-- Ver todos los pedidos
SELECT * FROM tb_Pedidos;
