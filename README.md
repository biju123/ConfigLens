# ConfigLens

Deployment configuration inspection and validation tool for Assessor
projects. See `CLAUDE.md` for the full product/architecture spec and
`docs/PLAN.md` / `docs/ARCHITECTURE.md` for how this MVP implements it.

## Run it locally

```
scripts/start        # macOS/Linux
scripts\start.ps1     # Windows
```

This copies `.env.example` to `.env` on first run (edit
`CONFIGLENS_JWT_SIGNING_KEY` before using it beyond local development) and
runs `docker compose up --build -d`.

- Frontend: http://localhost:5173
- Backend: http://localhost:5080 (Swagger UI at `/swagger`)
- Sign in with `user` / `password` (CLAUDE.md section 5 — local dev only)

Stop with `scripts/stop` (or `scripts\stop.ps1` on Windows).

## Development

Backend:
```
cd backend
dotnet build ConfigLens.slnx
dotnet test ConfigLens.slnx
```

Frontend:
```
cd frontend
npm install
npm run dev      # requires the backend running locally (see .env.development)
npm test
npm run build
```
