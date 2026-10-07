# Chrome popup farm — info (bois) vs actions (boutons)

**Création :** 2026-10-07  
**Contexte :** `FarmHarvestPanel_Build` · rejet barre batch H-farm-1 « cadre bois dans cadre bois ».

---

## Règle auteur (validée)

| Rôle | Chrome visuel | Exemples popup récolte |
|------|----------------|-------------------------|
| **Conteneur / info** | **Cadre bois** (panneau 9-s, cadre icône, piste progression, bandeau timer léger parchemin) | `FarmHarvestPanel` fond · `IconFrame` · `ProgressTrack` · labels TMP |
| **Action cliquable** | **Autre langage** — pas le même panneau bois lourd (rivets + planches) | Récolter (teal) · Arracher (danger) · Annuler (oval parchemin) · Batch (bandeau **léger** ou pill, pas H-farm-1 full wood) · Close (disque rouge) |

**Intent :** le joueur lit le **bois = carte / état** ; les **boutons = affordance** (couleur + forme), sans doubler le cadre du popup.

---

## Conséquences art

- **Garder** `UiKit_Panel_9s`, `UiKit_Frame_icon`, H-farm-4 piste, timer in-track (TMP).
- **Ne pas** réutiliser le chrome bois « panneau complet » sur chaque bouton.
- **Batch :** MVP `UiKit_Row_list` + icône enfant **ou** regen **H-farm-7** (parchemin fin, **sans** planches/rivets) — voir `Notes/Art/PROMPT_generation_icones.md` H-farm-2 / backlog H-farm-7.
- **Boutons primaire/secondaire :** kit existant (teal Sliced, oval, danger coral) — pattern Uproot, **pas** nested prefab étiré.

---

## Conséquences Unity

- **Info :** `Raycast Target` souvent **off** (labels, cadre icône, fill progression).
- **Actions :** `Raycast ON` sur l’Image du bouton ; taille + PPU mult tunés **par type** (batch mult ~3,5–12 selon sprite).
- Layout : placement **manuel** sous `Content` (VLG off) jusqu’à validation MVP.

---

## Références

- Mockup ref : `Assets/Art/Assets Store Dump/Ui/mockup_popup_recolte_roquette_20261005.jpg`
- Prefab build : `Assets/Prefabs/Ui/Farm/FarmHarvestPanel_Build.prefab`
- Charte : `Notes/Art/PROMPT_chatgpt_ui_kit_planche_bois.md` STYLE LOCK
