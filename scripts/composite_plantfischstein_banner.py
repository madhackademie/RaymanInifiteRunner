"""Composite Plantfischstein onto FirstTryBandeauAtelier (Dump art)."""
from __future__ import annotations

from pathlib import Path

from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
BANNER = ROOT / "Assets/Art/Assets Store Dump/Ui/Tab_Plus/FirstTryBandeauAtelier.png"
CHIMERA = Path(
    r"C:\Users\madbox\.cursor\projects\m-ProjetUnity6-RaymanInifiteRunner\assets\Plantfischstein_chimera.png"
)
OUT = ROOT / "Assets/Art/Assets Store Dump/Ui/Tab_Plus/BandeauAtelier_Plantfischstein_20260917.png"


def white_to_alpha(img: Image.Image, threshold: int = 248) -> Image.Image:
    img = img.convert("RGBA")
    pixels = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = pixels[x, y]
            if r >= threshold and g >= threshold and b >= threshold:
                pixels[x, y] = (r, g, b, 0)
    return img


def trim_alpha(img: Image.Image, pad: int = 4) -> Image.Image:
    alpha = img.split()[3]
    bbox = alpha.getbbox()
    if not bbox:
        return img
    left, top, right, bottom = bbox
    left = max(0, left - pad)
    top = max(0, top - pad)
    right = min(img.width, right + pad)
    bottom = min(img.height, bottom + pad)
    return img.crop((left, top, right, bottom))


def main() -> None:
    banner = Image.open(BANNER).convert("RGBA")
    chimera = trim_alpha(white_to_alpha(Image.open(CHIMERA)))

    bw, bh = banner.size
    # Placement tuned for FirstTryBandeauAtelier (~fish+leek on table, left of jar)
    # Placement v2 — couvrir truite + poireau d’origine
    target_w = int(bw * 0.40)
    scale = target_w / chimera.width
    target_h = int(chimera.height * scale)
    chimera = chimera.resize((target_w, target_h), Image.Resampling.LANCZOS)

    r, g, b, a = chimera.split()
    a = a.filter(ImageFilter.GaussianBlur(radius=1))
    chimera = Image.merge("RGBA", (r, g, b, a))

    cx = int(bw * 0.42)
    cy = int(bh * 0.74)
    x = cx - target_w // 2
    y = cy - int(target_h * 0.52)

    # Masque local : recouvrir l’ancienne truite/poireau (bois table voisin)
    px0, py0 = int(bw * 0.30), int(bh * 0.60)
    px1, py1 = int(bw * 0.58), int(bh * 0.80)
    ref = banner.crop((px1 + 8, py0, min(bw, px1 + 120), py1))
    if ref.width > 0:
        ref = ref.resize((px1 - px0, py1 - py0), Image.Resampling.LANCZOS)
        feather = Image.new("L", (px1 - px0, py1 - py0), 255)
        feather = feather.filter(ImageFilter.GaussianBlur(radius=10))
        banner.paste(ref, (px0, py0), feather)

    banner.alpha_composite(chimera, (x, y))
    OUT.parent.mkdir(parents=True, exist_ok=True)
    banner.save(OUT, "PNG")
    print(f"Saved {OUT} ({bw}x{bh}) chimera at ({x},{y}) size {target_w}x{target_h}")


if __name__ == "__main__":
    main()
