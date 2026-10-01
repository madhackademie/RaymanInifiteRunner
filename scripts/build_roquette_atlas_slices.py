#!/usr/bin/env python3
"""Recalcule 7 slices Roquette depuis AtlasRoquette.png (grille 7 col + trim alpha)."""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    print("pip install pillow", file=sys.stderr)
    raise

ROOT = Path(__file__).resolve().parents[1]
ATLAS = ROOT / "Assets/Art/Sprites/Plantes/AtlasRoquette.png"
META = ATLAS.with_suffix(".png.meta")

NAMES = [
    "Roquette_01_Graine",
    "Roquette_02_Starting",
    "Roquette_03_Baby",
    "Roquette_04_Growing",
    "Roquette_05_Mature",
    "Roquette_06_Flowering",
    "Roquette_07_Seedling",
]

# Stable internal IDs (Unity sub-sprite fileID)
INTERNAL_IDS = [
    410001001,
    410001002,
    410001003,
    410001004,
    410001005,
    410001006,
    410001007,
]


def compute_rects() -> list[dict]:
    im = Image.open(ATLAS).convert("RGBA")
    w, h = im.size
    cols = 7
    cw = w // cols
    alpha = im.split()[3]
    rects = []
    for i, name in enumerate(NAMES):
        x0 = i * cw
        x1 = w if i == cols - 1 else (i + 1) * cw
        col = alpha.crop((x0, 0, x1, h))
        bb = col.getbbox()
        if not bb:
            raise RuntimeError(f"empty column {i + 1} {name}")
        lx, ty, rx, by = bb
        gx = x0 + lx
        gw = rx - lx
        gh = by - ty
        uy = h - by
        rects.append({"name": name, "x": gx, "y": uy, "w": gw, "h": gh})
    return rects


def sprite_yaml_block(rect: dict, internal_id: int) -> str:
    name = rect["name"]
    return f"""    - serializedVersion: 2
      name: {name}
      rect:
        serializedVersion: 2
        x: {rect['x']}
        y: {rect['y']}
        width: {rect['w']}
        height: {rect['h']}
      alignment: 0
      pivot: {{x: 0, y: 0}}
      border: {{x: 0, y: 0, z: 0, w: 0}}
      customData: 
      outline: []
      physicsShape: []
      tessellationDetail: -1
      bones: []
      spriteID: 
      internalID: {internal_id}
      vertices: []
      indices: 
      edges: []
      weights: []"""


def patch_meta(rects: list[dict]) -> None:
    text = META.read_text(encoding="utf-8")
    id_table_lines = "\n".join(
        f"      {NAMES[i]}: {INTERNAL_IDS[i]}" for i in range(len(NAMES))
    )
    id_table = f"    nameFileIdTable:\n{id_table_lines}"

    internal_table = "\n".join(
        f"  - first:\n      213: {INTERNAL_IDS[i]}\n    second: {NAMES[i]}"
        for i in range(len(NAMES))
    )

    sprites_body = "\n".join(
        sprite_yaml_block(rects[i], INTERNAL_IDS[i]) for i in range(len(rects))
    )

    new_sheet = f"""  internalIDToNameTable:
{internal_table}
  externalObjects: {{}}
  serializedVersion: 13"""

    # Replace internalIDToNameTable through start of spriteSheet only if we do full file surgery
    # Simpler: replace spriteSheet section only
    pattern = re.compile(
        r"  spriteSheet:\n    serializedVersion: 2\n    sprites:.*?    nameFileIdTable:.*?\n(?:      .+\n)+",
        re.DOTALL,
    )
    replacement = f"""  spriteSheet:
    serializedVersion: 2
    sprites:
{sprites_body}
    outline: []
    customData: 
    physicsShape: []
    bones: []
    spriteID: 
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
{id_table}
"""
    if not pattern.search(text):
        raise RuntimeError("spriteSheet block not found in .meta")
    text = pattern.sub(replacement, text)

    id_name_pattern = re.compile(
        r"  internalIDToNameTable:.*?  externalObjects:",
        re.DOTALL,
    )
    text = id_name_pattern.sub(
        f"  internalIDToNameTable:\n{internal_table}\n  externalObjects:",
        text,
        count=1,
    )

    META.write_text(text, encoding="utf-8")


def main() -> None:
    rects = compute_rects()
    print(json.dumps(rects, indent=2))
    patch_meta(rects)
    print(f"Patched {META.relative_to(ROOT)}")


if __name__ == "__main__":
    main()
