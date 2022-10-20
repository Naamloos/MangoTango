# pull official base image
FROM node:16.15.0-alpine3.15

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH
ENV NODE_ENV production

ENV REACT_APP_API_ENDPOINT http://surface-web-api-link
ENV REACT_APP_NAME MyServer
ENV REACT_APP_DESCRIPTION MyServer is a Whitelisted server
ENV REACT_APP_MINECRAFT_IP 127.0.0.1

# install app dependencies
COPY package.json /app/
COPY package-lock.json /app/
RUN npm install --silent

# add app
COPY . ./

RUN npm install serve -g
RUN npm run build

# start app
CMD ["serve", "-s", "build"]
