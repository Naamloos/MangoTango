FROM node:16.15.0-alpine3.15 as build
WORKDIR /app
COPY . .

RUN npm ci
RUN npm run build

ENV NODE_ENV production

CMD ["npx", "serve", "build"]