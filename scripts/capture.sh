#!/usr/bin/env bash
set -euo pipefail
url=${1:?Informe a URL}
output=${2:?Informe o diretorio de evidencias}
mkdir -p "$output"
chrome=$(command -v google-chrome || command -v chromium || command -v chromium-browser)
"$chrome" --headless --no-sandbox --disable-dev-shm-usage --hide-scrollbars \
  --window-size=1280,1000 --virtual-time-budget=5000 \
  --screenshot="$PWD/$output/ambiente.png" "$url/" 2> "$output/chrome.log"
test -s "$output/ambiente.png"
curl --fail --silent --show-error "$url/health/ready" > "$output/health.json"
docker compose ps > "$output/compose-ps.txt"
docker image inspect "$API_IMAGE" --format '{{.Id}}' > "$output/image-id.txt"
