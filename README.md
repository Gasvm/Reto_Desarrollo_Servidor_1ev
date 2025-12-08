# Reto_Desarrollo_Servidor_1ev
Reto_Desarrollo_Servidor_1ev

# Docker Cheatsheet de comandos

docker-compose up -d --build      # Construir imagen + iniciar
docker-compose up -d              # Iniciar
docker-compose down               # Detener
docker-compose down -v            # Detener + borrar datos
docker-compose ps                 # Ver estado
docker-compose logs -f            # Ver log
docker-compose logs -f db-init    # Ver logs de db
docker-compose build api          # Reconstruir la imagen de la API

## URLs importantes

- Swagger: http://localhost:8607/swagger
- API: http://localhost:8607/api
- Frontend: http://localhost:5500/www/admin.html