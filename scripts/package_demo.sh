#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MOD_ID="Guzhenren"
CONFIGURATION="${CONFIGURATION:-Debug}"
OUTPUT_DIR="${1:-$ROOT_DIR/dist/$MOD_ID}"
INSTALL_DIR="${INSTALL_DIR:-}"

find_godot() {
  if [[ -n "${GODOT_BIN:-}" && -x "${GODOT_BIN}" ]]; then
    printf '%s\n' "${GODOT_BIN}"
    return 0
  fi

  local candidate
  for candidate in \
    "$(command -v godot4 2>/dev/null || true)" \
    "$(command -v godot 2>/dev/null || true)" \
    "/Applications/Godot_mono.app/Contents/MacOS/Godot" \
    "/Applications/Godot.app/Contents/MacOS/Godot" \
    "$HOME/Applications/Godot_mono.app/Contents/MacOS/Godot" \
    "$HOME/Applications/Godot.app/Contents/MacOS/Godot"; do
    if [[ -n "${candidate}" && -x "${candidate}" ]]; then
      printf '%s\n' "${candidate}"
      return 0
    fi
  done

  return 1
}

DLL_PATH="$ROOT_DIR/.godot/mono/temp/bin/$CONFIGURATION/$MOD_ID.dll"
JSON_PATH="$ROOT_DIR/$MOD_ID.json"
PCK_PATH="$OUTPUT_DIR/$MOD_ID.pck"

mkdir -p "$OUTPUT_DIR"

echo "==> Building $MOD_ID.dll ($CONFIGURATION)"
dotnet build "$ROOT_DIR/Guzhenren.sln" -c "$CONFIGURATION"

if [[ ! -f "$DLL_PATH" ]]; then
  echo "Expected DLL not found: $DLL_PATH" >&2
  exit 1
fi

GODOT_PATH="$(find_godot || true)"
if [[ -z "$GODOT_PATH" ]]; then
  echo "Godot editor not found. Install Godot 4.5+ or set GODOT_BIN, then rerun." >&2
  exit 2
fi

echo "==> Exporting $MOD_ID.pck with $GODOT_PATH"
"$GODOT_PATH" --headless --path "$ROOT_DIR" --export-pack "Mod PCK" "$PCK_PATH"

echo "==> Copying manifest and DLL"
cp "$DLL_PATH" "$OUTPUT_DIR/$MOD_ID.dll"
cp "$JSON_PATH" "$OUTPUT_DIR/$MOD_ID.json"

echo "==> Demo package ready"
echo "$OUTPUT_DIR/$MOD_ID.dll"
echo "$OUTPUT_DIR/$MOD_ID.pck"
echo "$OUTPUT_DIR/$MOD_ID.json"

if [[ -n "$INSTALL_DIR" ]]; then
  mkdir -p "$INSTALL_DIR/$MOD_ID"
  cp "$OUTPUT_DIR/$MOD_ID.dll" "$INSTALL_DIR/$MOD_ID/$MOD_ID.dll"
  cp "$OUTPUT_DIR/$MOD_ID.pck" "$INSTALL_DIR/$MOD_ID/$MOD_ID.pck"
  cp "$OUTPUT_DIR/$MOD_ID.json" "$INSTALL_DIR/$MOD_ID/$MOD_ID.json"
  echo "==> Installed to"
  echo "$INSTALL_DIR/$MOD_ID/"
fi
