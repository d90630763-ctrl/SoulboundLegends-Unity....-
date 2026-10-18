# Soulbound Legends — pre-upload audit and cloud workflow

## Intended workflow
1. Commit the complete `SL/Game` Unity project to GitHub (including `Assets`, `Packages`, and `ProjectSettings`).
2. Configure Unity Cloud Build/Automation for Unity `6000.5.6f1`, Android, and the correct repository branch.
3. Trigger a build. `Assets/Editor/SLBuildAutomationPreprocessor.cs` invokes the editor scene-generation pipeline before player packaging.
4. Review the Unity build log and generated `Assets/Documentation/BUILDER_LAST_REPORT.md`.
5. Install the APK on the target phone, test, report issues, patch the project, and rebuild.

## Fix applied in this revision
- Removed a duplicate `sphereMeshCache` field declaration from `Assets/Runtime/SLProceduralActorFactory.cs`. The duplicate was a C# compilation blocker.

## Validation performed on this archive
- ZIP integrity checked.
- Unity project version and package manifest present.
- Structural source check run.
- Duplicate exact-line declaration scan run and reviewed for the confirmed duplicate.

## Important limits
This is a source/archive audit, not a Unity Editor compilation or a device test. A real Unity Cloud build is still required to confirm compiler compatibility, package resolution, generated-scene behavior, APK output, and runtime performance. The generated procedural actors/environments are stylized procedural assets; this pipeline does not create hand-authored AAA art automatically. Online multiplayer, authentication, database, billing, and anti-cheat features require real service configuration and security testing before release.
