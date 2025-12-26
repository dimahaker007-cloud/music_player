# Етап збірки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копіюємо всі .csproj файли для відновлення залежностей
# Це робиться окремо, щоб Docker кешував шари
COPY ["Api/Api.csproj", "Api/"]
COPY ["Bussines/Bussines.csproj", "Bussines/"]
COPY ["Core/Core.csproj", "Core/"]

# 2. Відновлюємо пакети для головного проєкту
RUN dotnet restore "Api/Api.csproj"

# 3. Копіюємо ВЕСЬ код усіх проєктів
COPY . .

# 4. Публікуємо Api (він сам підтягне залежні проєкти Core та Bussines)
WORKDIR "/src/Api"
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Етап запуску
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]