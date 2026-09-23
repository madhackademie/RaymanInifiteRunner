# Bezy — prompts encore à faire (index unique)

**MAJ :** 2026-09-23 — les anciens `Notes/Ui/PROMPTS_Bezi_*.md` sont supprimés (doublons / livrés).  
**Texte complet des prompts :** `Assets/Docs/Bezi/PROMPTS_Bezi_*.md` (fichier `@` dans Unity).  
**Statut opérationnel :** cocher dans `Notes/Bezi/BEZY_QUEUE.md` après chaque phase.

**Livrés / historique :** `Notes/Bezi/ARCHIVE_prompts_bezi_index.md`

---

## Priorité immédiate (file courante)

| Task ID | Phase | Notes |
|---------|-------|--------|
*Vide — `[BZ-FARM-CAMERA-TOUCH-ZOOM-001]` clos 2026-09-23 (Ph.2 Inspector déjà OK).*

Blocs copier-coller : section **Bloc de lancement** dans `BEZY_QUEUE.md`.

---

## Backlog — rework UI kit

| Task ID | Statut | Notes |
|---------|--------|--------|
| `[BZ-UIKIT-POPUP-MOCK-001]` | **À refaire entièrement** | Prefab `ShopItemPopup_WoodMockup` — aperçu 2026-09-23 : quelque chose en place mais pas validable. Repartir Ph.1–3 ou brief visuel auteur + prompts Bezy neufs. **Hors** popup runtime `ShopItemPopup`. |

---

## Farm / caméra + VFX

**`[BZ-FARM-CAMERA-VIEW-001]`** — clos 2026-09-23. **`[BZ-FARM-HARVEST-READY-VFX-002]`** — clos 2026-09-23 (sparkles récolte, Ph.5–5f Bezy).

---

## Polish / backlog (pas P0 session)

| Task ID | Notes | Prompt `@` |
|---------|-------|------------|
| `[BZ-NAV-TAB5-CLIP-001]` | 5ᵉ onglet coupé — scène | `Assets/Docs/Bezi/PROMPTS_Bezi_nav_tab5_clip_fix.md` |
| `[BZ-NAV-WOOD-FRAME-001]` / refonte | 9-slice bois onglets — après mockups 4 onglets | `Assets/Docs/Bezi/PROMPTS_Bezi_nav_wood_frame_slice.md` |
| `[CT-FARM-BIO-SCALE-001]` | Scale mobile biofiltre | `Assets/Docs/Bezi/PROMPTS_Bezi_biofilter_mobile_scale.md` |

---

## Règle d’usage

1. Ouvrir le `.md` listé ci-dessus dans `Assets/Docs/Bezi/`.
2. Nouveau thread Bezy → `@` ce fichier + `@Notes/Bezi/RULES_bezy_code.md` si C# visuel.
3. Prefab UI : `/prefab-ui-3phases` + **une phase** par appel.
4. Cocher `BEZY_QUEUE.md` + commit prefab (auteur).
