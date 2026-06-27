# Prompts Bezy — HUD Points d'action

Spec : `Notes/GDD/SPEC_points_actions.md` (si présente)  
Script vue : `Assets/Scripts/UI/ActionPointsHudView.cs`  
**Ne pas rescanner tout le projet.**

---

## Historique

- **Phase 1** : prefab + instance NavigationHUD — **OK** (Bezy)
- **Micro P2** : layer 5 sur instance scène — **OK** ; layer 5 prefab source corrigé côté Cursor
- **Phase 3** : wiring Inspector — **à faire**

---

## Phase 3 — Wiring Inspector (prochaine étape)

```
Ne pas rescanner tout le projet.

Fichiers :
- Prefab : @Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
- Script : @Assets/Scripts/UI/ActionPointsHudView.cs (déjà créé par Cursor)
- Scène : @Assets/Scenes/NavigationHUD.unity (instance sous HUDRoot)

Tâche UNIQUE — wiring Inspector :

1. Sur la racine ActionPointsHudWidget du PREFAB :
   - Ajouter composant ActionPointsHudView
   - pointsLabel → enfant PointsLabel (TMP)
   - subtitleLabel → enfant SubtitleLabel (TMP)
   - barFillImage → ProgressBar/BarFill (Image, Type Filled)
   - barBackgroundImage → ProgressBar/BarBackground (Image)

2. Vérifier Layer 5 (UI) sur TOUS les GameObjects du prefab (déjà corrigé — ne pas repasser en Default).

3. Sur l'instance dans NavigationHUD : appliquer les overrides si le prefab a été modifié (Apply All si besoin).

Contraintes :
- Ne PAS modifier ActionPointService.cs, BiofiltreManager.cs
- Ne PAS implémenter la logique de débit PA dans ce script
- Ne PAS recréer la hiérarchie

Play Mode attendu :
- 240 / 240 au démarrage
- Sous-titre ≈ 24 h de travail
- Barre fill à 100%
- Après plantation : 239 / 240

Confirmer Inspector sans champs None + capture.
```

---

## Anti-patterns

- Pas de débit PA dans les scripts UI
- Pas de duplicate reset journalier
- Pas de recréation du prefab from scratch
