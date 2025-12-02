using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;
using Reto_Desarrollo_Servidor_1ev.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SistemaPedidosDB");

// Espacio reservado para registrar Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMedioDePagoRepository, MedioDePagoRepository>();
builder.Services.AddScoped<IPedidoCabRepository, PedidoCabRepository>();
builder.Services.AddScoped<IPedidoLinRepository, PedidoLinRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ITarjetaCreditoRepository, TarjetaCreditoRepository>();
builder.Services.AddScoped<ITipoIVARepository, TipoIVARepository>();

// Espacio reservado para registrar Servicios
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IMedioDePagoService, MedioDePagoService>();
builder.Services.AddScoped<IPedidoCabService, PedidoCabService>();
builder.Services.AddScoped<IPedidoLinService, PedidoLinService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ITarjetaCreditoService, TarjetaCreditoService>();
builder.Services.AddScoped<ITipoIVAService, TipoIVAService>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
