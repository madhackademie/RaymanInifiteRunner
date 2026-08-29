# Prompt agent VM — HUD slots biofiltre (nuit)

**Date brief :** 2026-08-29  
**Auteur :** demande session (mockup `biofiltreInterface`)  
**Branche :** `feature/rework-biofiltre-grid` (revalider au bootstrap ; ne **pas** merger vers `main` tout seul).  
**Crédits Bezy :** reset en dur le **30** de chaque mois (prochain : **30 août**). Jobs Bezy lourds **après** le reset si le stock est vide.  
**Ne pas faire :** `[P0-FARM-IBC-GRID-001]` (scale sprite IBC sur la grille) — c’est la tâche auteur du lendemain, pas ce brief.

Ce fichier est le **prompt à coller** dans un agent VM / Cursor overnight. Lis-le en entier avant d’écrire du code.

---

## Mission (une phrase)

Le **système d’étoiles existe déjà** (`UiStarSlot` + `UiStarRow`). Il ne faut **pas** le recréer. Il faut le **même modèle** pour **deux autres rangées** : slots **primaires** (N cadenas) et slots **secondaires** (N cadenas), plus un **HUD world** unique réutilisé sur **tous** les biofiltres, avec **repositionnement par instance** (systèmes non carrés, tailles différentes).

**Prefabs + intégration art = Bezy uniquement**, via le skill `/prefab-ui-3phases`. L’agent VM = **C# + copie art Dump→Sprites + docs**. **Aucun** `.prefab` / `.unity` YAML.

---

## 0) Lecture obligatoire (ordre)

1. `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` — skill prod (ne pas skip Bezy).
2. `Notes/Ui/PROMPTS_Bezi_ui_star_slot.md` — **modèle à cloner** (atome + rangée N).
3. `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md` — prompts Bezy **déjà rédigés** (ajuster seulement si tu renommes une API).
4. `Assets/Scripts/UI/Stars/UiStarSlotView.cs` + `UiStarRowView.cs`
5. `Notes/GDD/SPEC_biofiltre_slots_shields.md` — 3 primaires, 5 secondaires ; états cadenas / vide / équipé.
6. `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` — étoiles biofiltre = **même grammaire** que la vente, instance par cuve.
7. Mockup visuel : `Assets/Art/Mocup/biofiltreInterface_1.png` (le `.afpub` n’est pas lisible). `_2` / `_3` sont vides — ignorer.
8. Art Dump (source, **ne pas** câbler dans un prefab) :
   - `Assets/Art/Assets Store Dump/Ui/slotBiofiltrePrimaire.png` (Multiple : `_0` cadre vide, `_1` icône serre, `_2` cadenas)
   - `Assets/Art/Assets Store Dump/Ui/slotBiofiltreSecondaire.png` (`_0` vide, `_1` anti-slug, `_2` cadenas)
9. `.cursor/rules/bezi_prefab_ownership.mdc` + `.cursor/rules/art_asset_dump.mdc`
10. `Assets/Scripts/Farm/BiofiltreManager.cs` + `GridManager.cs`

**Interdit de relire / relancer :** chantier bed skin (`BiofiltreBedSkin`, `PROMPTS_Bezi_biofiltre_bed.md`) — **annulé** 2026-08-29. Ne pas recréer ces fichiers.

---

## 1) Ce qui existe déjà (ne pas dupliquer)

| Pièce | Chemin | Rôle |
|-------|--------|------|
| Atome ★ | `Assets/Prefabs/Ui/Common/UiStarSlot.prefab` | Slot + Fill, `UiStarSlotView.SetFilled(bool)` |
| Rangée ★ | `Assets/Prefabs/Ui/Common/UiStarRow.prefab` | **5** nested slots, `SetVisibleSlotCount` + `SetFilledCount` |
| Capacité ★ | `UiStarRowView.PrestigeStarCapacity = 5` | GDD ★1–5 biofiltre **et** vente |

Le HUD biofiltre **neste** `UiStarRow` tel quel (comme les bandeaux vente). **Ne pas unpack.** **Ne pas** forker `UiStarSlot` / `UiStarRow`.

Mockup : 1ère ★ remplie, 4 vides → `SetFilledCount(1)` + `SetVisibleSlotCount(5)` au preview prefab / binder (pas de logique prestige dans la vue).

---

## 2) Livrables agent VM (toi)

### 2.1 Promo art (autorisation auteur = ce brief)

Copier les **2 PNG + leur `.meta`** Dump → :

`Assets/Art/Sprites/UI/Biofiltre/slotBiofiltrePrimaire.png`  
`Assets/Art/Sprites/UI/Biofiltre/slotBiofiltreSecondaire.png`

Contraintes Dump :

- **Nouveau `guid:`** dans chaque `.meta` copié (sinon collision). Garder `spriteMode: 2` et les `spriteSheet.sprites` (slices `_0` `_1` `_2`).
- **Ne jamais** référencer `Assets/Art/Assets Store Dump/` depuis un script ou un prompt Bezy Phase 2.
- Créer le dossier `Assets/Art/Sprites/UI/Biofiltre/` s’il n’existe pas.
- **Ne pas** promouvoir `Cuve_IBC_3quart_carre_parfait.png` ni aucun art IBC Dump (autre chantier, OK auteur requis à part).

Mapping visuel (V0) :

| Sprite | Usage |
|--------|--------|
| `…Primaire_0` | cadre Slot primaire (vide) |
| `…Primaire_1` | Fill / icône équipée (serre — placeholder GDD `slot.primary.serre`) |
| `…Primaire_2` | overlay Lock primaire |
| `…Secondaire_0` | cadre Slot secondaire |
| `…Secondaire_1` | Fill (anti-slug placeholder) |
| `…Secondaire_2` | overlay Lock secondaire |

Les cadres Dump primaire sont **carrés** ; l’icône serre est **hex**. V0 = cadre + fill + lock (comme ★). Ne pas inventer un 4ᵉ sprite.

### 2.2 Scripts C# (vues sans métier)

Dossier : `Assets/Scripts/UI/BiofiltreHud/`  
Une classe par fichier, `[SerializeField]` private, pas de `FindObjectOfType` dans `Update`.

**`BiofiltreSlotVisualState`** (enum) : `Locked`, `Empty`, `Equipped`.

**`UiBiofiltreSlotView`** (calque `UiStarSlotView`) :

- Refs : `slotImage`, `fillImage`, `lockImage`
- `SetState(BiofiltreSlotVisualState)`
- `SetEquippedSprite(Sprite)` optionnel (Fill)
- Défaut prefab : **Locked** (`lock` on, `fill` off)
- Pas de prestige, pas de save, pas de clignotement consommable (GDD §5 = plus tard)

**`UiBiofiltreSlotRowView`** (calque `UiStarRowView`) :

- `slots[]` d’atomes nested
- `SetVisibleSlotCount(int)` — cache les extras
- `SetAllLocked()`
- `SetSlotState(int index, BiofiltreSlotVisualState)`
- Constantes : `PrimaryCapacity = 3`, `SecondaryCapacity = 5` (GDD). **N** = visible count, pas une 2ᵉ courbe ★.

Deux prefabs (Bezy) partagent **le même** pair de scripts : primaire plus grand (`72×80`), secondaire `48×48`.

**`GridManager`** : ajouter un getter AABB monde, sans casser l’API existante. Origin = coin **haut-gauche** cellule (0,0) ; Y décroît vers le bas.

```csharp
// Intention : Rect Unity (x,y = coin bas-gauche monde)
// width = Columns * CellSizeWorld.x
// height = Rows * CellSizeWorld.y
public Rect GetWorldRect();
```

**`BiofiltreHudView`** (vue HUD, pas de métier) :

- `UiStarRowView starRow`
- `UiBiofiltreSlotRowView primaryRow`
- `UiBiofiltreSlotRowView secondaryRow`
- Preview Inspector : ★ filled 1 / visible 5 ; primary+secondary **tous Locked** + visible = capacité GDD

**`BiofiltreHudBinder`** (sur chaque instance `Biofiltre` / à côté de `BiofiltreManager`) :

- `[SerializeField] BiofiltreHudView hudPrefab` — si null : `Debug.LogWarning` **fail closed**, pas d’instantiate magique
- Instantiate sous le transform du biofiltre (runtime)
- Canvas **World Space**, plan XY 2D
- Positionner les 3 widgets depuis `GetWorldRect()` + **offsets normalisés 0–1** + `Vector2` extra monde **par instance**
- Recaler si `Columns`/`Rows`/cell size changent (IBC vs futur bac bois / octogone : **pas** les mêmes offsets)

Ancres mockup (IBC FirstLvl — valeurs de **départ**, overridables) :

| Widget | Ancre normalisée dans l’AABB (0,0 bas-gauche → 1,1 haut-droite) |
|--------|------------------------------------------------------------------|
| Primary row | ~`(0.08, 0.92)` haut-gauche |
| Star row | ~`(0.92, 0.92)` haut-droite, pivot droite |
| Secondary row | ~`(0.18, 0.22)` bas / cuve |

**Pourquoi recaler :** le HUD est **le même prefab** pour **tous** les biofiltres. Les meshes / grilles **ne sont pas carrées** et **n’ont pas la même taille**. Interdit de hardcoder des positions FirstLvl uniquement. Chaque `BiofiltreHudBinder` porte ses offsets.

Ne **pas** éditer `Biofiltre.prefab` YAML. Le champ `hudPrefab` restera vide jusqu’à assignation Inspector (auteur / Bezy hors skill Ui si besoin). Documenter ce lien manuel.

Ne **pas** mixer `transform` et physique. `Time.deltaTime` inutile ici.

### 2.3 Hors scope (ne pas coder)

- Prestige ★3/★5, kill des étoiles, éclats, save `unlockedPrimarySlotIds`
- Équipement shields, clignotement slot vide, popups (`PopupId` / `ScreenPopupHost`)
- Bed skin, `GenerateGrid` double Start (régression connue `main`)
- Fond noir laitue, pose tactile, vente ★ playtest
- Prefabs UI, scènes, menus Editor « build prefab »

### 2.4 Docs (fin de passe)

- Cocher / préciser dans `Notes/Todo_project.md` les IDs ci-dessous
- Entrée courte `PROJECT_LOG.md`
- Si tu changes un nom de champ : aligner `Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`

**Pas de `git commit` / `push`** (règle auteur).

---

## 3) Livrables Bezy (pas toi) — skill

Chemin prod : `Notes/Bezi/WORKFLOW_skill_prefab_ui.md`

L’auteur (ou une session Unity) lance **un** appel par phase :

```
/prefab-ui-3phases
Task ID: [BZ-FARM-BIOHUD-PRIM-001]
Prefab: Assets/Prefabs/Ui/Common/UiBiofiltrePrimarySlot.prefab
Phase: 1
```

Puis `@Notes/Ui/PROMPTS_Bezi_biofiltre_hud_slots.md`

**Ordre jobs :** PRIM slot → PRIM row → SEC slot → SEC row → HOST HUD.  
**Jamais** Ph.1+2+3 fusionnés. **Jamais** Simulate / Play Mode dans un prompt Bezy.  
Fin de phase : `Save. List what changed. STOP.`  
Layer UI = **`m_Layer: 5`** (Water = 4).  
**Never unpack** `UiStarRow` / `UiStarSlot` / les nouveaux slots nested.

IDs :

| ID | Prefabs |
|----|---------|
| `[BZ-FARM-BIOHUD-PRIM-001]` | `UiBiofiltrePrimarySlot` puis `UiBiofiltrePrimarySlotRow` (3 nested) |
| `[BZ-FARM-BIOHUD-SEC-001]` | `UiBiofiltreSecondarySlot` puis `UiBiofiltreSecondarySlotRow` (5 nested) |
| `[BZ-FARM-BIOHUD-HOST-001]` | `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab` (Canvas world + nested ★ + 2 rows) |

Cursor IDs : `[P0-FARM-BIOHUD-001]` scripts + promo art + binder.

---

## 4) Critères de succès agent VM

- [ ] Art promu sous `Sprites/UI/Biofiltre/` (nouveaux GUID, slices conservées)
- [ ] Scripts compilables, API miroir étoiles, zéro métier prestige
- [ ] `GetWorldRect()` + binder avec offsets **par instance**
- [ ] Warning si `hudPrefab` null
- [ ] Aucun `.prefab` / `.unity` modifié
- [ ] Prompts Bezy toujours < 3500 car. par phase (recompter si tu édites)
- [ ] `PROJECT_LOG.md` + `Notes/Todo_project.md` à jour

Playtest visuel = **auteur**, après Bezy, pas l’agent VM.

---

## 5) Références mockup (layout)

`Assets/Art/Mocup/biofiltreInterface_1.png` :

- Haut-droite : rangée ★ (réutiliser `UiStarRow`)
- Haut-gauche : slots primaires (cadenas / vide / équipé)
- Bas sur la cuve : slots secondaires (vide / équipé / cadenas)

HUD **world** collé à la cuve, pas un panneau `NavigationHUD`.
