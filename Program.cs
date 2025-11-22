using Reto_Desarrollo_Servidor_1ev.Models;

var builder = WebApplication.CreateBuilder(args);


// Espacio reservado para registrar Repositorios


// Espacio reservado para registrar Servicios


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
