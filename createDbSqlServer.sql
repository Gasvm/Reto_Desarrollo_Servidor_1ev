-- Crear base de datos
CREATE DATABASE SistemaPedidosDB;

-- Verificar que la base de datos se creó correctamente
SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'SistemaPedidosDB';

-- Usar la base de datos
USE SistemaPedidosDB;

-- ================================
-- CREAR TABLAS (Orden: Master -> Detail)
-- ================================

-- Tabla tbTipoIVA (Tabla maestra - sin dependencias)
CREATE TABLE tbTipoIVA (
    idTipoIVA INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(100) NOT NULL,
    tasa DECIMAL(5,2) NOT NULL CHECK (tasa >= 0),
    fechaCreacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Tabla tbClientes (Tabla maestra - sin dependencias)
CREATE TABLE tbClientes (
    idCliente INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    apellidos NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    telefono NVARCHAR(20),
    fechaCreacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Tabla tbMedioDePago (Tabla maestra - sin dependencias)
CREATE TABLE tbMedioDePago (
    idMedioDePago INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(100) NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1
);

-- Tabla tbTarjetaCredito (Detalle de Cliente)
CREATE TABLE tbTarjetaCredito (
    idTarjetaCredito INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(100) NOT NULL,
    numeroTarjeta NVARCHAR(19) NOT NULL,
    fechaCaducidad DATETIME,
    idCliente INT NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1,
    FOREIGN KEY (idCliente) REFERENCES tbClientes(idCliente) ON DELETE CASCADE
);

-- Tabla tbProducto (Con referencia a TipoIVA)
CREATE TABLE tbProductos (
    idProducto INT IDENTITY(1,1) PRIMARY KEY,
    descripcion NVARCHAR(200) NOT NULL,
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    idTipoIVA INT NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    activo BIT DEFAULT 1,
    FOREIGN KEY (idTipoIVA) REFERENCES tbTipoIVA(idTipoIVA)
);

-- Tabla tbPedidoCab (Cabecera de Pedido)
CREATE TABLE tbPedidoCab (
    idPedido INT IDENTITY(1,1) PRIMARY KEY,
    idCliente INT NOT NULL,
    fechaPedido DATETIME DEFAULT GETDATE(),
    idMedioPago INT NOT NULL,
    idTarjetaCredito INT,
    activo BIT DEFAULT 1,
    FOREIGN KEY (idCliente) REFERENCES tbClientes(idCliente),
    FOREIGN KEY (idMedioPago) REFERENCES tbMedioDePago(idMedioDePago),
    FOREIGN KEY (idTarjetaCredito) REFERENCES tbTarjetaCredito(idTarjetaCredito)
);

-- Tabla tbPedidoLin (Línea de Pedido - Detalle)
CREATE TABLE tbPedidoLin (
    idLineaPedido INT IDENTITY(1,1) PRIMARY KEY,
    idPedido INT NOT NULL,
    idProducto INT NOT NULL,
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    descuento DECIMAL(5,2) DEFAULT 0 CHECK (descuento >= 0 AND descuento <= 100),
    idTipoIVA INT NOT NULL,
    cantidad INT NOT NULL CHECK (cantidad > 0),
    totalLinea DECIMAL(10,2),
    activo BIT DEFAULT 1,
    FOREIGN KEY (idPedido) REFERENCES tbPedidoCab(idPedido) ON DELETE CASCADE,
    FOREIGN KEY (idProducto) REFERENCES tbProductos(idProducto),
    FOREIGN KEY (idTipoIVA) REFERENCES tbTipoIVA(idTipoIVA)
);

-- ================================
-- INSERTAR DATOS DE EJEMPLO
-- ================================

-- Insertar Tipos de IVA (primero, ya que otros dependen)
INSERT INTO tbTipoIVA (descripcion, tasa, activo)
VALUES 
('IVA General (21%)', 21.00, 1),
('IVA Reducido (10%)', 10.00, 1),
('IVA Superreducido (4%)', 4.00, 1),
('Sin IVA (0%)', 0.00, 1);

-- Insertar Medios de Pago
INSERT INTO tbMedioDePago (descripcion, activo)
VALUES 
('Tarjeta de Crédito', 1),
('Tarjeta de Débito', 1),
('Efectivo', 1),
('Transferencia Bancaria', 1),
('PayPal', 1);

-- Insertar Clientes
INSERT INTO tbClientes (nombre, apellidos, email, password, telefono, fechaCreacion, activo)
VALUES 
('Juan', 'García López', 'juan.garcia@email.com', 'hash_password_123', '600123456', GETDATE(), 1),
('María', 'Martínez Ruiz', 'maria.martinez@email.com', 'hash_password_456', '600789012', GETDATE(), 1),
('Pedro', 'Sánchez Gil', 'pedro.sanchez@email.com', 'hash_password_789', '600345678', GETDATE(), 1),
('Ana', 'López Fernández', 'ana.lopez@email.com', 'hash_password_012', '600901234', GETDATE(), 1),
('Carlos', 'Ruiz Navarro', 'carlos.ruiz@email.com', 'hash_password_345', '600567890', GETDATE(), 1);

-- Insertar Tarjetas de Crédito
INSERT INTO tbTarjetaCredito (descripcion, numeroTarjeta, fechaCaducidad, idCliente, fechaCreacion, activo)
VALUES 
('Visa Gold', '4532123456789012', '2026-12-31', 1, GETDATE(), 1),
('MasterCard', '5425233430109903', '2027-06-30', 2, GETDATE(), 1),
('American Express', '374245455400126', '2025-09-30', 1, GETDATE(), 1),
('Visa Platinum', '4916338506082832', '2028-03-31', 3, GETDATE(), 1),
('Visa Electron', '4024007134432500', '2026-08-31', 4, GETDATE(), 1),
('MasterCard Debit', '5579430014216007', '2027-12-31', 5, GETDATE(), 1);

-- Insertar Productos
INSERT INTO tbProductos (descripcion, precio, idTipoIVA, fechaCreacion, activo)
VALUES 
('Laptop HP 15.6 i7', 799.99, 1, GETDATE(), 1),
('Mouse Inalámbrico Logitech', 25.50, 1, GETDATE(), 1),
('Teclado Mecánico RGB', 89.99, 1, GETDATE(), 1),
('Monitor ASUS 27" 4K', 349.99, 1, GETDATE(), 1),
('Webcam HD 1080p', 45.00, 1, GETDATE(), 1),
('Auriculares Bluetooth', 65.50, 1, GETDATE(), 1),
('Cable USB-C 2m', 12.99, 2, GETDATE(), 1),
('Adaptador HDMI', 9.99, 2, GETDATE(), 1),
('Hub USB 4 puertos', 28.50, 1, GETDATE(), 1),
('Mousepad XL Gaming', 19.99, 2, GETDATE(), 1),
('SSD 500GB', 59.99, 1, GETDATE(), 1),
('Memoria RAM 16GB', 79.99, 1, GETDATE(), 1);

-- Insertar Pedidos (Cabeceras)
INSERT INTO tbPedidoCab (idCliente, fechaPedido, idMedioPago, idTarjetaCredito, activo)
VALUES 
(1, '2024-11-15 10:30:00', 1, 1, 1),
(2, '2024-11-16 14:45:00', 1, 2, 1),
(3, '2024-11-17 09:15:00', 3, NULL, 1),
(1, '2024-11-18 16:20:00', 1, 3, 1),
(4, '2024-11-19 11:00:00', 2, 4, 1),
(5, '2024-11-20 13:30:00', 1, 6, 1),
(2, '2024-11-21 15:45:00', 4, NULL, 1);

-- Insertar Líneas de Pedido
INSERT INTO tbPedidoLin (idPedido, idProducto, precio, descuento, idTipoIVA, cantidad, totalLinea, activo)
VALUES 
-- Pedido 1 (Juan)
(1, 1, 799.99, 5.00, 1, 1, 759.99, 1),
(1, 2, 25.50, 0.00, 1, 2, 51.00, 1),
(1, 5, 45.00, 10.00, 1, 1, 40.50, 1),

-- Pedido 2 (María)
(2, 3, 89.99, 0.00, 1, 1, 89.99, 1),
(2, 4, 349.99, 0.00, 1, 1, 349.99, 1),
(2, 11, 59.99, 15.00, 1, 2, 101.98, 1),

-- Pedido 3 (Pedro - Efectivo)
(3, 6, 65.50, 5.00, 1, 1, 62.23, 1),
(3, 7, 12.99, 0.00, 2, 3, 38.97, 1),
(3, 10, 19.99, 0.00, 2, 1, 19.99, 1),

-- Pedido 4 (Juan - Segunda compra)
(4, 8, 9.99, 0.00, 2, 2, 19.98, 1),
(4, 9, 28.50, 10.00, 1, 1, 25.65, 1),
(4, 12, 79.99, 0.00, 1, 1, 79.99, 1),

-- Pedido 5 (Ana)
(5, 2, 25.50, 0.00, 1, 5, 127.50, 1),
(5, 3, 89.99, 8.00, 1, 1, 82.79, 1),

-- Pedido 6 (Carlos)
(6, 1, 799.99, 10.00, 1, 1, 719.99, 1),
(6, 11, 59.99, 0.00, 1, 1, 59.99, 1),

-- Pedido 7 (María - Segunda compra)
(7, 4, 349.99, 0.00, 1, 1, 349.99, 1),
(7, 12, 79.99, 5.00, 1, 2, 151.98, 1);

-- ================================
-- ÍNDICES ÚTILES PARA CONSULTAS
-- ================================

-- Índice para búsquedas por cliente
CREATE INDEX idx_PedidoCab_Cliente ON tbPedidoCab(idCliente);

-- Índice para búsquedas por producto
CREATE INDEX idx_PedidoLin_Producto ON tbPedidoLin(idProducto);

-- Índice para búsquedas por pedido
CREATE INDEX idx_PedidoLin_Pedido ON tbPedidoLin(idPedido);

-- Índice para búsquedas por tipo IVA
CREATE INDEX idx_Productos_TipoIVA ON tbProductos(idTipoIVA);

-- Índice para búsquedas de clientes activos
CREATE INDEX idx_Clientes_Activos ON tbClientes(activo);

-- ================================
-- CONSULTAS DE EJEMPLO
-- ================================

-- Ver todos los clientes activos
SELECT idCliente, nombre, apellidos, email, telefono, fechaCreacion
FROM tbClientes 
WHERE activo = 1
ORDER BY nombre;

-- Ver todos los productos con su tipo de IVA
SELECT p.idProducto, p.descripcion, p.precio, t.descripcion AS TipoIVA, t.tasa AS TasaIVA
FROM tbProductos p
INNER JOIN tbTipoIVA t ON p.idTipoIVA = t.idTipoIVA
WHERE p.activo = 1
ORDER BY p.descripcion;

-- Ver todos los pedidos con información del cliente
SELECT pc.idPedido, pc.fechaPedido, c.nombre, c.apellidos, COUNT(pl.idLineaPedido) AS NumProductos, SUM(pl.totalLinea) AS TotalPedido
FROM tbPedidoCab pc
INNER JOIN tbClientes c ON pc.idCliente = c.idCliente
LEFT JOIN tbPedidoLin pl ON pc.idPedido = pl.idPedido
WHERE pc.activo = 1
GROUP BY pc.idPedido, pc.fechaPedido, c.nombre, c.apellidos
ORDER BY pc.fechaPedido DESC;

-- Ver tarjetas de crédito de un cliente
SELECT idTarjetaCredito, descripcion, numeroTarjeta, fechaCaducidad
FROM tbTarjetaCredito
WHERE idCliente = 1 AND activo = 1;
