# SL — Testing Handoff

After opening `Game` in Unity `6000.5.6f1`, perform the real testing stage.

## First pass
1. Let Unity regenerate `.meta` files and import all assets.
2. Check the Console for compile/import errors.
3. Open the ready-made lobby demo/setup supplied by the canonical lobby package.
4. Verify the Combined Bootstrap initializes without errors.

## Core gameplay
- Account/login flow
- Character creation
- Lobby navigation
- Level progression
- Level 10 mode unlock
- Adventure World entry
- Village sharding
- Movement/combat/skills
- Monster/NPC generation
- Terrain/water/destruction
- Crafting/alchemy/equipment
- Quests/world events
- CS/BR/Siege
- Social/party/chat/mail
- HUD/settings/mobile controls

## Special systems
- Rift lifecycle and city consequences
- Supreme Ruin entrance requirements
- Exactly five Divine Creatures
- God inheritance claim and persistence
- Audio/music contexts
- Skill VFX
- Animation/IK generation
- Finisher eligibility (future/live-update controlled feature)
- Live update/version/rollback flows

## Multiplayer/backend
- Authentication and Google identity provider
- Session/reconnect
- Database persistence
- Server authority
- Network transport
- AOI
- Instance creation/transfer/destruction
- Matchmaking
- Anti-cheat/security
- Economy transaction integrity

## Production validation
- Android device performance
- iOS/device performance where applicable
- low-memory devices
- poor network conditions
- packet loss/jitter
- reconnects
- concurrent players
- server load
- database load
- backups/restores
- security/penetration testing
- payment sandbox and production flows
- CDN/live-update deployment

## Important
Do not put production secrets into the Unity project ZIP. Use the server/deployment secret configuration described in `Server/production.env.template` and the individual service configuration documentation.
