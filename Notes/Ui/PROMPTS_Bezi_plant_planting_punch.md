# Prompts Bezy — Punch scale pose graine `[BZ-POLISH-017]`

**Prefab cible :** `Assets/Prefabs/World/Plantes/LaitueObj.prefab` (et tout autre prefab plante au même format)  
**Script Cursor (déjà prêt, ne pas recréer) :** `Assets/Scripts/Farm/PlantPlantingPunch.cs`  
**Hook runtime :** `BiofiltreManager.PlantSeedAtInternal` → `PlantPlantingPunch.Play()` (placement joueur only)

**Règles :** ne pas rescanner tout le projet ; ne pas créer de nouveaux scripts C# ; ne pas toucher `PlantingDirtBurst` ; pas de Play Mode / Simulate.

**Pourquoi pas d’Animator :** les sliders Inspector `[Range(1,5)]` / durées doivent rester retunable par l’auteur. Le punch est piloté par le script ; Bezy = branchement prefab + valeurs par défaut.

---

## Phase 1 — Brancher `PlantPlantingPunch` sur LaitueObj

```
[BZ-POLISH-017] Phase 1 ONLY — wire PlantPlantingPunch on LaitueObj. Wait success. STOP after.

Do NOT rescan whole project. Do NOT create scripts. Do NOT create Animator / AnimationClip.
Do NOT modify PlantingDirtBurst. Do NOT change PlantGrow / sprite / collider.

Open ONLY: Assets/Prefabs/World/Plantes/LaitueObj.prefab

On root LaitueObj:
1. Add component PlantPlantingPunch (script Assets/Scripts/Farm/PlantPlantingPunch.cs)
2. Set fields:
   - peakScaleMultiplier = 3
   - zoomInDurationSeconds = 2
   - zoomOutDurationSeconds = 2.5
   - scaleTarget = leave empty (null = self)

Do NOT change existing localScale (keep 0.1, 0.1, 0.1).
Do NOT rename GameObject. Do NOT change GUID of prefab.

Save. List components on LaitueObj + PlantPlantingPunch field values. STOP.
```

---

## Checklist auteur (après Bezy)

- [ ] `PlantPlantingPunch` présent sur `LaitueObj`
- [ ] Pose une graine in-game → scale 0 → ×3 → repos (~2s + ~2.5s)
- [ ] Reload save → pas de punch sur plantes déjà posées
- [ ] Retune sliders Inspector si trop long / trop gros (`peakScaleMultiplier` 1–5)

---

## Valeurs de tuning (réf.)

| Champ | Défaut | Slider |
|-------|--------|--------|
| `peakScaleMultiplier` | 3 | 1–5 |
| `zoomInDurationSeconds` | 2 | 0.5–5 |
| `zoomOutDurationSeconds` | 2.5 | 0.5–5 |
