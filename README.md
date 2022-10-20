# MangoTango
A web panel for Geyser+Floodgate users to handle whitelist requests.

I wrote this in about half a day.

Coincidentally my server is also called MangoTango 🤔

## Setup
MangoTango requires some setup to get working properly. As of right now, I only officially support docker-compose setups, but more will be added later.

1. Clone this repository with `git clone https://github.com/Naamloos/MangoTango`.
2. `cd MangoTango`
3. Edit `docker-compose.yml`. All values you'll most likely want to modify have a comment reading `CHANGEME`
4. Open the Frontend directory (`cd Frontend`) and modify all values in `.env`
5. Go back up one directory (`cd ../`)
6. Run docker compose `docker compose up --detach`
7. Your web services should now be up and running. We still need to do a bit more though.
8. Add the `mangotango-mangotango-api` container to the `bridge` network to give it internet access. `docker network connect bridge mangotango-mangotango-api`
9. Done! You should be up and running. You can for example use NGINX to reverse proxy to `mangotango-api` for the api and `mangotango-web` for the web panel.

### Example NGINX configuration
```
server
{
    listen 80;
    server_name whitelist.example.org;
    location /
    {
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header Host $http_host;
        proxy_pass http://mangotango-web:3000;
    }
}

server
{
    listen 80;
    server_name whitelist-api.example.org;
    location /
    {
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header Host $http_host;
        proxy_pass http://mangotango-api;
    }
	
	underscores_in_headers on;
}
```

#### ⚠️ NGINX note!
MangoTango uses a header named `rcon_password` to authenticate! 
NGINX does not allow underscode headers by default, so you will have to add `underscores_in_headers on;` to your NGINX server declaration.
Example can be seen in the example NGINX config above.