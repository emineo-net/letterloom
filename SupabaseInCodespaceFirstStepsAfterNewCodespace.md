Wichtige Hinweise nach dem Rebuild
Nach dem Speichern musst du den Container neu bauen lassen („Dev Containers: Rebuild Container“ oder in Codespaces den Container neu erstellen). Danach gilt:

supabase start ausführen – beim ersten Mal werden die Images heruntergeladen, das dauert einige Minuten.

Port 54321 weiterleiten – im Codespace-Ports-Tab taucht der Port nach dem Start auf. Stelle ihn auf Public (oder zumindest „Organization“), damit du vom Browser deines lokalen Rechners aus auf das Supabase Studio und die API zugreifen kannst.

URL in Blazor WASM anpassen – http://localhost:54321 funktioniert aus dem Browser deines lokalen Rechners nicht, weil dein Codespace auf einem anderen Host läuft. Verwende stattdessen die von Codespaces generierte URL im Format https://<codespace-name>-54321.app.github.dev in deinen Supabase-Verbindungseinstellungen.

Eventuell SUPABASE_SERVICES_HOSTNAME setzen – wenn die Supabase-CLI aus dem DevContainer heraus Probleme hat, die gestarteten Dienste zu erreichen, kann die Umgebungsvariable SUPABASE_SERVICES_HOSTNAME=host.docker.internal helfen.

Damit ist die Supabase CLI reproduzierbar für jeden Codespace-Start verfügbar und du musst dich nicht mehr um manuelle Installation kümmern.

