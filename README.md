# MangoTango
A simple web panel for Minecraft servers using Geyser+Floodgate users to handle whitelist requests for both Java and Bedrock players.

This application was initially written in about half a day.

This application is called after my Minecraft server that is as well called MangoTango. That server is named after one of my cats though 🙂

## Setup
MangoTango requires some setup to get working properly. As of right now, I only _officially_ support docker-compose setups. The application _should_ work outside of docker as well.

1. Clone this repository with `git clone https://github.com/Naamloos/MangoTango`.
2. `cd MangoTango`
3. Edit `docker-compose.yml`. All values you'll most likely want to modify have a comment reading `CHANGEME`
4. Open the Frontend directory (`cd Frontend`) and modify all values in `.env`
5. Go back up one directory (`cd ../`)
6. Run docker compose `docker compose up --detach`
7. Your web services should now be up and running. We still need to do a bit more though.
8. Give internet access to the API. `docker network connect bridge mangotango-mangotango-api`
9. Done! You should be up and running. You can for example use NGINX to reverse proxy to `mangotango-api` for the api and `mangotango-web` for the web panel.

### Example NGINX configuration
```nginx
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
NGINX does not allow underscores in headers by default, so you will have to add `underscores_in_headers on;` to your NGINX server declaration.
Example can be seen in the example NGINX config above.

## Screenshots (as of the 20th of October, 2022)
### Unauthenticated view
![afbeelding](https://user-images.githubusercontent.com/12187179/196905103-167d2f40-249e-44b1-af3c-3c0526e6f6a3.png)
### After Authentication, requests appear under the form
![afbeelding](https://user-images.githubusercontent.com/12187179/196904836-3d594a90-3f96-41e8-99c6-42eefb435f1d.png)
