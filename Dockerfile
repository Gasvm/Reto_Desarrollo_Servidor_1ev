# Imagen base para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura dependencias
COPY ["Reto_Desarrollo_Servidor_1ev.csproj", "./"]
RUN dotnet restore "Reto_Desarrollo_Servidor_1ev.csproj"

# Copia todo el código fuente
COPY . .

# Compila la aplicación
RUN dotnet build "Reto_Desarrollo_Servidor_1ev.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "Reto_Desarrollo_Servidor_1ev.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reto_Desarrollo_Servidor_1ev.dll"]