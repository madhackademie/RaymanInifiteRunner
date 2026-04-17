# INDEX — Documentation Markdown du projet (V2)

Ce fichier centralise tous les `.md` du repo pour retrouver rapidement l'information.

Légende :
- **Statut** : `Actif` (utile maintenant), `A trier` (vrac / brouillon), `Vide` (placeholder), `Reference` (doc externe/package).
- **Priorité** : `P1` (reprise session), `P2` (exécution courante), `P3` (contexte/lecture ponctuelle).

## Racine du projet

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `ASSISTANT_CONTEXT.md` | Actif | P1 | Snapshot de contexte projet (état technique, décisions actées, priorités, références clés). |
| `PROJECT_LOG.md` | Actif | P1 | Journal chronologique des sessions (ce qui a été fait, décisions, problèmes, prochaines actions). |
| `WORKFLOW_PROTOCOL.md` | Actif | P1 | Protocole d'ouverture/fermeture de session avec le flux de travail IA + mise à jour du journal. |
| `GIT_HELPER.md` | Actif | P2 | Aide-mémoire Git 80/20 pour démarrer/finir une session, commit, pull/push et bonnes pratiques. |

## Notes globales

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/Todo_project.md` | Actif | P1 | Hub principal des TODOs (prototype, GDD, workflow, art, polish, backlog nettoyage). |

## Notes Farm

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/Farm/WORKFLOW_ajouter_nouvelle_plante.md` | Actif | P1 | Workflow complet pour ajouter une plante (SO, prefab, UI, tests), incluant la config plantes à fruits. |
| `Notes/Farm/TODO_plantation_pipeline.md` | Actif | P1 | Pipeline recommandé de la plantation (grille, UI, preview, placement) avec état d'avancement prototype. |
| `Notes/Farm/GUIDE_footprint_GetOccupiedCells.md` | Actif | P2 | Guide pratique du footprint multi-cases (`GetOccupiedCells`) avec exemples d'usage placement/occupation. |
| `Notes/Farm/SPEC_plant_footprint_prompt.md` | Actif | P3 | Spécification + prompt type pour implémenter le placement footprint (modèle données, validation, service grille). |

## Notes UI

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/Ui/Journal_ui.md` | Actif | P2 | Note UI fusionnée (décisions, architecture stack, localisation, inbox, tâches, questions ouvertes). |
| `Notes/Ui/ARCHI_hud_ui_manager_additive.md` | Actif | P1 | Shell `NavigationHUD` + `UIManager`, état implémenté 2026-04-16, hub **`Carte`** §10. |
| `Notes/Ui/GUIDE_scenes_navigation_Unity_inventaire_market.md` | Actif | P2 | Options A–D navigation scènes / UI additive, sync-async, pièges `EventSystem`. |
| `Notes/Ui/LOADINGSCREEN_image_workflow.md` | Actif | P1 | Illustration + import sprite + intégration Canvas **`Bootstrap`** / fade `LoadingScreen`. |
| `Notes/Ui/Todo_ui.md` | Actif | P1 | Backlog UI : focus **LoadingScreen** (session auteur) ; **hub Carte** + retour croix depuis `FirstLvl` ; bootstrap/`UIManager` ; `LanguageManager` / TMP. |
| `Notes/Ui/Decision_ui.md` | Actif | P3 | Spéc UI proto -> polish (objectifs, architecture panneaux, animations, contrat d'orchestration). |
| `Notes/Ui/Spec_ui.md` | Vide | P3 | Fichier placeholder pour future spécification UI dédiée. |

## Notes GDD

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/GDD/SPEC_progression_xp_joueur_et_biofiltre.md` | Actif | P2 | Spéc de progression: XP joueur + maturité du biofiltre/système pour gate les cultures avancées. |
| `Notes/GDD/Inbox_gdd.md` | A trier | P3 | Brouillon d'idées GDD en vrac (états de plantes, inspirations casual farm, pistes à structurer). |

## Notes Learning

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/Learning/README_learning.md` | Actif | P3 | Index des fiches pédagogiques et mode d'utilisation pour monter en compétence sur le projet. |
| `Notes/Learning/Event_Listener_Unity_CSharp.md` | Actif | P3 | Explication des events/listeners Unity C# avec exemples concrets pour gameplay découplé. |
| `Notes/Learning/Plant_Architecture_Unity_SO_MB.md` | Actif | P3 | Proposition d'architecture plante réutilisable (ScriptableObject + MonoBehaviour + events). |

## Notes Bezi

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Notes/Bezi/README_bezi.md` | Reference | P3 | Référence d'usage Bezi (prompting, threads, tagging `@`, sécurité, lien avec workflow Cursor). |

## Packages

| Fichier | Statut | Priorité | Synthèse |
|---|---|---|---|
| `Packages/com.bezi.sidekick/README.md` | Reference | P3 | README du plugin Unity Bezi (connexion app, indexation projet, sécurité/data privacy). |

---

## Remarques

- `Notes/Ui/Inbox_ui.md` a été fusionné dans `Notes/Ui/Journal_ui.md`.
- Ligne directrice retenue : conserver des notes monothématiques, sauf les hubs (`PROJECT_LOG`, doc globale, GDD inbox).
