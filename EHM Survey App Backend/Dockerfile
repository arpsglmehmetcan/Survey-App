# 1. Aşama: Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Çözüm ve proje dosyalarını doğru şekilde kopyalayın
COPY ["EHM_Survey_App.sln", "./"]
COPY ["EHM_Survey_App.csproj", "./"]

# Bağımlılıkları yükleyin
RUN dotnet restore "./EHM_Survey_App.sln"

# Tüm dosyaları kopyalayın ve projeyi derleyin
COPY . .
RUN dotnet publish -c Release -o /out

# 2. Aşama: Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 5000

# Uygulamayı başlatın
ENTRYPOINT ["dotnet", "EHM_Survey_App.dll"]
