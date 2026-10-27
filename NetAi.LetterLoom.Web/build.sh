#!/usr/bin/env bash

# Fehler abfangen (bricht das Skript ab, falls ein Befehl fehlschlägt)
set -e

# 1. .NET Installationsskript herunterladen
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh

# 2. .NET 10 SDK installieren
./dotnet-install.sh -c 10.0 -InstallDir ./dotnet
export DOTNET_ROOT=$PWD/dotnet
export PATH=$PATH:$PWD/dotnet

# Temporäres Installationsskript aufräumen
rm dotnet-install.sh

# 3. Blazor WebAssembly App veröffentlichen
./dotnet/dotnet publish NetAi.LetterLoom.Web/NetAi.LetterLoom.Web.csproj -c Release -o output

# =====================================================================
# ERGÄNZUNG FÜR CLOUDFLARE PAGES & PWA (Wird automatisch nach dem Build erzeugt)
# =====================================================================

TARGET_WWWROOT="output/wwwroot"

# A) _redirects-Datei für SPA-Routing & PWA-Assets erstellen
cat << 'EOF' > "$TARGET_WWWROOT/_redirects"
# Verhindert, dass kritische PWA-Core-Dateien auf die index.html umgeleitet werden
/service-worker.js /service-worker.js 200
/service-worker-assets.js /service-worker-assets.js 200
/manifest.json /manifest.json 200

# Standard-Fallback für das SPA-Routing (leitet alle URLs intern an Blazor weiter)
/* /index.html 200
EOF

# B) _headers-Datei für das korrekte PWA Cache-Verhalten erstellen
cat << 'EOF' > "$TARGET_WWWROOT/_headers"
/service-worker.js
  cache-control: max-age=0, no-cache, no-store, must-revalidate
/service-worker-assets.js
  cache-control: max-age=0, no-cache, no-store, must-revalidate
EOF

echo "Build und Cloudflare-PWA-Konfiguration erfolgreich abgeschlossen!"
