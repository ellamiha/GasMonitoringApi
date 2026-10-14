# GasMonitoringApi

Учебное Web API мониторинга газовых скважин на ASP.NET Core / .NET 10. Это самостоятельное учебное задание, связанное с темой диплома.

## Соответствие заданию

| Требование | Реализация |
|---|---|
| 2–3 предметных класса с бизнес-логикой | `Models/Well.cs`: метод `CalculatePlanPercent`; `Models/Measurement.cs`: метод `IsPressureExceeded` |
| CRUD для модели | `WellsController` и `MeasurementsController`: GET списка, GET по ID, POST, PUT, DELETE |
| 2–5 запросов с объединением данных | `ReportsController`: `journal`, `pressure-alerts`, `summary` — три отчёта по скважинам и измерениям |
| LINQ, в том числе Where и Select | `Join`, `GroupJoin`, `Where`, `Select`, сортировка и поиск используются в контроллерах |
| Отчёты только после авторизации (+2) | `[Authorize]` на `ReportsController`; без токена — 401 |
| Добавление только администраторам (+2, тот же пункт) | `[Authorize(Roles = "Admin")]` на POST; пользователь — 403, администратор — 201. PUT и DELETE также защищены |
| Release и Postman (+2) | Release опубликован локально через `dotnet publish -c Release`; коллекция `postman/GasMonitoringApi.postman_collection.json` содержит 45 запросов и 64 проверки за прогон |
| Релиз в репозитории и ссылка (+1) | Требуется опубликованный GitHub Release; наличие локального архива само по себе этот пункт не закрывает |

`LoginRequest` — DTO для входа, а не дополнительный предметный класс.

## Модель и бизнес-логика

**Well (скважина)**: ID, название, максимально допустимое давление и плановый расход. `CalculatePlanPercent(actualFlow)` рассчитывает `actualFlow / PlannedFlow * 100` с округлением до двух знаков.

**Measurement (измерение)**: ID, ID скважины, время, давление и фактический расход. `IsPressureExceeded(maxPressure)` проверяет строгое превышение давления. Равенство пределу не считается превышением.

Одна скважина может иметь много измерений. Нельзя добавить измерение для отсутствующей скважины. Нельзя удалить скважину, пока существуют её измерения (409). Ограничения входных данных заданы атрибутами `Required`, `StringLength`, `Range`.

## Запуск исходного проекта

Нужен .NET 10 SDK. В терминале из папки проекта:

```bash
dotnet restore
dotnet run --urls http://localhost:5052
```

В режиме Development Swagger доступен по адресу `http://localhost:5052/swagger`. Для запросов используется `http://localhost:5052`. Если порт занят, остановите предыдущий экземпляр API или выберите другой порт и измените `baseUrl` в Postman.

## Сборка и запуск Release

```bash
dotnet publish GasMonitoringApi.csproj -c Release --self-contained false -p:UseAppHost=false -o artifacts/release
dotnet artifacts/release/GasMonitoringApi.dll --urls http://localhost:5052
```

Готовый архив `GasMonitoringApi-v1.0.0-release.zip` содержит опубликованную сборку. Для её запуска нужен **ASP.NET Core Runtime 10** либо .NET 10 SDK. Сборка зависит от установленной среды выполнения, не является автономной. После распаковки перейдите в папку `GasMonitoringApi-release` и выполните:

```bash
dotnet GasMonitoringApi.dll --urls http://localhost:5052
```

Swagger включён только в Development; его отсутствие при обычном запуске готовой сборки не является ошибкой API.

## Учебные учётные записи

| Логин | Пароль | Роль |
|---|---|---|
| user | user123 | User |
| admin | admin123 | Admin |

POST `/api/auth/login` возвращает JWT в `accessToken`. Для защищённых запросов используется `Authorization: Bearer <token>`. Токены действуют два часа. Ключ подписи создаётся при запуске API: после перезапуска необходимо снова выполнить вход.

Это демонстрационная авторизация с учебными паролями в коде. Данные хранятся в памяти `AppData` и сбрасываются при перезапуске; постоянная база данных заданием не требуется.

## Маршруты

| Метод и маршрут | Назначение | Доступ |
|---|---|---|
| POST /api/auth/login | Вход | Все |
| GET /api/wells и /api/wells/{id} | Просмотр скважин | Все |
| POST /api/wells | Создание скважины | Admin |
| PUT, DELETE /api/wells/{id} | Изменение, удаление | Admin |
| GET /api/measurements и /api/measurements/{id} | Просмотр измерений | Все |
| POST /api/measurements | Создание измерения | Admin |
| PUT, DELETE /api/measurements/{id} | Изменение, удаление | Admin |
| GET /api/reports/journal | Измерения с названием скважины | User / Admin |
| GET /api/reports/pressure-alerts | Превышения давления с пределами скважины | User / Admin |
| GET /api/reports/summary | Последнее измерение каждой скважины, выполнение плана и статус давления | User / Admin |

## Проверка в Postman

1. Запустите API.
2. Импортируйте `postman/GasMonitoringApi.postman_collection.json`.
3. Выберите коллекцию **GasMonitoringApi — полная проверка**, `No Environment` и `baseUrl = http://localhost:5052`.
4. Откройте Run и выполните все 45 запросов по порядку, одну итерацию.
5. Ожидаемый результат: **Passed 64, Failed 0, Errors 0**.

Токены и ID заполняются автоматически. Создаётся отдельная скважина `Postman <число>` и измерение; в конце они удаляются. Коды 400, 401, 403, 404 и 409 в отрицательных сценариях ожидаемы — ориентируйтесь на PASS. Для повторного прогона начинайте с первого запроса.

## Проверено перед сдачей

- Текущие исходники успешно собраны в Release и опубликованы локально.
- На опубликованной Release-сборке выполнены два последовательных прогона: 90 HTTP-запросов, 128 проверок без ошибок.
- JavaScript-скрипты коллекции проверены локальным Node.js-исполнителем используемых методов `pm`; данные до и после каждого прогона совпали.
- Пользовательский запуск этой коллекции в интерфейсе Postman ранее показал Passed 64, Failed 0, Errors 0.

## Что показать преподавателю

1. Два класса в Models и по одному бизнес-методу в каждом.
2. CRUD-контроллеры и три метода ReportsController; указать Join/GroupJoin, Where и Select.
3. Запустить готовую Release-сборку и весь набор Postman.
4. Показать вход под двумя ролями, 401 без токена, 403 для User при добавлении и 201 для Admin.
5. Показать результаты отчётов и зелёные проверки Runner.
6. Открыть опубликованный GitHub Release и передать преподавателю его ссылку.

Окончательная оценка зависит от демонстрации и проверки преподавателем.
