# [BZ-NAV-TABS-SPRITE-FOLDER-001] Reassign nav tab sprites after folder move

Run ONLY after author moved PNGs to `Assets/Art/Sprites/UI/Nav/Tabs/` (Unity Move).

OPEN Assets/Scenes/NavigationHUD.unity. m_Layer 5. Do NOT edit .cs. STOP after Save.

Assign Tab*/IconLift/Icon Image sprites ONLY:

| GameObject path | Sprite asset |
|-----------------|--------------|
| TabAventures/IconLift/Icon | Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Aventures_glyph.png |
| TabInventaire/IconLift/Icon | Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Inventaire_glyph.png |
| TabShop/IconLift/Icon | Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Shop_glyph.png |
| TabVente/IconLift/Icon | Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Vente_glyph.png |
| TabMoreOption/IconLift/Icon | Assets/Art/Sprites/UI/Nav/Tabs/IconeTab_Plus_glyph.png *(feuille composite — texture entière, comme les autres onglets)* |

Each Icon: Preserve Aspect ON, Raycast OFF, color white alpha 1 when sprite set.
Do NOT change RectTransform layout, glow, labels, wiring.

Save. List each tab + sprite path assigned. STOP. No Play Mode.

Launch:
```
@Assets/Docs/Bezi/PROMPTS_Bezi_nav_tabs_sprite_folder.md
[BZ-NAV-TABS-SPRITE-FOLDER-001] Reassign all 5 tab icons to Nav/Tabs folder. STOP.
```
