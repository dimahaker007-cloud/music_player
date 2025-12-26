# Етап 1: Збірка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Копіюємо файли проектів
COPY ["frontend/frontend.csproj", "frontend/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Bussines/Bussines.csproj", "Bussines/"]

# 2. Відновлюємо залежності
RUN dotnet restore "frontend/frontend.csproj"

# 3. Копіюємо ТІЛЬКИ необхідні папки, щоб уникнути конфліктів файлів
COPY frontend/ frontend/
COPY Core/ Core/
COPY Bussines/ Bussines/

# 4. Публікуємо
WORKDIR "/src/frontend"
RUN dotnet publish "frontend.csproj" -c Release -o /app/publish

# Етап 2: Роздача
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]