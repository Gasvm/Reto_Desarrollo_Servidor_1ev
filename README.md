# Reto_Desarrollo_Servidor_1ev
Reto_Desarrollo_Servidor_1ev

#Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=AlejandroPonnosUn10(!)" -p 7068:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04

# Docker Cheatsheet de comandos

docker-compose up -d              # Iniciar
docker-compose down               # Detener
docker-compose ps                 # Ver estado
docker-compose logs -f            # Ver logs
docker-compose up -d --build      # Rebuild + iniciar
docker-compose down -v            # Detener + borrar datos
docker-compose build api          # Reconstruir la imagen de la API

## URLs importantes

- Swagger: http://localhost:8607/swagger
- API: http://localhost:8607/api
- Frontend: http://localhost:5500/www/admin.html
