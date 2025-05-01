#!/bin/bash
echo "Creando migración..."
if [ -z "$1" ]; then
  echo "❌ Debes pasar un nombre para la migración"
  exit 1
fi

dotnet ef migrations add "$1" \
  --project ./AuthenticationApi.Infrastructure \
  --startup-project ./AuthenticationApi \
  --context ApplicationDbContext
