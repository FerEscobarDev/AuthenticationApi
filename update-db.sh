#!/bin/bash
echo "Aplicando migraciones a la base de datos..."
dotnet ef database update \
  --project ./AuthenticationApi.Infrastructure \
  --startup-project ./AuthenticationApi \
  --context ApplicationDbContext
