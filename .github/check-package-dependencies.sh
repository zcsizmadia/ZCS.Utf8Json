#!/usr/bin/env bash
# Fails if a packed .nupkg depends on the upstream "Utf8Json" package instead of "ZCS.Utf8Json".
# 1.4.6 and 1.5.0 of the extension packages shipped with that bug and could not be installed
# (the upstream package stops at 1.3.7).
set -euo pipefail

dir="${1:?usage: check-package-dependencies.sh <nupkg-dir>}"
shopt -s nullglob
packages=("$dir"/*.nupkg)
if [ ${#packages[@]} -eq 0 ]; then
  echo "::error::No .nupkg files found in $dir"
  exit 1
fi

status=0
for pkg in "${packages[@]}"; do
  nuspec="$(unzip -p "$pkg" '*.nuspec')"
  echo "$(basename "$pkg"):"
  echo "$nuspec" | grep -o '<dependency id="[^"]*" version="[^"]*"' | sort -u | sed 's/^/  /' || true
  if echo "$nuspec" | grep -q '<dependency id="Utf8Json"'; then
    echo "::error::$(basename "$pkg") depends on the upstream 'Utf8Json' package instead of 'ZCS.Utf8Json'"
    status=1
  fi
done
exit $status
