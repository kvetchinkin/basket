# OpenApi
> Вызывать из папки BasketApp.Api/Adapters/Http/Contract
```
openapi-generator generate -i https://gitlab.com/microarch-ru/microservices/dotnet/system-design/-/raw/main/services/basket/contracts/openapi.yml -g aspnetcore -o . --package-name Api --additional-properties classModifier=abstract --additional-properties operationResultTask=true
```
# БД 
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```
[Подробнее про dotnet cli](https://learn.microsoft.com/ru-ru/ef/core/cli/dotnet)

# Миграции
```
dotnet ef migrations add Init --startup-project ./BasketApp.Api --project ./BasketApp.Infrastructure
dotnet ef database update --startup-project ./BasketApp.Api --connection "Server=localhost;Port=5432;User Id=username;Password=secret;Database=basket;"
```