# Reto_Desarrollo_Servidor_1ev
Reto_Desarrollo_Servidor_1ev

#Setup local db (Sql server)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=AlejandroPonnosUn10(!)" -p 7068:1433 -d mcr.microsoft.com/mssql/server:2019-CU21-ubuntu-20.04
