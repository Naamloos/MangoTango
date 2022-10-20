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

# add app
COPY . ./

RUN npm install serve -g
RUN npm run build
RUN serve -s build

# start app
CMD ["npm", "start"]
