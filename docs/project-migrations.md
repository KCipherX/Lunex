## Create Initial Migration

Run the following command to create the initial database migration:
```powershell
dotnet ef migrations add AccountEntity --project .\src\Lunex.Infrastructure --startup-project .\src\Lunex.Api\ -o Persistance\Migrations
```
Run the following command to create update the migrated data into database:
```powershell
dotnet ef database update --startup-project .\src\Lunex.Api\
```