# Prompts Bezy — Halo inventaire (micro-anim slots)

**Statut :** Phase clips + wiring **livrés** (2026-07-23).  
**Prefab :** `Assets/Prefabs/Ui/Progression/PlayerHaloSlotUI.prefab`  
**GUID (ne pas changer) :** `a1931597dd60ec948aeb14c6a9ccfa34`  
**Assets anim :** `Assets/Animations/UI/PlayerHaloSlot.controller` (+ Idle / Click)  
**Hook Cursor :** `PlayerHaloPanelController` → `PlayTrigger("Click")`

---

## Note technique Bezy (workaround path prefab)

La modification directe des GameObjects existants dans ce prefab via les actions standard échouait systématiquement (bug de résolution de chemin sur les prefabs déjà présents sur disque, reproductible même sur d'autres prefabs du projet comme `ActionPointsHudWidget.prefab`). Contournement : instanciation temporaire du prefab dans la scène `Bootstrap.unity` (où la résolution de chemin fonctionne), application des modifications (Animator, câblage du champ, layers), puis régénération du prefab à son emplacement d'origine. Le GUID du prefab (`a1931597dd60ec948aeb14c6a9ccfa34`) a été vérifié inchangé, donc les 8 instances dans `PlayerHaloPanel.prefab` restent correctement liées.

Réf. playbook : `Notes/Bezi/README_bezi.md` § *Workaround Bezy — bug résolution de chemin*.

---

## Livré

| Élément | OK |
|---------|----|
| Idle breathe `AnimatedVisual` | oui |
| Trigger `Click` punch | oui |
| `animator` SerializeField câblé | oui |
| Layer UI 5 | oui |
| Pas de TestProbe résiduel | oui |
| Bootstrap sans instance temp | oui |

## Suite optionnelle

- Phase locked pulse sur `LockedOverlay` (slots réservés)
- Portrait centre `portraitAnimator` (panel)
