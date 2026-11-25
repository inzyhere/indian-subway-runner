#!/usr/bin/env python3
"""Import the separately delivered original Indian Subway Runner starter asset pack.

Usage:
  python3 Tools/Assets/import_starter_asset_pack.py /path/to/IndianSubwayRunner_AssetPack_v0.1.zip

The importer only copies this pack's own GLB, WAV, text and manifest files. It
never reads or copies the Subway Surfers reverse-engineering archive.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import sys
import zipfile
from pathlib import Path, PurePosixPath

REPO_ROOT = Path(__file__).resolve().parents[2]
DESTINATION = REPO_ROOT / "godot" / "assets" / "indian_pack_v01"
ARCHIVE_ROOT = "IndianSubwayRunner_AssetPack/"
ALLOWED_SUFFIXES = {".glb", ".wav", ".txt", ".json", ".md"}
ALLOWED_TOP_LEVEL = {"models", "environment", "audio", "docs"}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("archive", type=Path, help="Path to the starter asset ZIP")
    parser.add_argument("--overwrite", action="store_true", help="Replace already imported files")
    args = parser.parse_args()

    archive = args.archive.expanduser().resolve()
    if not archive.is_file():
        parser.error(f"Archive does not exist: {archive}")

    copied: list[dict[str, str | int]] = []
    skipped: list[str] = []
    DESTINATION.mkdir(parents=True, exist_ok=True)

    try:
        with zipfile.ZipFile(archive) as bundle:
            for info in bundle.infolist():
                if info.is_dir() or not info.filename.startswith(ARCHIVE_ROOT):
                    continue
                relative = PurePosixPath(info.filename[len(ARCHIVE_ROOT):])
                if relative.is_absolute() or ".." in relative.parts or not relative.parts:
                    raise ValueError(f"Unsafe archive path: {info.filename}")
                if relative.parts[0] not in ALLOWED_TOP_LEVEL:
                    continue
                if relative.suffix.lower() not in ALLOWED_SUFFIXES:
                    continue

                target = DESTINATION.joinpath(*relative.parts)
                target.parent.mkdir(parents=True, exist_ok=True)
                if target.exists() and not args.overwrite:
                    skipped.append(relative.as_posix())
                    continue

                data = bundle.read(info)
                target.write_bytes(data)
                copied.append({
                    "path": relative.as_posix(),
                    "bytes": len(data),
                    "sha256": hashlib.sha256(data).hexdigest(),
                })
    except (zipfile.BadZipFile, OSError, ValueError) as exc:
        print(f"Import failed: {exc}", file=sys.stderr)
        return 2

    report = {
        "source_archive": archive.name,
        "destination": "godot/assets/indian_pack_v01",
        "copied_files": copied,
        "skipped_existing_files": skipped,
    }
    report_path = DESTINATION / "import_manifest.json"
    report_path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Imported {len(copied)} files into {DESTINATION}")
    if skipped:
        print(f"Skipped {len(skipped)} existing files (use --overwrite to replace).")
    print(f"Import manifest: {report_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
