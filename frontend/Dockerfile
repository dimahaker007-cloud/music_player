FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["frontend/frontend.csproj", "frontend/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Bussines/Bussines.csproj", "Bussines/"]

RUN dotnet restore "frontend/frontend.csproj"

COPY frontend/ frontend/
COPY Core/ Core/
COPY Bussines/ Bussines/

WORKDIR "/src/frontend"
RUN dotnet publish "frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]