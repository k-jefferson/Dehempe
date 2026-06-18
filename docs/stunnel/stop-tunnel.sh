#!/usr/bin/env bash
# Arrête le tunnel mTLS Déhempé lancé par start-tunnel.sh.

set -euo pipefail

PIDFILE="/tmp/dehempe-stunnel.pid"

if [[ ! -f "$PIDFILE" ]]; then
    echo "Pas de PID file ($PIDFILE) — tunnel probablement déjà arrêté."
    exit 0
fi

PID="$(cat "$PIDFILE")"
if kill -0 "$PID" 2>/dev/null; then
    kill "$PID"
    sleep 1
    if kill -0 "$PID" 2>/dev/null; then
        kill -9 "$PID"
    fi
    echo "stunnel arrêté (pid=$PID)"
else
    echo "Process $PID n'existe plus."
fi

rm -f "$PIDFILE"
