#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$SCRIPT_DIR/Grinderino/Grinderino.csproj"
CONFIGURATION="${CONFIGURATION:-Debug}"

dotnet build "$PROJECT_PATH" -c "$CONFIGURATION" "$@"
