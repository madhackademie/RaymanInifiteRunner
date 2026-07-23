# Prompts Bezy — HUD Points d'action

Spec : `Notes/GDD/SPEC_points_actions.md` (si présente)  
Script vue : `Assets/Scripts/UI/ActionPointsHudView.cs`  
**Layers UI :** voir `Notes/Ui/CONVENTION_layers_unity.md` — **`m_Layer: 5`** (UI), pas 4 (Water).  
**Ne pas rescanner tout le projet.**

---

## Snippet layer (à réutiliser dans les prompts Bezy)

```
Layer UI = m_Layer: 5 sur Canvas et tous les enfants (Water = index 4, UI = index 5).
Voir @Notes/Ui/CONVENTION_layers_unity.md
```

---

## Historique

- **Phase 1** : prefab + instance NavigationHUD — **OK** (Bezy)
- **Micro P2** : layer UI (`m_Layer: 5`) sur instance scène — **OK**
- **Phase 3** : wiring Inspector prefab — **OK** (Bezy, 2026-06-29)
- **Phase 3bis** : sync instance `NavigationHUD` (doublon override) — **OK** (Bezy, 2026-07-23)
- **Phase 4.1** : hiérarchie zones fatigue — **OK** (Bezy, 2026-06-29)
- **Phase 4.2** : couleurs milestones + overlay consommé — **OK** (Bezy, 2026-06-29)
- **Phase 4.3** : tooltip survol zones — **OK** (Bezy, 2026-06-29)
- **Phase 5.1** : clips + controller SpendPulse — **OK** (Bezy, 2026-07-23)
- **Phase 5.2** : wiring Animator sur prefab — **OK** (Bezy, 2026-07-23)
- **Playtest** : **[P0-AP-PLAY-001]** barre fatigue + tooltips + malus PA + pulse Spend

### Design cible (Cursor + Bezy)

- **3 bandes colorées toujours visibles** sous la barre (milestones 80 / 120 / 160 PA).
- **BarFill** = overlay sombre semi-transparent sur la portion **consommée** (script `ActionPointsHudView`).
- **Tooltip au survol** de chaque bande (`ActionPointFatigueZoneHover` + `ActionPointFatigueTooltipHost`).
- **Polish anim** : trigger Animator `Spend` → clip `SpendPulse` (punch scale `Row`) quand la conso PA augmente.

Scripts Cursor (ne pas modifier sauf wiring Inspector demandé) :
- `ActionPointsHudView.cs` — champ `animator` + trigger `Spend` déjà prêt
- `ActionPointFatigueTooltipHost.cs`
- `ActionPointFatigueZoneHover.cs`
- `ActionPointFatigueUiCopy.cs`

---

## Phase 4 — Barre fatigue 160 PA (milestones + tooltips)

> **Prérequis : Phase 3 OK.**

### Phase 4.1 — Hiérarchie — OK

Validé repo :
- `BarBackground/ZoneSegments` + `HorizontalLayoutGroup`
- `ZoneComfort`, `ZoneCaution`, `ZoneFatigue` (LayoutElement flex 2:1:1)
- Ordre `ProgressBar` : `BarBackground` puis `BarFill`

### Phase 4.2 — Couleurs milestones — OK

Validé repo :
- ZoneComfort vert ~#3CB85A, ZoneCaution jaune ~#E8C820, ZoneFatigue orange ~#E87818
- Raycast Target ON sur les 3 zones
- BarFill : noir alpha 0.42, Filled Horizontal, Raycast OFF
- Layer UI (`m_Layer: 5`) sur ZoneSegments + zones

### Phase 4.3 — Tooltip survol — OK

Validé repo :
- `FatigueTooltipPanel` inactif par défaut, fond ~#1A1A1A alpha 0.92
- `ActionPointFatigueTooltipHost` : `panelRoot`, `panelRect`, `titleLabel`, `bodyLabel` câblés
- `ActionPointFatigueZoneHover` sur les 3 zones : tier Comfort(0) / Caution(1) / Fatigue(2)
- `tooltipHost` → même host sur les 3 zones
- Mineur : `FatigueTooltipPanel` en Default (0) — passer en UI (`m_Layer: 5`) si micro-fix Bezy

### Phase 4.3 — Tooltip survol (archive prompt)

```
Ne pas rescanner tout le projet. Réutiliser scripts existants.

Fichier unique :
- @Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab

Budget 160 PA. Bandes proportionnelles 80:40:40 (50% / 25% / 25%).

Tâche UNIQUE — hiérarchie GameObjects :

Sous ProgressBar/BarBackground :
1. Enfant "ZoneSegments" (stretch full).
2. HorizontalLayoutGroup : Spacing 0, Child Force Expand W+H ON.
3. Trois enfants Image :
   - ZoneComfort (Layout Element flex width = 2)
   - ZoneCaution (flex = 1)
   - ZoneFatigue (flex = 1)

Ordre siblings ProgressBar (fond → avant) :
1. BarBackground (+ ZoneSegments)
2. BarFill (Image Type Filled, Horizontal, Origin Left)

BarFill = overlay consommé (le script pilote fillAmount + couleur sombre).

Interdits : pas de logique C# PA ; ne pas recréer le widget.

Confirmer hiérarchie. Attendre OK avant 4.2.
```

### Phase 4.2 — Couleurs milestones

```
Ne pas rescanner tout le projet.

Fichier : @Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
Prérequis : Phase 4.1 OK.

Tâche UNIQUE — couleurs lisibles (milestones visibles dès le départ) :

1. ZoneComfort : vert #3CB85A, alpha 1, Raycast Target ON
2. ZoneCaution : jaune #E8C820, alpha 1, Raycast Target ON
3. ZoneFatigue : orange #E87818, alpha 1, Raycast Target ON

4. BarBackground : cadre sombre existant (alpha ~0.55).

5. BarFill : noir alpha ~0.42, Filled Horizontal Origin Left, fillAmount 0 par défaut, Raycast Target OFF.

6. Layer UI (`m_Layer: 5`) partout. Apply All instance NavigationHUD.

Play Mode : 3 couleurs toujours visibles ; overlay sombre grandit à gauche quand PA consommés.

Confirmer capture. Attendre OK avant 4.3.
```

### Phase 4.3 — Tooltip survol

```
Ne pas rescanner tout le projet.

Fichiers :
- @Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
- Scripts (lecture) : ActionPointFatigueTooltipHost.cs, ActionPointFatigueZoneHover.cs

Prérequis : Phase 4.2 OK.

Tâche UNIQUE — panneau tooltip + wiring :

1. Sous ActionPointsHudWidget, créer FatigueTooltipPanel (inactif par défaut) :
   - Image fond #1A1A1A alpha 0.92, padding
   - TitleLabel (TMP, gras, petit)
   - BodyLabel (TMP, corps, wrap)

2. Sur FatigueTooltipPanel : ActionPointFatigueTooltipHost
   - panelRoot → racine panel
   - panelRect → RectTransform panel
   - titleLabel, bodyLabel → TMP

3. Sur chaque zone :
   - ActionPointFatigueZoneHover
   - tier = Comfort / Caution / Fatigue
   - tooltipHost → ActionPointFatigueTooltipHost du widget

4. Panel au-dessus de la barre (sibling order / sort order élevé).

Play Mode : survol ZoneComfort → "Zone confort… aucun malus" ; ZoneCaution → +25% ; ZoneFatigue → +50%.

Confirmer wiring sans None + capture survol.
```

---

## Phase 3 — Wiring Inspector prefab — OK

Validé repo (`ActionPointsHudWidget.prefab`) :
- `ActionPointsHudView` : `pointsLabel`, `subtitleLabel`, `barFillImage`, `barBackgroundImage`, `fatigueIconImage`, `consumedOverlayColor`
- Tooltips / zones fatigue (Phase 4) aussi câblés sur le prefab

---

## Phase 3bis — Sync instance NavigationHUD — OK

Validé repo (Bezy 2026-07-23) :
- `m_AddedComponents: []` — override `ActionPointsHudView` scène **supprimé**
- Instance utilise le composant **prefab** uniquement (plus de doublon)
- Prefab : ordre SerializeField aligné + `fatigueIconImage` → `Row/Icon`
- Layers UI (`m_Layer: 5`) ajoutés sur panel tooltip / enfants manquants

### Prompt archive (Phase 3bis)

```
Ne pas rescanner tout le projet. Réutiliser scripts existants. Pas de nouveaux sprites.

Fichiers :
- Prefab : @Assets/Prefabs/Ui/ActionPoints/ActionPointsHudWidget.prefab
- Script : @Assets/Scripts/UI/ActionPointsHudView.cs
- Scène : @Assets/Scenes/NavigationHUD.unity (instance ActionPointsHudWidget sous HUDRoot)

Constat : le PREFAB a déjà ActionPointsHudView câblé (pointsLabel, subtitleLabel, barFillImage, barBackgroundImage, fatigueIconImage → Row/Icon, consumedOverlayColor).
L’INSTANCE scène a un ActionPointsHudView AJOUTÉ en override, incomplet (pas fatigueIconImage) → doublon possible.

Tâche UNIQUE — nettoyer + synchroniser l’instance :

1. Sur l’instance ActionPointsHudWidget dans NavigationHUD :
   - Supprimer le composant ActionPointsHudView ajouté en override (Added Component), s’il existe en double.
   - Garder UNIQUEMENT le ActionPointsHudView venant du prefab (un seul composant).

2. Vérifier Inspector (instance, sans None) :
   - pointsLabel → PointsLabel (TMP)
   - subtitleLabel → SubtitleLabel (TMP)
   - barFillImage → ProgressBar/BarFill
   - barBackgroundImage → ProgressBar/BarBackground
   - fatigueIconImage → Row/Icon (Image)
   - consumedOverlayColor ≈ noir alpha 0.42

3. Si un champ est None : le recâbler. Ne pas recréer la hiérarchie. Ne pas toucher ActionPointService.

4. Layer UI m_Layer: 5 partout sur le widget. Apply overrides utiles (layers) ; pas de second script HUD.

Play Mode :
- Un seul ActionPointsHudView sur le widget
- Affiche "0 / 160" (ou budget courant) + sous-titre travail
- Icône Row/Icon change de teinte selon zone fatigue
- Overlay BarFill suit la conso

Confirmer : 1 composant, 0 None, capture Inspector + Play Mode.
```

---

## Phase 3 — Wiring Inspector (archive prompt)

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

2. Vérifier layer UI (`m_Layer: 5`) sur TOUS les GameObjects du prefab — pas Default (0), pas Water (4).

3. Sur l'instance dans NavigationHUD : appliquer les overrides si le prefab a été modifié (Apply All si besoin).

Contraintes :
- Ne PAS modifier ActionPointService.cs, BiofiltreManager.cs
- Ne PAS implémenter la logique de débit PA dans ce script
- Ne PAS recréer la hiérarchie

Play Mode attendu :
- 160 / 160 au démarrage
- Sous-titre ≈ 16 h de travail
- Barre fill verte à 100 %
- Après plantation : 159 / 160

Confirmer Inspector sans champs None + capture.
```

---

## Phase 5 — Polish anim HUD (SpendPulse)

> Pattern projet : `Assets/Animations/UI/` (comme ShopItemPopup / TalentTreeOverlay).  
> **Pas de nouveaux sprites.** Ne pas animer `BarFill.fillAmount` (piloté par le script).

### Phase 5.1 — Clips + Animator Controller — OK

Validé repo (Bezy 2026-07-23) :
- `ActionPointsHud_Idle.anim` — path `Row`, scale 1, 0.01 s, loop OFF
- `ActionPointsHud_SpendPulse.anim` — keys 1 → 1.07 → 0.97 → 1, 0.28 s, path `Row` only
- `ActionPointsHud.controller` — Trigger `Spend`, Idle default, Any→SpendPulse, SpendPulse→Idle (exit 1)

### Phase 5.2 — Wiring Animator sur prefab — OK

Validé repo (Bezy 2026-07-23) :
- Animator sur racine `ActionPointsHudWidget` → `ActionPointsHud.controller`
- Update Mode Normal (0), Culling Always Animate (0)
- `ActionPointsHudView.animator` câblé ; labels / barre / icône inchangés
- Instance `NavigationHUD` : pas de composant ajouté (hérite du prefab)

### Phase 5.2 — archive prompt

```
(voir historique session — livré OK)
```

---

## Anti-patterns

- Pas de débit PA dans les scripts UI
- Pas de duplicate reset journalier
- Pas de recréation du prefab from scratch
- Pas d’animation de `fillAmount` / couleurs barre (conflit script)
