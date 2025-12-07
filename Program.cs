using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;
using Reto_Desarrollo_Servidor_1ev.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SistemaPedidosDB");
// Nombre de la aplicación - Variable de entorno
var appName = Environment.GetEnvironmentVariable("APP_NAME") ?? "More Than Brows API";

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
// Configura swagger para mostrar el nombre dinámico en la documentación
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = appName,
        Version = "v1" 
    });
});

/* Configuración CORS para permitir solicitudes desde el front-end */
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("PermitirTodo");

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
