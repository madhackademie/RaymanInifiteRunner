# SPEC — Dossier sprites barre nav `Nav/Tabs`

**Création :** 2026-09-15  
**Scène :** `Assets/Scenes/NavigationHUD.unity`  
**Job Bezy (reassign) :** `[BZ-NAV-TABS-SPRITE-FOLDER-001]` (après déplacement auteur)

---

## Dossier cible (à créer dans Unity Project)

```
Assets/Art/Sprites/UI/Nav/Tabs/
```

Un seul dossier pour les **5 glyphes** de la barre du bas (mockup zoom actif).

---

## Tableau déplacement (Move dans Unity, pas copier)

| Onglet | Nom fichier **cible** | Emplacement **actuel** |
|--------|------------------------|-------------------------|
| Aventures | `IconeTab_Aventures_glyph.png` | `Assets/Art/Sprites/UI/Inventory/IconePlay.png` |
| Inventaire | `IconeTab_Inventaire_glyph.png` | `Assets/Art/Sprites/UI/Inventory/InventaireIconeBagPack.png` |
| Shop | `IconeTab_Shop_glyph.png` | `Assets/Art/Sprites/UI/Inventory/IconeMarket.png` |
| Vente | `IconeTab_Vente_glyph.png` | **Scène (2026-09-15) :** `Inventory/SellIcone.png` — spec Bezy V0 citait `Currency/GoldBill.png` (obsolète). Art cible = fichier unique dans `Nav/Tabs/`. |
| Plus | `IconeTab_Plus_glyph.png` | Feuille **composite** (cadre + 4 pictos dont « + ») — **validé auteur 2026-09-15** : fichier entier sur `Icon`, même mockup zoom/glow/cadre que Aventures/Inventaire/Shop/Vente. Pas de slice obligatoire. |

**Ne pas déplacer** (autres écrans / legacy) : `IconeInventaire.png`, dumps, etc. — seulement ce qui sert **NavigationHUD** `Tab*/IconLift/Icon`.

**Hors dossier `Nav/Tabs/` :** pas de `dump.png`, `Atelier_DIY.png`, ni autres assets hub — uniquement les **5** lignes du tableau.

**Rev. repo 2026-09-15 :** les 5 noms cibles existent sous `Nav/Tabs/` ; la scène pointe encore les **GUID** `Inventory/` (sauf Plus = sprite vide). Lancer `[BZ-NAV-TABS-SPRITE-FOLDER-001]` après import Unity + `.meta`.

---

## Procédure auteur (Unity)

1. Créer le dossier `Assets/Art/Sprites/UI/Nav/Tabs`.
2. **Glisser-déposer** chaque PNG depuis la colonne « actuel » vers `Nav/Tabs/` (Unity **Move**, pas Duplicate).
3. Renommer si besoin pour matcher la colonne « cible » (clic lent / F2).
4. Pour chaque texture : Inspector → **Sprite (2D and UI)** · PPU **100** · **Alpha Is Transparency** · **Trim** dans Sprite Editor (comme les autres onglets).
5. Vérifier `NavigationHUD` : les 4 onglets déjà câblés gardent le sprite (même **GUID** si Move Unity).
6. `TabMoreOption` : assigner `IconeTab_Plus_glyph.png` (Bezy P2 ou Inspector).

---

## Import settings (rappel)

| Paramètre | Valeur |
|-----------|--------|
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single |
| Pixels Per Unit | 100 |
| Mesh Type | Tight |
| Filter | Bilinear (ou project default) |

---

## Bezy — reassign si une ref casse

Fichier prompt : `Assets/Docs/Bezi/PROMPTS_Bezi_nav_tabs_sprite_folder.md` *(à coller après move)*.

Cible : `NavigationHUD.unity` — uniquement `Tab*/IconLift/Icon` Image sprites.

---

## Docs liées

- `Notes/Art/PROMPT_generation_icones.md` § H-nav mockup
- `Notes/Ui/SPEC_nav_onglets_zoom_actif.md`
- `[BZ-TAB-MORE-001]` Phase 2 avec `IconeTab_Plus_glyph.png`
