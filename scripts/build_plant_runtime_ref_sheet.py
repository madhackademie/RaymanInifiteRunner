#!/usr/bin/env python3
"""Assemble separate plant stage PNGs into one horizontal reference sprite sheet."""

from __future__ import annotations

import argparse
from pathlib import Path

from PIL import Image

# Laitue runtime order (PlantGrow / Laitue.asset)
LAITUE_STAGES = [
    "0_GraineGermé.png",
    "01_StartingPlant.png",
    "02_BabyLaituce_image.png",
    "03_GrowingLaituce_image.png",
    "04_MatureLaituce_image.png",
    "05_FlowerLaituce_image.png",
    "06_SeedlingLaituce_image.png",
]


def fit_in_cell(image: Image.Image, cell_size: int, margin_ratio: float = 0.08) -> Image.Image:
    """Center sprite in a transparent square cell, uniform scale across all stages."""
    cell = Image.new("RGBA", (cell_size, cell_size), (0, 0, 0, 0))
    usable = int(cell_size * (1.0 - margin_ratio * 2))
    src = image.convert("RGBA")
    w, h = src.size
    scale = min(usable / w, usable / h)
    new_w = max(1, int(w * scale))
    new_h = max(1, int(h * scale))
    resized = src.resize((new_w, new_h), Image.Resampling.LANCZOS)
    x = (cell_size - new_w) // 2
    # Anchor toward bottom (soil line) — sprites sit on lower third
    y = cell_size - new_h - int(cell_size * margin_ratio)
    cell.paste(resized, (x, y), resized)
    return cell


def build_sheet(
    input_dir: Path,
    filenames: list[str],
    cell_size: int,
    output_path: Path,
) -> None:
    cells: list[Image.Image] = []
    for name in filenames:
        path = input_dir / name
        if not path.exists():
            raise FileNotFoundError(f"Missing sprite: {path}")
        cells.append(fit_in_cell(Image.open(path), cell_size))

    width = cell_size * len(cells)
    sheet = Image.new("RGBA", (width, cell_size), (0, 0, 0, 0))
    for i, cell in enumerate(cells):
        sheet.paste(cell, (i * cell_size, 0), cell)

    output_path.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(output_path, "PNG")
    print(f"Saved {output_path} ({width}x{cell_size}, {len(cells)} frames)")


def main() -> None:
    root = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description="Build plant runtime reference sprite sheet")
    parser.add_argument(
        "--input",
        type=Path,
        default=root / "Assets/Art/Sprites/Plantes/Laitue",
        help="Folder with stage PNGs",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=root / "Assets/Art/Assets Store Dump/Plantes/Laitue/Laitue_Runtime_Ref_7stades.png",
        help="Output PNG path",
    )
    parser.add_argument("--cell", type=int, default=512, help="Cell size in pixels")
    args = parser.parse_args()

    build_sheet(args.input, LAITUE_STAGES, args.cell, args.output)


if __name__ == "__main__":
    main()
