# Playtest — Points d'action V0

**Branche :** `feature/points-actions`  
**ID tâche parent :** `[P0-AP-PLAY-001]` — **batch** `Notes/Todo_playtest.md` (pas priorité session Bezy).  
**Statut global :** `Notes/Todo_project.md` + cocher le batch dans `Todo_playtest.md`.

**Prérequis**
- Play Mode depuis `Bootstrap` → ferme (`FirstLvlFarm`).
- HUD PA : Phase 3 + Phase 5 SpendPulse OK (2026-07-23).
- Save PA : `%USERPROFILE%\AppData\LocalLow\DefaultCompany\My project\action_points.json`.

**Forcer PA / inventaire à la mano** → cheatsheet complète dans `Notes/Todo_playtest.md` (Context Menu `PA Debug/*` sur `ActionPointService`, reset inventaire, cooldown vente).

**Références code**
- `Assets/Scripts/Systems/ActionPointService.cs`
- `Assets/Scripts/Farm/BiofiltreManager.cs` (planter)
- `Assets/Scripts/Farm/PlantHarvestInteractor.cs` (récolte + refund inventaire plein)
- `Assets/Scripts/UI/ActionPointsHudView.cs`

---

## 1. HUD et affichage

| # | Cas | Étapes | Résultat attendu | OK |
|---|-----|--------|------------------|-----|
| 1.1 | HUD visible | Entrer en ferme | Widget PA visible (NavigationHUD) | [ ] |
| 1.2 | Valeur initiale | Nouveau jour / save fraîche | **240 / 240** (ou budget Inspector) | [ ] |
| 1.3 | Refresh après action | Planter ou récolter | Compteur HUD baisse de **1** (ou log `[ActionPointService] -1 PA`) | [ ] |

---

## 2. Plantation (−1 PA)

| # | Cas | Étapes | Résultat attendu | OK |
|---|-----|--------|------------------|-----|
| 2.1 | Plantation nominale | Planter 1 graine | **−1 PA**, graine consommée, plante posée | [ ] |
| 2.2 | PA insuffisants | PA = 0, tenter planter | Plantation refusée, graine intacte | [ ] |

---

## 3. Récolte (−1 PA)

| # | Cas | Étapes | Résultat attendu | OK |
|---|-----|--------|------------------|-----|
| 3.1 | Récolte nominale | Plante mature → popup → Récolter | **−1 PA**, item inventaire, plante retirée, feedback récompense | [ ] |
| 3.2 | **Inventaire plein** | Remplir inventaire → récolter | Popup inventaire plein, **PA remboursés** (même valeur qu'avant), **plante intacte** | [ ] |
| 3.3 | **PA à 0** | Vider budget PA → tenter récolter | Log `Points d'action insuffisants`, **aucun item**, **plante intacte** | [ ] |
| 3.4 | Arrachage | Popup → Arracher | **0 PA** consommés, plante retirée | [ ] |

---

## 4. Persistance

| # | Cas | Étapes | Résultat attendu | OK |
|---|-----|--------|------------------|-----|
| 4.1 | Écriture save | Après 2–3 actions (plant/récolte) | `action_points.json` : `remainingPoints` cohérent | [ ] |
| 4.2 | Relance jeu | Quit Play Mode → relancer | Même `remainingPoints` qu'à la sortie | [ ] |

---

## 5. Vente (−1 PA) — après hook `[P0-AP-CODE-002]` vente

> Section à activer quand `SaleChannelService` consommera des PA.

| # | Cas | Étapes | Résultat attendu | OK |
|---|-----|--------|------------------|-----|
| 5.1 | Vente nominale | Récolter laitue → Vente → Voisinage → vendre | **−1 PA**, gold + inventaire OK | [ ] |
| 5.2 | PA insuffisants | PA = 0 → tenter vendre | Vente refusée, stock intact | [ ] |

---

## Notes session

_Date :_  
_Testeur :_  
_Blocages / bugs :_  

---

## Clôture

Quand **§1–4** sont OK (et **§5** si hook vente livré) :
1. Passer `[P0-AP-PLAY-001]` en `[x]` dans `Notes/Todo_project.md`.
2. Trace dans `PROJECT_LOG.md` si fin de session.
