# pull official base image
FROM node:16.15.0-alpine3.15

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH
ENV NODE_ENV production

# install app dependencies
COPY package.json /app/
COPY package-lock.json /app/
RUN npm install --silent
RUN npm install react-scripts@3.4.1 -g --silent

# add app
COPY . ./

# start app
CMD ["npm", "start"]
