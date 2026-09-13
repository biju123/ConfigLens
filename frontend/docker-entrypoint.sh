#!/bin/sh
set -e

# Generates the runtime config the already-built bundle reads at startup, so
# the same image can point at a different API URL per environment without
# rebuilding (CLAUDE.md section 20: no environment-specific config baked
# into images).
cat > /usr/share/nginx/html/env-config.js <<EOF
window.__CONFIGLENS_API_BASE__ = "${CONFIGLENS_API_BASE_URL:-}";
EOF

exec "$@"
