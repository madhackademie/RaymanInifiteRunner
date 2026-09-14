# Workspace Rules — à coller dans Bezy.ai (Unity)

**Mise à jour :** 2026-09-14 — remplace l’ancienne ligne « Cursor fournit tout le C# ».

Copier le bloc **ENGLISH** ci-dessous dans les **Workspace Rules** de Bezy (Settings du projet Unity).  
Référence repo : `Notes/Bezi/RULES_bezy_code.md`, `Notes/Bezi/README_bezi.md`.

---

## Bloc à coller (English — recommandé pour Bezy)

```
RaymanInfiniteRunner — Unity 6. Layer UI = 5 (not 4).

OWNERSHIP
- Bezy: prefabs, scenes, Inspector wiring, visual polish.
- Bezy ALSO owns SIMPLE C# on files listed in the prompt: RectTransform, scale, anchors, sprites, TMP, glow, SetActive, Animator triggers, SerializeField wiring on existing views (e.g. NavigationHUD tab mockup: zoom, lift, frame).
- Cursor/Codex: business logic, services, SceneNavigator, UIManager, PopupId, persistence, specs, phased prompts. Cursor does NOT implement visual transform tuning unless author says "without Bezy".

C# RULES (when prompt allows .cs)
- Read ONLY @ files and GameObjects named in the prompt. Do not rescan the whole project.
- Reuse existing patterns in the same file (e.g. copy TabAventures mockup to TabInventaire).
- FORBIDDEN unless author explicitly asks: SceneNavigator, UIManager, ScreenPopupHost, new services/singletons, FindObjectOfType in Update, new shaders/files not listed.
- No business logic refactor. No architecture changes.

PREFAB UI (heavy screens)
- Use 3 phases when asked: (1) hierarchy only (2) components only (3) wiring only. One phase per run. Wait for success.

SUCCESS
- Save. List files changed. STOP.
- Do NOT run Simulate / Play Mode / "confirm it looks good".

PROMPTS
- Follow task ID and phase in the user message. Character limit ~3500 if stated.
```

---

## Ancienne formulation à **retirer** dans Bezy

Si tu as encore une règle du type :

> « Cursor fournit scripts C#, specs et prompts phasés. Ne pas réécrire la logique métier en C# sauf demande explicite auteur. »

**Remplacer** par le bloc ci-dessus : elle mélangeait **tout** le C# (Cursor) alors que le **C# visuel HUD/onglets** est **Bezy** ; seule la **logique métier** reste interdite à Bezy sans demande explicite.

---

## Version courte (si limite de caractères Bezy)

```
Unity 6. UI layer 5. Bezy = prefabs, scenes, visual C# on listed files (transforms, NavigationHUD mockup tabs). Cursor = services/navigation/popups — do not touch those. No business logic, no full project scan. 3-phase prefab when asked. Save, list changes, STOP. No Play Mode.
```
