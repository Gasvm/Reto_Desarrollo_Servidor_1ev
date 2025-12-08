using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;
using Reto_Desarrollo_Servidor_1ev.Repositories;
using Reto_Desarrollo_Servidor_1ev.Services.DTOs;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class PedidoCabService : IPedidoCabService
    {
        private readonly IPedidoCabRepository _pedidoCabRepository;
        private readonly IPedidoLinRepository _pedidoLinRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMedioDePagoRepository _medioDePagoRepository;
        private readonly ITarjetaCreditoRepository _tarjetaCreditoRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ITipoIVARepository _tipoIVARepository;

        public PedidoCabService(
            IPedidoCabRepository pedidoCabRepository,
            IPedidoLinRepository pedidoLinRepository,
            IClienteRepository clienteRepository,
            IMedioDePagoRepository medioDePagoRepository,
            ITarjetaCreditoRepository tarjetaCreditoRepository,
            IProductoRepository productoRepository,
            ITipoIVARepository tipoIVARepository)
        {
            _pedidoCabRepository = pedidoCabRepository;
            _pedidoLinRepository = pedidoLinRepository;
            _clienteRepository = clienteRepository;
            _medioDePagoRepository = medioDePagoRepository;
            _tarjetaCreditoRepository = tarjetaCreditoRepository;
            _productoRepository = productoRepository;
            _tipoIVARepository = tipoIVARepository;
        }

        public async Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters)
        {
            return await _pedidoCabRepository.GetAllAsync(filters);
        }

        public async Task<PedidoCab?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _pedidoCabRepository.GetByIdAsync(id);
        }

        public async Task<int> AddAsync(PedidoCab pedidoCab)
        {
            return await _pedidoCabRepository.AddAsync(pedidoCab);
        }

        public async Task UpdateAsync(PedidoCab pedidoCab)
        {
            if (pedidoCab.idPedido <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoCabRepository.UpdateAsync(pedidoCab);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoCabRepository.DeleteAsync(id);
        }

        // ==================== NUEVOS MÉTODOS ====================

        public async Task<PedidoCompletoDTO?> GetPedidoCompletoAsync(int idPedido)
        {
            // 1. Obtener cabecera del pedido
            var pedidoCab = await _pedidoCabRepository.GetByIdAsync(idPedido);
            if (pedidoCab == null)
                return null;

            // 2. Obtener información del cliente
            var cliente = await _clienteRepository.GetByIdAsync(pedidoCab.idCliente ?? 0);
            if (cliente == null)
                throw new InvalidOperationException($"Cliente con ID {pedidoCab.idCliente} no encontrado");

            // 3. Obtener información del medio de pago
            var medioPago = await _medioDePagoRepository.GetByIdAsync(pedidoCab.idMedioPago ?? 0);
            if (medioPago == null)
                throw new InvalidOperationException($"Medio de pago con ID {pedidoCab.idMedioPago} no encontrado");

            // 4. Obtener información de la tarjeta (si existe)
            TarjetaCredito? tarjeta = null;
            if (pedidoCab.idTarjetaCredito.HasValue && pedidoCab.idTarjetaCredito.Value > 0)
            {
                tarjeta = await _tarjetaCreditoRepository.GetByIdAsync(pedidoCab.idTarjetaCredito.Value);
            }

            // 5. Obtener líneas del pedido
            var filtros = new QueryParamsFilters { filtroIdPedido = idPedido };
            var lineasPedido = await _pedidoLinRepository.GetAllAsync(filtros);

            // 6. Construir DTO con líneas detalladas
            var lineasDTO = new List<LineaPedidoDTO>();
            decimal subtotalPedido = 0;
            decimal descuentoTotal = 0;
            decimal baseImponibleTotal = 0;
            decimal ivaTotal = 0;

            foreach (var linea in lineasPedido)
            {
                // Obtener información del producto
                var producto = await _productoRepository.GetByIdAsync(linea.idProducto ?? 0);
                if (producto == null) continue;

                // Obtener información del tipo de IVA
                var tipoIVA = await _tipoIVARepository.GetByIdAsync(linea.idTipoIVA ?? 0);
                if (tipoIVA == null) continue;

                // Calcular valores de la línea
                decimal subtotalLinea = (linea.precio ?? 0) * (linea.cantidad ?? 0);
                decimal descuentoLinea = subtotalLinea * ((linea.descuento ?? 0) / 100);
                decimal baseImponibleLinea = subtotalLinea - descuentoLinea;
                decimal ivaLinea = baseImponibleLinea * ((tipoIVA.tasa ?? 0) / 100);
                decimal totalLinea = baseImponibleLinea + ivaLinea;

                lineasDTO.Add(new LineaPedidoDTO
                {
                    IdLineaPedido = linea.idLineaPedido ?? 0,
                    IdProducto = producto.idProducto ?? 0,
                    DescripcionProducto = producto.descripcion ?? "",
                    PrecioUnitario = linea.precio ?? 0,
                    Cantidad = linea.cantidad ?? 0,
                    Descuento = linea.descuento ?? 0,
                    IdTipoIVA = tipoIVA.idTipoIVA ?? 0,
                    DescripcionTipoIVA = tipoIVA.descripcion ?? "",
                    TasaIVA = tipoIVA.tasa ?? 0,
                    SubtotalLinea = subtotalLinea,
                    DescuentoAplicado = descuentoLinea,
                    BaseImponible = baseImponibleLinea,
                    ImporteIVA = ivaLinea,
                    TotalLinea = totalLinea
                });

                subtotalPedido += subtotalLinea;
                descuentoTotal += descuentoLinea;
                baseImponibleTotal += baseImponibleLinea;
                ivaTotal += ivaLinea;
            }

            // 7. Construir y devolver DTO completo
            return new PedidoCompletoDTO
            {
                IdPedido = pedidoCab.idPedido ?? 0,
                FechaPedido = pedidoCab.fechaPedido ?? DateTime.Now,
                Activo = pedidoCab.activo,
                
                IdCliente = cliente.idCliente ?? 0,
                NombreCliente = cliente.nombre ?? "",
                ApellidosCliente = cliente.apellidos ?? "",
                EmailCliente = cliente.email ?? "",
                TelefonoCliente = cliente.telefono ?? "",
                
                IdMedioPago = medioPago.idMedioDePago ?? 0,
                DescripcionMedioPago = medioPago.descripcion ?? "",
                
                IdTarjetaCredito = tarjeta?.idTarjetaCredito,
                DescripcionTarjeta = tarjeta?.descripcion,
                NumeroTarjetaEnmascarado = tarjeta != null ? EnmascararNumeroTarjeta(tarjeta.numeroTarjeta) : null,
                
                Lineas = lineasDTO,
                
                SubtotalPedido = subtotalPedido,
                DescuentoTotal = descuentoTotal,
                BaseImponibleTotal = baseImponibleTotal,
                IVATotal = ivaTotal,
                TotalPedido = baseImponibleTotal + ivaTotal
            };
        }

        public async Task<int> CreatePedidoCompletoAsync(CreatePedidoCompletoDTO pedidoDTO)
        {
            // 1. Validar que el cliente existe
            var cliente = await _clienteRepository.GetByIdAsync(pedidoDTO.IdCliente);
            if (cliente == null)
                throw new InvalidOperationException($"Cliente con ID {pedidoDTO.IdCliente} no encontrado");

            // 2. Validar que el medio de pago existe
            var medioPago = await _medioDePagoRepository.GetByIdAsync(pedidoDTO.IdMedioPago);
            if (medioPago == null)
                throw new InvalidOperationException($"Medio de pago con ID {pedidoDTO.IdMedioPago} no encontrado");

            // 3. Validar tarjeta si se proporciona
            if (pedidoDTO.IdTarjetaCredito.HasValue)
            {
                var tarjeta = await _tarjetaCreditoRepository.GetByIdAsync(pedidoDTO.IdTarjetaCredito.Value);
                if (tarjeta == null)
                    throw new InvalidOperationException($"Tarjeta con ID {pedidoDTO.IdTarjetaCredito} no encontrada");
                
                // Validar que la tarjeta pertenece al cliente
                if (tarjeta.idCliente != pedidoDTO.IdCliente)
                    throw new InvalidOperationException("La tarjeta no pertenece al cliente especificado");
            }

            // 4. Validar que hay líneas en el pedido
            if (pedidoDTO.Lineas == null || pedidoDTO.Lineas.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos una línea");

            // 5. Validar productos y calcular totales
            foreach (var lineaDTO in pedidoDTO.Lineas)
            {
                var producto = await _productoRepository.GetByIdAsync(lineaDTO.IdProducto);
                if (producto == null)
                    throw new InvalidOperationException($"Producto con ID {lineaDTO.IdProducto} no encontrado");
                
                if (lineaDTO.Cantidad <= 0)
                    throw new ArgumentException($"La cantidad debe ser mayor a 0 para el producto {lineaDTO.IdProducto}");
            }

            // 6. Crear cabecera del pedido
            var pedidoCab = new PedidoCab
            {
                idCliente = pedidoDTO.IdCliente,
                fechaPedido = DateTime.Now,
                idMedioPago = pedidoDTO.IdMedioPago,
                idTarjetaCredito = pedidoDTO.IdTarjetaCredito,
                activo = true
            };

            int idPedidoGenerado = await _pedidoCabRepository.AddAsync(pedidoCab);

            // 7. Crear líneas del pedido
            foreach (var lineaDTO in pedidoDTO.Lineas)
            {
                var producto = await _productoRepository.GetByIdAsync(lineaDTO.IdProducto);
                
                decimal subtotal = (producto.precio ?? 0) * lineaDTO.Cantidad;
                decimal descuento = subtotal * (lineaDTO.Descuento / 100);
                decimal baseImponible = subtotal - descuento;
                
                var tipoIVA = await _tipoIVARepository.GetByIdAsync(producto.idTipoIVA ?? 0);
                decimal iva = baseImponible * ((tipoIVA?.tasa ?? 0) / 100);
                decimal totalLinea = baseImponible + iva;

                var pedidoLin = new PedidoLin
                {
                    idPedido = idPedidoGenerado,
                    idProducto = lineaDTO.IdProducto,
                    precio = producto.precio,
                    descuento = lineaDTO.Descuento,
                    idTipoIVA = producto.idTipoIVA,
                    cantidad = lineaDTO.Cantidad,
                    totalLinea = totalLinea,
                    activo = true
                };

                await _pedidoLinRepository.AddAsync(pedidoLin);
            }

            return idPedidoGenerado;
        }

        // Método auxiliar para enmascarar número de tarjeta
        private string EnmascararNumeroTarjeta(string? numeroTarjeta)
        {
            if (string.IsNullOrEmpty(numeroTarjeta))
                return "****";

            var numeroLimpio = numeroTarjeta.Replace(" ", "").Replace("-", "");
            
            if (numeroLimpio.Length < 4)
                return "****";

            return "****" + numeroLimpio.Substring(numeroLimpio.Length - 4);
        }
    }
}