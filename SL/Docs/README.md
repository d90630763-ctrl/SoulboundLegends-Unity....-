# SL — Combined Final Project

This archive is the integrated project assembled from the supplied system ZIPs.

## Canonical duplicate handling
- Canonical lobby: `SLLobby_System_READY_MADE_v2.zip`
- Excluded duplicate lobby packages: `SLLobby_System.zip`, `SL_LobbySystem_FINAL.zip`
- Excluded wrapper duplicate: `FINAL_45_Unity_System_ZIPs_COMPLETE_SET.zip`; System 45 was extracted from it as the canonical System 45 source.

## Structure
- `Game/` — Unity 6000.5.6f1 project containing the integrated client-side systems and integration bootstrap.
- `Server/Services/` — backend/network/authority/database/auth/instance/commerce server components.
- `Server/Tests/` — supplied backend/transport/instance test sources where present.
- `Docs/` — provenance and integration manifest.

## Important
This is an integration-stage deliverable. The Unity Editor must regenerate `.meta` files and perform the authoritative compile/import pass. Real-device, multiplayer, load, security, payment, Google OAuth, database, server deployment and production testing remain intentionally outside this integration step.

Production secrets are not included.


D3D12 readiness: see Game/Assets/SL_D3D12_READINESS.md.
