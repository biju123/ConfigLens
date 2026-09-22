# ConfigLens

Deployment configuration inspection and validation tool for Assessor
projects. See `CLAUDE.md` for the full product/architecture spec,
`docs/PLAN.md` / `docs/ARCHITECTURE.md` for how this MVP implements it, and
`docs/FRONTEND.md` for a frontend-specific developer guide (component
composition, Redux vs. local state, click-to-API-call flows).

ConfigLens is **read-only** - it never modifies Kubernetes resources, Azure
resources, application configuration, secrets, or deployments.

## What needs real infrastructure vs. sample data

Three of the four scan categories run entirely on deterministic sample data
baked into the backend - they work out of the box with no external
dependencies:

* Application Configuration Scan
* Configuration Characteristics Scan
* Dependency Accessibility Scan

The **AKS Deployment Scan** is different: it connects to a real Azure
subscription and a real AKS cluster's API server to read live inventory
(deployments, pods, cronjobs, jobs, services, HPAs, KEDA ScaledObjects). It
has its own prerequisites, covered in
["AKS Deployment Scan prerequisites"](#aks-deployment-scan-prerequisites)
below. Skip that section entirely if you only need the other three scan
categories.

## Prerequisites

Required for any scan category:

* [Docker](https://docs.docker.com/get-docker/) and Docker Compose v2 (the
  `docker compose` subcommand, bundled with Docker Desktop)

Only required for the AKS Deployment Scan:

* An Azure subscription containing the AKS cluster(s) you want to scan
* An Azure AD service principal (steps below) with Azure- and
  Kubernetes-level read access to that cluster
* [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
  (`az`), installed on your host machine - only needed to *create* the
  service principal below, not to run ConfigLens itself

Only required if you want to build/test outside Docker:

* [.NET SDK 10](https://dotnet.microsoft.com/download) for the backend
* [Node.js 20+](https://nodejs.org/) for the frontend

## Run it locally (Docker Compose)

```
scripts/start        # macOS/Linux
scripts\start.ps1     # Windows
```

This copies `.env.example` to `.env` on first run (edit
`CONFIGLENS_JWT_SIGNING_KEY` before using it beyond local development) and
runs `docker compose up --build -d`.

* Frontend: http://localhost:5173
* Backend: http://localhost:5080 (Swagger UI at `/swagger`)
* Sign in with `user` / `password` (CLAUDE.md section 5 - local dev only)

Stop with `scripts/stop` (or `scripts\stop.ps1` on Windows).

Without any further setup, every scan category except AKS Deployment Scan
works immediately. Attempting an AKS scan without completing the section
below fails with an authentication error - that's expected, not a bug.

## AKS Deployment Scan prerequisites

The backend authenticates to Azure using
[`DefaultAzureCredential`](https://learn.microsoft.com/dotnet/api/azure.identity.defaultazurecredential),
which - inside the Docker container ConfigLens ships in - resolves
credentials from the `AZURE_TENANT_ID` / `AZURE_CLIENT_ID` /
`AZURE_CLIENT_SECRET` environment variables. There is no Azure CLI inside
the container, so running `az login` on your host does **not** give the
backend container access; you must provision a service principal and put
its credentials in `.env`.

### 1. Create a service principal

On your host machine, with Azure CLI installed and logged in
(`az login`) as a user who can assign roles on the target subscription:

```
az ad sp create-for-rbac \
  --name "configlens-scanner" \
  --role "Reader" \
  --scopes "/subscriptions/<subscription-id>"
```

This prints `appId`, `password`, and `tenant` - map them to
`AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, and `AZURE_TENANT_ID`
respectively. Treat the `password` as a secret: it is only ever shown once.

`Reader` on the subscription lets ConfigLens list AKS clusters and resolve
cluster credentials via Azure Resource Manager. It does **not** by itself
grant access to the cluster's Kubernetes API - that's steps 2 and 3.

### 2. Grant the service principal cluster credentials access

Assign the built-in **Azure Kubernetes Service Cluster User Role** on each
AKS cluster (or its resource group) you want ConfigLens to scan:

```
az role assignment create \
  --assignee "<appId from step 1>" \
  --role "Azure Kubernetes Service Cluster User Role" \
  --scope "/subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.ContainerService/managedClusters/<cluster-name>"
```

This is what lets `AksClusterConnector` fetch a kubeconfig for the cluster
(`GetClusterUserCredentials`). Without it, cluster/namespace discovery and
the scan itself fail with an Azure Resource Manager authorization error.

### 3. Grant the service principal read access inside the cluster

Azure RBAC only gets you a kubeconfig - the identity still needs
**Kubernetes-level** RBAC permission to list the resources ConfigLens reads
(deployments, pods, cronjobs, jobs, services, HPAs, and KEDA
`ScaledObjects` if installed). If the cluster uses Azure AD + Kubernetes
RBAC integration, bind the service principal to a read-only ClusterRole,
e.g.:

```
kubectl create clusterrolebinding configlens-scanner-view \
  --clusterrole=view \
  --user="<appId from step 1>"
```

(Run this once, from a machine that already has `kubectl` access to the
cluster - it is a one-time cluster-side setup step, not something
ConfigLens itself needs at runtime.) If the cluster instead uses local
Kubernetes accounts (no Azure AD integration), the kubeconfig from step 2
already carries a cluster-admin certificate and this step can be skipped.

### 4. Network access to the AKS API server

The machine running the ConfigLens backend container must be able to reach
the cluster's API server over HTTPS:

* Public clusters: works from anywhere with outbound internet access.
* Private clusters: the backend must run somewhere with network
  line-of-sight to the private endpoint (VPN, ExpressRoute, or a
  jumpbox/self-hosted runner on the same virtual network) - a private AKS
  API server is not reachable from an arbitrary laptop on the public
  internet.

### 5. Configure ConfigLens

Add the three values from step 1 to your `.env` file (copied from
`.env.example`):

```
AZURE_TENANT_ID=<tenant from step 1>
AZURE_CLIENT_ID=<appId from step 1>
AZURE_CLIENT_SECRET=<password from step 1>
```

Restart the stack so the backend container picks up the new environment
variables:

```
docker compose up --build -d
```

Then, in the AKS Deployment Scan parameter form, enter the subscription ID
from step 1 and select the cluster/namespace - the "AKS cluster" and
"Namespace" dropdowns call the backend's discovery endpoints
(`GET /api/aks/clusters`, `GET /api/aks/namespaces`), which exercise this
same credential.

If authentication or connectivity is misconfigured, ConfigLens surfaces it
as a distinct infrastructure-failure error (not a generic 500) naming the
cluster - check that error message against steps 1-4 above first.

## Development (outside Docker)

Backend:
```
cd backend
dotnet build ConfigLens.slnx
dotnet test ConfigLens.slnx
```

Backend tests never need live Azure/AKS access - `ConfigLensApiFactory`
substitutes sample-data-backed adapters for the AKS scan in-process.

Frontend:
```
cd frontend
npm install
npm run dev      # requires the backend running locally (see .env.development)
npm test
npm run build
```

Running the backend with `dotnet run` instead of Docker lets
`DefaultAzureCredential` fall back to your host's `az login` session
(`AzureCliCredential`) instead of requiring the service principal from
steps 1-3 above - useful for local AKS scan development if you're already
signed in with `az login` and have access to a test cluster.
