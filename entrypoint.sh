#!/bin/sh
set -e

KEY_FILE="${JWT_DEV_SECRET_FILE:-/var/lib/seniorcrud/jwt_secret}"
PLACEHOLDER="CHANGE_ME_WITH_A_STRONG_SECRET"
CURRENT="${Jwt__SecretKey:-}"

if [ -n "$CURRENT" ] && [ "$CURRENT" != "$PLACEHOLDER" ]; then
  echo "[entrypoint] Using JWT secret provided via JWT_SECRET_KEY."
elif [ -f "$KEY_FILE" ]; then
  export Jwt__SecretKey="$(cat "$KEY_FILE")"
  echo "[entrypoint] Using persisted development JWT secret from $KEY_FILE."
else
  export Jwt__SecretKey="$(od -An -N64 -tx1 /dev/urandom | tr -d ' \n')"
  umask 077
  mkdir -p "$(dirname "$KEY_FILE")"
  printf '%s' "$Jwt__SecretKey" > "$KEY_FILE"
  chmod 600 "$KEY_FILE"
  echo "[entrypoint] JWT_SECRET_KEY not set: generated a random development secret (64 bytes) and persisted it in $KEY_FILE."
  echo "[entrypoint] Local-development convenience only. For any non-local run, set a strong JWT_SECRET_KEY explicitly."
fi

exec dotnet SeniorCrud.Api.dll
