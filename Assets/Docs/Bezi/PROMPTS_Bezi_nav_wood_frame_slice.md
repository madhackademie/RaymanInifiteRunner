# Prompts Bezy — cadre bois onglets nav `[BZ-NAV-WOOD-REFONTE-001]`

**Statut :** **polish backlog** — pas avant mockup zoom OK sur 4 onglets + budget auteur (« quand le fric rentre »).  
**Base à garder :** mockup actuel (glow + zoom + label TMP) — **ne pas casser** pour la refonte bois.  
**Problème actuel :** `cadreBoisFinal.png` en stretch simple → déformé ; cadre **recouvre le texte** « Inventaire » / labels onglets.  
**Art :** `Assets/Art/Assets Store Dump/Ui/cadreBoisFinal.png` → promo `Sprites/UI/Nav/` quand rendu validé.  
**Règles :** `@Notes/Bezi/RULES_bezy_code.md` · scène + `NavigationHUD.cs` visuel autorisé.

**Succès Bezy = Save + liste changements. STOP. Pas Simulate.**

---

## Phase 1 — Import 9-slice + WoodFrame Sliced (pilote TabAventures)

```
[BZ-NAV-WOOD-REFONTE-001] PHASE 1 — cadreBoisFinal 9-slice + WoodFrame Sliced. STOP.

OPEN NavigationHUD.unity. @Notes/Bezi/RULES_bezy_code.md
Files: cadreBoisFinal.png (+ .meta), NavigationHUD.unity — TabAventures/SelectedFrame/WoodFrame only.

1) Sprite Import: Single sprite, Full Rect, PPU 100, alpha transparency ON.
2) Sprite Editor: 9-slice borders — corners/rivets fixed; bottom fish plaque readable (tune L~240 R~240 T~200 B~380 start).
3) WoodFrame Image: Type **Sliced**, stretch inside SelectedFrame, raycast OFF.
4) Do NOT cover Tab label TMP — WoodFrame must stay **behind** IconLift and **above** slot bg only; label remains visible below icon (match active mockup layout).

Save. Report border L,R,T,B. STOP. No Play Mode.
```

---

## Phase 2 — Hiérarchie : cadre n’empiète pas sur le label (4 onglets)

```
[BZ-NAV-WOOD-REFONTE-001] PHASE 2 — layout cadre vs label. P1 OK. STOP.

OPEN NavigationHUD.unity. All 4 tabs after mockup zoom complete (Inventaire/Shop/Vente parity).

Per tab: SelectedFrame/WoodFrame englobe icon+zoom area only; Label TMP **sibling after IconLift**, bottom of cell, NOT child of WoodFrame.
SelectedFrame repos: Inventaire/Shop (0,0); Aventures (+10 X); Vente (-10 X repos) per Todo.
Runtime expand: keep NavigationHUD mockup frame expand (do not bake active zoom in scene).

Save. List hierarchy per tab. STOP.
```

---

## Phase 3 — Rollout + désactiver bordures or legacy si bois OK

```
[BZ-NAV-WOOD-REFONTE-001] PHASE 3 — rollout wood, gold borders OFF. P2 OK. STOP.

NavigationHUD.unity: WoodFrame on each tab SelectedFrame; BorderTop/Bottom/Left/Right OFF when WoodFrame ON.
Wire unchanged. NavigationHUD.cs: only if needed to toggle WoodFrame with SelectedFrame active state.

Save. STOP.
```
