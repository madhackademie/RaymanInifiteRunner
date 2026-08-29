# Skill Bezi — Prefab UI 3 phases (brouillon)

**Usage :** créer un skill custom dans Bezi (Workspace Settings → Skills → **+ Create with Bezi**) en collant la section **Instructions du skill** ci-dessous.

**Commande suggérée :** `/prefab-ui-3phases`

**Référence projet :** `.cursor/rules/bezy_execution_phases.mdc`, `Notes/Ui/CONVENTION_layers_unity.md`

---

## Métadonnées skill (pour Bezi)

| Champ | Valeur |
|-------|--------|
| **Nom** | `prefab-ui-3phases` |
| **Description** | Crée ou modifie un prefab UI en 3 phases : hiérarchie → composants → wiring. Conventions RaymanInfiniteRunner. |
| **Slash** | `/prefab-ui-3phases` |

---

## Instructions du skill (à donner à Bezi pour création)

Coller ce bloc dans « Create with Bezi » ou dans un thread Agent Mode :

```
Create a custom Bezi skill named "prefab-ui-3phases" with slash command /prefab-ui-3phases.

Purpose: Standardize UI prefab work for Unity project RaymanInfiniteRunner using a strict 3-phase workflow. One phase per invocation unless the user explicitly names a single phase (e.g. "Phase 2 only").

WHEN TO RUN:
- User asks to create or modify a UI prefab, HUD widget, popup shell, or inventory/shop screen layout.
- User provides or @mentions a target prefab path under Assets/Prefabs/Ui/.
- User references a PROMPTS_Bezi_*.md task ID like [BZ-XXX-NNN].

INPUTS (ask if missing):
1. Task ID (e.g. [BZ-INV-TABS-001])
2. Target prefab path (exact, e.g. Assets/Prefabs/Ui/InventoryScreen.prefab)
3. Optional: scene to keep open (Bootstrap.unity or prefab mode)
4. Optional: script paths to wire in Phase 3 (read-only, do not edit C# unless asked)
5. Phase number: 1, 2, 3, or "all" (if "all", still execute ONLY phase 1 first and STOP)

GLOBAL RULES (every phase):
- Do not rescan whole project.
- Reuse existing scripts; do not create or edit C# unless user explicitly requests.
- UI layer: m_Layer 5 on Canvas root and ALL UI children (Water is index 4 — never use 4 for UI).
- Do not run Simulate or Play Mode. Do not ask for visual confirmation.
- End every phase with: Save. List changed files and GameObject paths. STOP.

PHASE 1 — Hierarchy shell only:
- Create or update prefab at the exact path given.
- GameObjects + RectTransform layout only.
- NO Image, Button, TMP, LayoutGroup, Animator, or SerializeField wiring.
- Preserve existing children and SerializeFields on controllers unless task says otherwise.
- Output checklist: parent paths, new GO names, layer verification (all m_Layer 5).

PHASE 2 — Components only:
- Add Image, Button, TextMeshProUGUI, LayoutGroup, LayoutElement, CanvasGroup, etc.
- NO SerializeField wiring to scripts yet.
- NO OnClick events yet.
- Match project visual baseline: dark panels ~0.12,0.12,0.16 alpha 0.9, TMP white center, min tap 48px.
- Output checklist: components added per GameObject.

PHASE 3 — Wiring only:
- Assign SerializeField references on existing view/controller components.
- Wire Button.onClick and similar events to public methods on assigned scripts.
- Do not change hierarchy or add new visual components unless fixing a broken reference.
- Prerequisite: target prefab or scene MUST be open in Editor (path resolution workaround).
- Output checklist: each SerializeField filled, each event wired, any missing manual link flagged.

WORKAROUND — If direct prefab-on-disk edit fails (path resolution bug):
1. Instantiate prefab temporarily in Bootstrap.unity.
2. Apply changes on instance.
3. Apply prefab overrides back to original path.
4. Verify prefab GUID unchanged.
5. Remove temporary instance.

ON COMPLETION:
- Remind user: playtest is author step, not Bezi.
- Suggest next phase command: "/prefab-ui-3phases Phase 2 ..." if phase 1 succeeded.
```

---

## Exemple d'invocation (après install du skill)

### Phase 1 seule

```
/prefab-ui-3phases

[BZ-EXAMPLE-001] Phase 1 ONLY
Prefab: Assets/Prefabs/Ui/ShopItemPopup.prefab
Open prefab in Prefab Mode before starting.

Add under Root:
- HeaderRow (empty)
- ContentArea (empty)
- FooterRow (empty)

Do not touch ShopItemPopupView.cs. STOP after Phase 1.
```

### Phase 3 après scripts Cursor

```
/prefab-ui-3phases

[BZ-EXAMPLE-001] Phase 3 ONLY
Prefab: Assets/Prefabs/Ui/ShopItemPopup.prefab
Wire ShopItemPopupView.cs SerializeFields per Notes/Ui/PROMPTS_Bezi_shop_popup.md Phase 3.
STOP after save.
```

---

## Checklist auteur (hors Bezi)

Après chaque phase :

- [ ] Diff Git : prefab/scène attendus uniquement
- [ ] `m_Layer: 5` sur tous les GO UI
- [ ] GUID prefab inchangé (si workaround Bootstrap)
- [ ] Pas de modification C# non demandée
- [ ] Playtest / Simulate **après** Phase 3 complète

---

## Limites connues (doc Bezi)

- Skills : locaux au compte, pas partagés auto en team workspace.
- Skills : JavaScript only dans le skill ; pas de shell.
- Bezy ne valide pas le rendu runtime — playtest auteur obligatoire.
