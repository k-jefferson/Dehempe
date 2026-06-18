#!/usr/bin/env bash
# Lance le tunnel mTLS local Déhempé en mode foreground détaché (nohup).
# Le foreground est nécessaire : stunnel en daemon (foreground=no) fork à chaque
# connexion et le child fils perd l'état PKCS#11 de la carte → "User not logged in".
#
# Usage : docs/stunnel/start-tunnel.sh
#         (à exécuter depuis la racine du repo)
#
# Arrêt : docs/stunnel/stop-tunnel.sh

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CONF="/tmp/dehempe-cps-fg.conf"
SSL_CONF="$REPO_ROOT/docs/stunnel/openssl.cnf"
LOG="/tmp/dehempe-stunnel.log"
PIDFILE="/tmp/dehempe-stunnel.pid"

# Arrête un éventuel reliquat
if [[ -f "$PIDFILE" ]] && kill -0 "$(cat "$PIDFILE")" 2>/dev/null; then
    echo "Tunnel déjà actif (pid=$(cat "$PIDFILE")). Stop avec stop-tunnel.sh d'abord." >&2
    exit 1
fi

# Génère la conf foreground depuis la conf de référence (juste foreground=yes)
sed -e 's/^foreground.*$/foreground = yes/' \
    -e '/^output\b/d' -e '/^pid\b/d' \
    "$REPO_ROOT/docs/stunnel/dehempe-cps.conf" > "$CONF"

# Lance détaché ; le shell parent peut mourir sans tuer stunnel
nohup env OPENSSL_CONF="$SSL_CONF" stunnel "$CONF" > "$LOG" 2>&1 &
STUNNEL_PID=$!
echo "$STUNNEL_PID" > "$PIDFILE"
disown $STUNNEL_PID

# Attente courte que ça s'ouvre + sanity check
for i in 1 2 3 4 5; do
    if lsof -nP -iTCP:5443 2>/dev/null | grep -q LISTEN; then
        echo "stunnel up (pid=$STUNNEL_PID, log=$LOG)"
        exit 0
    fi
    sleep 1
done

echo "stunnel n'écoute pas sur 5443 après 5s, log :" >&2
tail -20 "$LOG" >&2
exit 1
