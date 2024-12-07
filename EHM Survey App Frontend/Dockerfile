# Frontend için Node.js imajı kullanın
FROM node:18-alpine
WORKDIR /frontend

# Bağımlılıkları yükleyin
COPY FRONT-END/package*.json ./
RUN npm install

# Tüm dosyaları kopyalayın
COPY FRONT-END .

# React uygulamasını derleyin
RUN npm run build

# Uygulamayı sunmak için bir Nginx sunucusu kullanın
FROM nginx:alpine
COPY --from=build /frontend/build /usr/share/nginx/html
EXPOSE 80
