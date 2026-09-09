# HUD biofiltre — prefab en dur, pas de moule unique

**Date :** 2026-09-07  
**Branche :** `main` (iso mergé)

---

## Décision auteur (2026-09-07)

Les biofiltres pourront avoir **des tailles différentes**. Un moule HUD unique (ancres normalisées + recale auto) n’est **pas** la cible.

**Pour commencer :** pose **à la main** sur **ce** biofiltre (`Biofiltre.prefab`), comme `IbcSprite`. Play = ce qui est déjà posé. Plus tard, chaque instance pourra overrider les transforms des rows.

---

## Ce qui ne marche pas (abandonné)

`Instantiate(hudPrefab)` au `Start` + gizmos Play + cache Edit :

- Rows absentes en Edit.
- Save interdit en Play.
- Recalcul d’ancres se bat avec un calage à l’œil sur cuve iso.

---

## État (Cursor, 2026-09-07)

```
Biofiltre
├── IbcSprite
├── Grid
├── Plants
└── BiofiltreHud          ← nested (pas unpack)
    ├── TopIsoLine
    │   ├── PrimaryRow
    │   └── StarRow
    └── SecondaryRow
```

- `BiofiltreHudBinder` référence l’enfant `hud` — **plus d’Instantiate**.
- Canvas world scale instance **0.01** (800 px → ~8 u monde) — à ajuster à l’œil.
- Play : **aucun recale** HUD / IBC (ne pas setter `renderMode` : Unity resettait scale/rotation).
- Cellules : toujours `GridManager.GridToWorldCenter` dans `BiofiltreGridVisualizer`.
- Gizmos cyan grille **seulement** si la racine `Biofiltre` est sélectionnée.

**Bezy nest `[BZ-FARM-BIOHUD-NEST-001]` :** skip — Cursor a nesté. Prompts conservés au cas où.

---

## Travail auteur (Prefab Mode)

Ouvrir `Assets/Prefabs/World/Biofiltre.prefab` :

1. Sélectionner `PrimaryRow` / `StarRow` / `SecondaryRow` (pas la racine Biofiltre).
2. Move / Rotate / Scale Unity — gauche / droite / bas-gauche sur la cuve.
3. Sauver le prefab. Playtest FirstLvl.

Ne pas unpack les nested rows.
