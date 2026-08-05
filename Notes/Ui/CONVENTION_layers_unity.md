# Convention — Layers Unity (projet RaymanInfiniteRunner)

Référence pour **tous les prompts Bezy** et la revue UI. Source : `ProjectSettings/TagManager.asset`.

---

## Table des layers (indices `m_Layer`)

| Index `m_Layer` | Nom Unity | Usage projet |
|-----------------|-----------|----------------|
| **0** | Default | Gameplay 3D/2D, objets non-UI |
| 1 | TransparentFX | — |
| 2 | Ignore Raycast | — |
| 3 | *(vide)* | — |
| **4** | **Water** | **Pas l'UI** |
| **5** | **UI** | **Tout Canvas / Image / Button / TMP HUD** |

---

## Règle Bezy (obligatoire)

- Pour l'UI runtime : **layer = UI → `m_Layer: 5`** sur le Canvas **et** tous ses enfants.
- **Ne pas** mettre l'UI sur l'index **4** (c'est **Water** dans ce projet).
- **Ne pas** confondre « 5 layers nommés » avec « index max 4 » : l'UI est bien à l'**index 5** car le slot 3 est vide dans TagManager.

Snippet à coller dans les prompts Bezy :

```
Layer UI = m_Layer: 5 (TagManager : Water=4, UI=5).
Ne pas utiliser l'index 4 pour l'UI. Voir Notes/Ui/CONVENTION_layers_unity.md
```

---

## Piège fréquent (2026-06-29)

Bezy a signalé « UI = index 4 » — **faux pour ce repo**. Vérifier `TagManager.asset` avant de « corriger » des prefabs déjà en `m_Layer: 5`.

---

## Fichiers de référence déjà conformes

- `Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab` → `m_Layer: 5`
- `Assets/Scenes/NavigationHUD.unity` → éléments HUD en `m_Layer: 5`

## À aligner si encore en Default (0)

- Assets/Scenes/HomeScene.unity — Canvas UI OK ; Header ACCUEIL via `[BZ-POLISH-012]` Ph.3 ; prefab `MapNodeButton` layers via Ph.1

---

## Anti-patterns

- « Layer 5 » sans précision → toujours écrire **« UI (`m_Layer: 5`) »**
- Laisser des panneaux tooltip / zones interactives en Default (0) alors que le reste du HUD est en UI (5)
