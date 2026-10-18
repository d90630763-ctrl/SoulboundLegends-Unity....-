from pathlib import Path
import json, zipfile, collections, re, sys

root = Path(__file__).resolve().parents[1]
game = root / "Game"
assert (game / "Packages/manifest.json").exists(), "Missing package manifest"
version = (game / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8").strip()
assert version.startswith("m_EditorVersion: 6000.5.6f1"), f"Unexpected Unity version: {version}"
json.loads((game / "Packages/manifest.json").read_text(encoding="utf-8"))
sources = list((game / "Assets").rglob("*.cs"))
assert len(sources) > 300, f"Unexpectedly few C# files: {len(sources)}"
for required in ["PlayerDatabaseBackend", "AuthenticationIdentity", "NetworkTransport",
                 "UniversalInstanceCoordinator", "RiftSystem", "SupremeRuinSystem",
                 "AudioMusicSystem"]:
    assert any(required.lower() in p.as_posix().lower() for p in (game / "Assets").rglob("*")), required

# Catch exact duplicate non-empty declaration lines in a single source file.
# This is a heuristic; it complements but cannot replace the Unity compiler.
suspects = []
decl = re.compile(r"^\\s*(?:(?:public|private|protected|internal|static|readonly|const|volatile|new|sealed|virtual|override|partial|async)\\s+)+[\\w<>\\[\\],.?]+\\s+\\w+\\s*(?:[=;({])")
for path in sources:
    lines = path.read_text(encoding="utf-8-sig", errors="replace").splitlines()
    seen = set()
    for line_no, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped and decl.match(line):
            key = re.sub(r"\\s+", " ", stripped)
            if key in seen:
                suspects.append(f"{path.relative_to(root)}:{line_no}: {stripped}")
            seen.add(key)
if suspects:
    print("Potential duplicate declaration lines (review; repeated members in different classes can be valid):")
    print("\\n".join(suspects[:100]))
else:
    print("No exact repeated declaration lines detected by heuristic.")
print(f"Structural validation passed: {len(sources)} C# files; Unity version {version.split(':',1)[1].strip()}")
