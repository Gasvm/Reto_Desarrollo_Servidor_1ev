using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class TarjetaCreditoService : ITarjetaCreditoService
    {
        private readonly ITarjetaCreditoRepository _tarjetaCreditoRepository;
        private readonly IClienteRepository _clienteRepository; //Añadido para validación de cliente en la asignación de tarjeta

        public bool validarConAlgLuhn = false; //Candidato a configuración por variables de entorno

        public TarjetaCreditoService(ITarjetaCreditoRepository tarjetaCreditoRepository, IClienteRepository clienteRepository)
        {
            _tarjetaCreditoRepository = tarjetaCreditoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filters)
        {
            return await _tarjetaCreditoRepository.GetAllAsync(filters);
        }

        public async Task<TarjetaCredito?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _tarjetaCreditoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TarjetaCredito tarjetaCredito)
        {
            // Validar descripción
            if (string.IsNullOrWhiteSpace(tarjetaCredito.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            
            // Validar número de tarjeta
            ValidarNumeroTarjeta(tarjetaCredito.numeroTarjeta);
            
            // Validar fecha de caducidad
            if (tarjetaCredito.fechaCaducidad.HasValue)
            {
                if (tarjetaCredito.fechaCaducidad.Value.Date < DateTime.Now.Date)
                    throw new ArgumentException("La fecha de caducidad no puede ser anterior a la fecha actual.");
            }
            else
            {
                throw new ArgumentException("La fecha de caducidad es obligatoria.");
            }

            // Validar que el cliente existe y está activo
            if (!tarjetaCredito.idCliente.HasValue || tarjetaCredito.idCliente.Value <= 0)
                throw new ArgumentException("Debe especificar un cliente válido.");

            var clienteExiste = await _clienteRepository.GetByIdAsync(tarjetaCredito.idCliente.Value);
            if (clienteExiste == null)
                throw new ArgumentException($"El cliente con ID {tarjetaCredito.idCliente.Value} no existe.");

            if (!clienteExiste.activo)
                throw new ArgumentException($"El cliente con ID {tarjetaCredito.idCliente.Value} está inactivo.");

            await _tarjetaCreditoRepository.AddAsync(tarjetaCredito);
        }

        public async Task UpdateAsync(TarjetaCredito tarjetaCredito)
        {
            if (tarjetaCredito.idTarjetaCredito <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");

            // Validar descripción
            if (string.IsNullOrWhiteSpace(tarjetaCredito.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");

            // Validar número de tarjeta
            ValidarNumeroTarjeta(tarjetaCredito.numeroTarjeta);
            
            // Validar fecha de caducidad
            if (tarjetaCredito.fechaCaducidad.HasValue)
            {
                if (tarjetaCredito.fechaCaducidad.Value.Date < DateTime.Now.Date)
                    throw new ArgumentException("La fecha de caducidad no puede ser anterior a la fecha actual.");
            }
            else
            {
                throw new ArgumentException("La fecha de caducidad es obligatoria.");
            }

            // Validar que el cliente existe y está activo
            if (!tarjetaCredito.idCliente.HasValue || tarjetaCredito.idCliente.Value <= 0)
                throw new ArgumentException("Debe especificar un cliente válido.");

            var clienteExiste = await _clienteRepository.GetByIdAsync(tarjetaCredito.idCliente.Value);
            if (clienteExiste == null)
                throw new ArgumentException($"El cliente con ID {tarjetaCredito.idCliente.Value} no existe.");

            if (!clienteExiste.activo)
                throw new ArgumentException($"El cliente con ID {tarjetaCredito.idCliente.Value} está inactivo.");

            await _tarjetaCreditoRepository.UpdateAsync(tarjetaCredito);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _tarjetaCreditoRepository.DeleteAsync(id);
        }

        //Métodos para DTOs
        public async Task<List<TarjetaCreditoResponseDTO>> GetAllDTOAsync(QueryParamsFilters? filters)
        {
            var tarjetas = await _tarjetaCreditoRepository.GetAllAsync(filters);
            var tarjetasDTO = new List<TarjetaCreditoResponseDTO>();

            foreach (var tarjeta in tarjetas)
            {
                var cliente = await _clienteRepository.GetByIdAsync(tarjeta.idCliente ?? 0);
                
                tarjetasDTO.Add(new TarjetaCreditoResponseDTO
                {
                    IdTarjetaCredito = tarjeta.idTarjetaCredito ?? 0,
                    Descripcion = tarjeta.descripcion,
                    NumeroTarjetaEnmascarado = EnmascararNumeroTarjeta(tarjeta.numeroTarjeta),
                    FechaCaducidad = tarjeta.fechaCaducidad ?? DateTime.Now,
                    IdCliente = tarjeta.idCliente ?? 0,
                    NombreCliente = cliente?.nombre ?? "N/A",
                    ApellidosCliente = cliente?.apellidos ?? "N/A",
                    Activo = tarjeta.activo
                });
            }

            return tarjetasDTO;
        }

        public async Task<TarjetaCreditoResponseDTO?> GetByIdDTOAsync(int id)
        {
            var tarjeta = await _tarjetaCreditoRepository.GetByIdAsync(id);
            if (tarjeta == null) return null;

            var cliente = await _clienteRepository.GetByIdAsync(tarjeta.idCliente ?? 0);

            return new TarjetaCreditoResponseDTO
            {
                IdTarjetaCredito = tarjeta.idTarjetaCredito ?? 0,
                Descripcion = tarjeta.descripcion,
                NumeroTarjetaEnmascarado = EnmascararNumeroTarjeta(tarjeta.numeroTarjeta),
                FechaCaducidad = tarjeta.fechaCaducidad ?? DateTime.Now,
                IdCliente = tarjeta.idCliente ?? 0,
                NombreCliente = cliente?.nombre ?? "N/A",
                ApellidosCliente = cliente?.apellidos ?? "N/A",
                Activo = tarjeta.activo
            };
        }

        // Método helper para enmascarar número de tarjeta (oculta todos los dígitos excepto los últimos 4)
        private string EnmascararNumeroTarjeta(string? numeroTarjeta)
        {
            if (string.IsNullOrEmpty(numeroTarjeta))
                return "****";

            // Eliminar espacios y guiones para trabajar solo con dígitos
            var numeroLimpio = numeroTarjeta.Replace(" ", "").Replace("-", "");

            if (numeroLimpio.Length < 4)
                return "****";

            return "****" + numeroLimpio.Substring(numeroLimpio.Length - 4);
        }


        // Método privado para validar número de tarjeta
        private void ValidarNumeroTarjeta(string? numeroTarjeta)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta))
                throw new ArgumentException("El número de tarjeta no puede estar vacío.");

            // Eliminar espacios y guiones
            var numeroLimpio = numeroTarjeta.Replace(" ", "").Replace("-", "");

            // Validar que solo contiene dígitos
            if (!numeroLimpio.All(char.IsDigit))
                throw new ArgumentException("El número de tarjeta solo puede contener dígitos.");

            // Validar longitud (las tarjetas reales tienen entre 13 y 19 dígitos)
            if (numeroLimpio.Length < 13 || numeroLimpio.Length > 19)
                throw new ArgumentException("El número de tarjeta debe tener entre 13 y 19 dígitos.");

            // Opcional: Validar con algoritmo de Luhn
            if(validarConAlgLuhn)
                if (!ValidarAlgoritmoLuhn(numeroLimpio))
                    throw new ArgumentException("El número de tarjeta no es válido según el algoritmo de Luhn.");
        }

        // Algoritmo de Luhn para validar números de tarjeta
        private bool ValidarAlgoritmoLuhn(string numero)
        {
            int suma = 0;
            bool alternar = false;

            // Recorrer de derecha a izquierda
            for (int i = numero.Length - 1; i >= 0; i--)
            {
                int digito = int.Parse(numero[i].ToString());

                if (alternar)
                {
                    digito *= 2;
                    if (digito > 9)
                        digito -= 9;
                }

                suma += digito;
                alternar = !alternar;
            }

            return (suma % 10) == 0;
        }
    }
}