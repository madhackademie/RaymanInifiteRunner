# Prompts Bezy — TalentNode Idle breathe

**Statut :** livré (2026-07-23).  
**Prefab :** `Assets/Prefabs/Ui/Progression/TalentNodeView.prefab`  
**GUID (ne pas changer) :** `0f1b14c68efb3324ba77f23eb509d0c8`  
**Assets :** `Assets/Animations/UI/TalentNode.controller`, `TalentNode_Idle.anim`  
**Hook Cursor :** aucun (Idle = state défaut)

---

## Livré

| Élément | OK |
|---------|----|
| Idle loop path `Icon` 1.00→1.05→1.00 (~1.4s) | oui |
| Animator + Unscaled Time | oui |
| Layer UI 5 | oui |
| GUID inchangé / `Track_Commerce` | oui |
| Pas de Probe | oui |

## Playtest

Inventaire → slot Commerce → overlay : chaque nœud pulse légèrement sur l’icône.

## Suite optionnelle

- Trigger punch à l’achat (nécessite petit hook Cursor)
- Pulse `AvailableOverlay` seulement
