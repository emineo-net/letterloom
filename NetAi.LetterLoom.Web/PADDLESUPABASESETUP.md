

letterloom
default db password
letterloom

projectId:
lvfvnmbigfzpgxsvqlzn


region:
eu-west-1





# ============================================================================
#  LETTERLOOM V2 — NEUES GITHUB REPO + NEUES SUPABASE PROJEKT + WEBHOOK
#  Ausgangslage: Neue Solution, neues Repository, neues Supabase-Projekt
#  Ziel:         Vollständig eingerichtete Server-Seite für Paddle-Webhooks
# ============================================================================


# ============================================================================
# SCHRITT 0 — NEUES GITHUB REPOSITORY ERSTELLEN
# Ausführen in: Browser (0.1) und PowerShell (0.2–0.5)
# ============================================================================

# 0.1  Repository auf GitHub anlegen
#      → https://github.com/new
#      → Repository name:  LetterLoom (oder dein gewünschter Name)
#      → Description:      "WPF Blazor Hybrid App mit Paddle-Lizenzierung"
#      → Visibility:       Private (empfohlen für kommerzielle Projekte)
#      → Initialize:       KEIN README, KEINE .gitignore, KEINE Lizenz
#                          (wir initialisieren lokal, um Konflikte zu vermeiden)
#      → "Create repository" klicken
#      → Die angezeigte Remote-URL kopieren (z. B. https://github.com/<USER>/LetterLoom.git)

# 0.2  Lokalen Projektordner anlegen und hineinwechseln
#      (Pfad an deine Struktur anpassen)
New-Item -ItemType Directory -Force -Path "C:\Users\steph\source\repos\LetterLoomV2" | Out-Null
cd C:\Users\steph\source\repos\LetterLoomV2

# 0.3  Git initialisieren
git init

# 0.4  .gitignore anlegen (WICHTIG: schützt vor versehentlichem Commit von Secrets)
@'
# .NET
bin/
obj/
publish/
*.user

# Secrets & Keys
*.pem
!licensing/license-public.pem
.env
.env.*
!.env.example

# Supabase
supabase/.temp/
supabase/.branches/

# IDE
.vs/
.vscode/
*.suo
'@ | Set-Content -Path ".gitignore" -Encoding ascii

# 0.5  Remote hinzufügen und ersten Commit pushen
git remote add origin https://github.com/<USER>/LetterLoom.git
git add .
git commit -m "Initial commit: Solution scaffold"
git branch -M master
git push -u origin master
# Erwartet: "branch 'master' set up to track 'origin/master'"


# ============================================================================
# SCHRITT 1 — NEUES SUPABASE PROJEKT ERSTELLEN
# Ausführen in: Browser
# ============================================================================

# 1.1  Supabase Dashboard öffnen
#      → https://supabase.com/dashboard
#      → Mit deinem Account einloggen

# 1.2  Neues Projekt anlegen
#      → "New Project" klicken
#      → Organization:  deine Organisation wählen
#      → Name:          LetterLoom (oder "LetterLoom V2")
#      → Database Password: starkes Passwort generieren und SICHER speichern
#      → Region:        die Region wählen, die deinen Nutzern am nächsten liegt
#      → Plan:          Free (für Entwicklung) oder Pro (für Produktion)
#      → "Create new project" klicken

# 1.3  Warten, bis das Projekt bereit ist
#      Die Bereitstellung dauert in der Regel 1–2 Minuten.
#      Status wechselt von "Provisioning" zu "Healthy".

# 1.4  Project-Ref notieren
#      → Project Settings → General → "Reference ID"
#      Format: abcdefghijklmnopqrst
#      MERKEN als: <PROJECT_REF>

# 1.5  Optional: Supabase CLI mit dem neuen Projekt verknüpfen
#      (falls du die CLI nutzen möchtest; die Dashboard-Variante funktioniert auch ohne)
cd C:\Users\steph\source\repos\LetterLoomV2
supabase login
supabase init
supabase link --project-ref <PROJECT_REF>


# ============================================================================
# SCHRITT 2 — SECRETS SETZEN (DASHBOARD)
# Ausführen in: Browser (Supabase Dashboard)
# ============================================================================

# 2.1  Edge Function Secrets öffnen
#      → Linke Seitenleiste: "Edge Functions"
#      → Oben rechts: "Secrets" (oder Project Settings → Edge Functions → Secrets)

# 2.2  Secret 1 von 4: LICENSE_PRIVATE_KEY
#      → "Add new secret"
#      → Name:  LICENSE_PRIVATE_KEY
#      → Value: Gesamten Inhalt von privateKey.pem einfügen
#               (inkl. -----BEGIN PRIVATE KEY----- und -----END PRIVATE KEY-----)
#      → Save

# 2.3  Secret 2 von 4: PADDLE_WEBHOOK_SECRET
#      → Name:  PADDLE_WEBHOOK_SECRET
#      → Value: pdl_ntfset_REPLACE_ME   (wird in Schritt 5 ersetzt)
#      → Save

# 2.4  Secret 3 von 4: PADDLE_PRODUCT_IDS
#      → Name:  PADDLE_PRODUCT_IDS
#      → Value: pro_REPLACE_ME          (wird in Schritt 5 ersetzt)
#      → Save

# 2.5  Secret 4 von 4: PADDLE_ENVIRONMENT
#      → Name:  PADDLE_ENVIRONMENT
#      → Value: sandbox                 (für Live später auf "production" ändern)
#      → Save

# 2.6  Verify: Alle 4 Secrets sind in der Liste sichtbar
#      Erwartet: LICENSE_PRIVATE_KEY, PADDLE_WEBHOOK_SECRET,
#                PADDLE_PRODUCT_IDS, PADDLE_ENVIRONMENT


# ============================================================================
# SCHRITT 3 — DB-MIGRATION (SQL EDITOR)
# Ausführen in: Browser (Supabase Dashboard → SQL Editor)
# ============================================================================

# 3.1  SQL Editor öffnen
#      → Linke Seitenleiste: "SQL Editor"

# 3.2  Neues Query anlegen
#      → "+ New query" klicken

# 3.3  Folgendes SQL einfügen und ausführen (Run-Button oder Ctrl+Enter):
/*
-- Lizenzen für einmalige Paddle-Käufe (LetterLoom pro)
create table if not exists public.licenses (
  id                    uuid        primary key default gen_random_uuid(),
  license_id            text        not null unique,
  paddle_transaction_id text        not null unique,
  paddle_customer_id    text        not null,
  paddle_event_id       text        not null,
  license_key           text        not null,
  tier                  text        not null default 'pro',
  issued_at             timestamptz not null default now(),
  expires_at            timestamptz,
  is_sandbox            boolean     not null default false,
  created_at            timestamptz not null default now()
);

create index if not exists idx_licenses_paddle_customer
  on public.licenses (paddle_customer_id);

-- RLS aktiv: kein direkter Client-Zugriff.
-- Lesezugriff läuft ausschließlich über Edge Functions mit Service-Role.
alter table public.licenses enable row level security;
*/

# 3.4  Verify: Tabelle existiert
#      → Linke Seitenleiste: "Table Editor"
#      → Tabelle "licenses" sollte sichtbar sein (leer, 0 rows)


# ============================================================================
# SCHRITT 4 — EDGE FUNCTION DEPLOYEN (DASHBOARD)
# Ausführen in: Browser (Supabase Dashboard)
# ============================================================================

# 4.1  Edge Functions öffnen
#      → Linke Seitenleiste: "Edge Functions"

# 4.2  Neue Function anlegen
#      → "Deploy a new function" klicken
#      → "Via Editor" auswählen

# 4.3  Function-Name
#      → Name: paddle-webhook

# 4.4  Code einfügen
#      → Den kompletten TypeScript-Code der supabaseEdgeFunc.ts einfügen

# 4.5  WICHTIG: JWT-Verifikation deaktivieren
#      → In den Function-Einstellungen (Zahnrad / Settings):
#        "Enforce JWT Verification" → AUS (deaktivieren)
#      Begründung: Paddle sendet keinen Supabase-JWT, sondern die
#                  Paddle-Signatur. Die Auth-Prüfung passiert im Code.

# 4.6  Deployen
#      → "Deploy function" klicken
#      → Warten, bis der Status "Deployed" oder "Active" ist.

# 4.7  Verify: URL ist erreichbar
$webhookUrl = "https://<PROJECT_REF>.supabase.co/functions/v1/paddle-webhook"
Invoke-WebRequest -Uri $webhookUrl -Method OPTIONS -UseBasicParsing |
  Select-Object StatusCode
# Erwartet: StatusCode 200 (CORS-Preflight)

# 4.8  Verify: POST ohne Signatur → 401
try {
  Invoke-WebRequest -Uri $webhookUrl -Method POST `
    -Body '{"test":true}' -ContentType "application/json" -UseBasicParsing
  Write-Host "FEHLER: hätte 401 geben müssen"
} catch {
  Write-Host "Erwarteter Fehler: $($_.Exception.Response.StatusCode)"
}
# Erwartet: Unauthorized (401)


# ============================================================================
# SCHRITT 5 — PADDLE-DASHBOARD KONFIGURIEREN
# Ausführen in: Browser (Paddle Sandbox Dashboard)
# ============================================================================

# 5.1  Paddle (Sandbox) öffnen
#      → https://sandbox-vendors.paddle.com

# 5.2  Product-ID ablesen
#      → Catalog → Products → LetterLoom → "Product ID"
#      Format: pro_01h...
#      MERKEN als: <PRODUCT_ID>

# 5.3  Webhook-Endpoint anlegen
#      → Developer Tools → Notifications → "+ New destination"
#      - Destination type: Webhook
#      - URL: https://<PROJECT_REF>.supabase.co/functions/v1/paddle-webhook
#      - Events: transaction.completed (nur dieses eine)
#      - Type: Fulfillment Webhook
#      - Beschreibung: "LetterLoom License Issuer (Supabase)"
#      Speichern → "Secret key" kopieren
#      Format: pdl_ntfset_...
#      MERKEN als: <WEBHOOK_SECRET>

# 5.4  Zurück im Supabase Dashboard → Edge Functions → Secrets:
#      → PADDLE_WEBHOOK_SECRET  → Wert auf <WEBHOOK_SECRET> setzen
#      → PADDLE_PRODUCT_IDS     → Wert auf <PRODUCT_ID> setzen
#      → Jeweils "Save"

# 5.5  Function neu deployen (damit die neuen Secrets gelesen werden)
#      → Edge Functions → paddle-webhook → "Deploy new version"
#        (oder Code unverändert lassen und erneut "Deploy" klicken)


# ============================================================================
# SCHRITT 6 — END-TO-END-TEST (SANDBOX)
# Ausführen in: PowerShell (6.1), Dashboard (6.2–6.3), WPF (6.4–6.7)
# ============================================================================

# 6.1  Paddle Sandbox Simulator
paddle sandbox simulate transaction.completed `
  --customer-id "ctm_test_REPLACE_ME" `
  --product-id  "<PRODUCT_ID>"
# Alternativ (ohne Paddle CLI):
#   → Paddle Dashboard → Developer Tools → Notifications → Endpoint öffnen
#   → "Send test event" → Event-Typ: transaction.completed

# 6.2  Kurz warten (5-10 s), dann in Supabase → SQL Editor:
/*
select license_id, paddle_transaction_id, tier, is_sandbox,
       left(license_key, 20) as key_prefix, created_at
from public.licenses
order by created_at desc
limit 3;
*/
# Erwartet: Eine neue Zeile mit key_prefix = "llv1...."

# 6.3  Function-Logs prüfen
#      → Edge Functions → paddle-webhook → "Logs"
#      Erwartet: "[paddle-webhook] Issued license ... for txn ..."
#      Keine "Invalid signature" oder "DB insert failed"

# 6.4  Vollen license_key aus der DB kopieren
#      → SQL Editor:
/*
select license_key from public.licenses
order by created_at desc limit 1;
*/
#      → Kompletten String in die Zwischenablage kopieren

# 6.5  WPF-Aktivierung testen
#      - WPF-App starten
#      - Sidebar → "License"
#      - Key einfügen → "Activate"
#      Erwartet: Grüner "Pro license active"-Banner
#                PRO-Badge in der Sidebar
#                license.key liegt unter:
#                %LOCALAPPDATA%\NetAi.LetterLoom\license.key

# 6.6  WPF-Persistenz-Test
#      - WPF-App schließen und neu starten
#      - Sidebar → "License"
#      Erwartet: "Pro license active" (Key wurde offline verifiziert)

# 6.7  Deaktivierungs-Test
#      - Im License-Dialog → "Deactivate on this PC"
#      Erwartet: Banner weg, PRO-Badge weg, license.key gelöscht


# ============================================================================
# SCHRITT 7 — TROUBLESHOOTING (DASHBOARD)
# ============================================================================

# 7.1  Function gibt 401 "Invalid signature" zurück
#      → PADDLE_WEBHOOK_SECRET stimmt nicht mit dem Endpoint-Secret überein
#      → Edge Functions → Secrets → PADDLE_WEBHOOK_SECRET korrigieren
#      → Function neu deployen (Schritt 5.5)

# 7.2  Function gibt 500 "server misconfigured" zurück
#      → Mindestens ein Secret fehlt
#      → Edge Functions → Secrets: Alle 4 Werte prüfen (Schritt 2.6)

# 7.3  Function gibt 200 zurück, aber keine Zeile in DB
#      → Logs prüfen: "Unknown product" → PADDLE_PRODUCT_IDS korrigieren
#      → Logs prüfen: "ignored: ..." → falscher Event-Typ im Paddle-Endpoint
#        (muss transaction.completed sein, nicht subscription.*)

# 7.4  Function gibt 500 "signing failed" zurück
#      → LICENSE_PRIVATE_KEY ist kein gültiger PEM/PKCS#8
#      → Prüfen: Get-Content .\LetterLoomKeygen\privateKey.pem -TotalCount 1
#        Erwartet: -----BEGIN PRIVATE KEY-----
#      → Falls "BEGIN RSA PRIVATE KEY": mit OpenSSL zu PKCS#8 konvertieren
#        openssl pkcs8 -topk8 -inform PEM -in privateKey.pem -outform PEM `
#          -nocrypt -out privateKey-pkcs8.pem
#      → Dann Secret in Supabase mit der PKCS#8-Datei neu setzen

# 7.5  WPF sagt "InvalidSignature" trotz korrektem Key aus DB
#      → publicKey in license-public.pem passt nicht zum privateKey in Supabase
#      → Fingerprint vergleichen (lokal):
$pem = Get-Content -Raw .\LetterLoomKeygen\publicKey.pem
$rsa = [System.Security.Cryptography.RSA]::Create()
$rsa.ImportFromPem($pem)
[Convert]::ToBase64String($rsa.ExportSubjectPublicKeyInfo())
#      → Vergleichen mit dem Inhalt von
#        .\NetAi.LetterLoom\Licensing\license-public.pem
#        (Body ohne Header/Footer, Zeilenumbrüche entfernen)
#      → Falls unterschiedlich: Keygen neu laufen lassen, BEIDE Seiten tauschen

# 7.6  Cleanup: Sandbox-Lizenzen aus der DB löschen
#      → SQL Editor:
/*
delete from public.licenses where is_sandbox = true;
*/