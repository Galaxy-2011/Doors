#!/usr/bin/env bash
# publish.sh - build a self-contained linux-x64 publish and copy Assets
set -euo pipefail
ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT="${ROOT_DIR}/MonogameDoors.csproj"
OUT_DIR="${ROOT_DIR}/publish/linux-x64"

echo "Publishing project to ${OUT_DIR} (self-contained linux-x64, Release)"

dotnet publish "$PROJECT" -c Release -r linux-x64 --self-contained true -o "$OUT_DIR"

echo "Copying Assets folder to publish output"
mkdir -p "$OUT_DIR/Assets"
cp -r "$ROOT_DIR/Assets/"* "$OUT_DIR/Assets/" || true

echo "Publish completed. Run the game with: $OUT_DIR/MonogameDoors"
