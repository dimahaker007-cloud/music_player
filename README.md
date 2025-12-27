# Music Player Project
Щоб запустити мій додаток спочатку запускається база даних потім апі а далі фронтенд
Обраний підхід до Delivery
Для автоматизації доставки проєкту обрано **Варіант A: GitHub Container Registry (GHCR)**.
Цей підхід дозволяє зберігати готові до розгортання Docker-образи безпосередньо в екосистемі GitHub.
![CI Status](https://github.com/dimahaker007-cloud/music_player/actions/workflows/delivery.yml/badge.svg)

## Локальний запуск
Проект складається з API, Blazor-фронтенду та MySQL.
Для запуску всіх компонентів одночасно використовуйте Docker Compose:
```bash
docker-compose up --build 
