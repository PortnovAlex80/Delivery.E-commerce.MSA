docker exec -it infrastructure-postgres-1 bash
apt-get update
apt-get install vim -y

vi couriers.sql

psql -U username -d delivery
