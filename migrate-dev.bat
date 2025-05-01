@echo off
IF [%1]==[] (
  echo ❌ Debes pasar un nombre para la migracion
  GOTO end
)

dotnet ef migrations add %1 ^
  --project .\AuthenticationApi.Infrastructure ^
  --startup-project .\AuthenticationApi ^
  --context ApplicationDbContext

:end
