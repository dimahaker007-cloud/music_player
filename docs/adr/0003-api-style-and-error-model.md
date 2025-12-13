REST API Стиль
-
Resource Naming:
+ Множина для колекцій: /api/music, /api/users
+ Сингular для конкретних ресурсів: /api/music/{id}, /api/users/{id}, /api/stream/{id}

HTTP Methods Semantics:
+ POST - створення нового ресурсу (повертає 201 Created)
+ GET - отримання ресурсу (повертає 200 OK)
+ PUT - повна заміна ресурсу (повертає 200 OK)
+ PATCH - часткове оновлення (повертає 200 OK)
+ DELETE - видалення ресурсу (повертає 204 No Content)

Політика HTTP Status Codes
-
Успішні відповіді:
+ 200 OK - успішне виконання GET, PUT, PATCH
+ 201 Created - успішне створення ресурсу (POST)
+ 204 No Content - успішне видалення (DELETE)

Помилки клієнта :

400 Bad Request - невалідний запит (помилки валідації)
401 Unauthorized - необхідна аутентифікація
403 Forbidden - доступ заборонено
404 Not Found - ресурс не знайдено
409 Conflict - конфлікт стану (наприклад, дублювання)
