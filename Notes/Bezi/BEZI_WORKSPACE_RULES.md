# Bezi — Workspace Rules (brouillon à coller)

**Usage :** Bezi → Workspace Settings → **Your Rules** → copier le bloc ci-dessous.

**Complément team :** créer une Shared Page `@Notes/Bezi/BEZI_WORKSPACE_RULES` si besoin de rappeler les chemins repo.

**Mise à jour :** 2026-08-29 — aligné sur `.cursor/rules/bezy_*` et `Notes/Ui/CONVENTION_layers_unity.md`.

---

## Bloc à coller dans Bezi (Your Rules)

```
CRITICAL — Projet Unity 6000.3.10f1 (RaymanInfiniteRunner). URP. Boot Bootstrap → shell NavigationHUD + écrans via UIManager (ScreenId). Popups via PopupId + ScreenPopupHost uniquement.

CRITICAL — Ownership prefabs UI : Bezy crée et modifie les prefabs UI (.prefab) et le wiring Inspector. Cursor fournit scripts C#, specs et prompts phasés. Ne pas réécrire la logique métier en C# sauf demande explicite auteur.

CRITICAL — Layer UI : m_Layer 5 sur Canvas et TOUS les enfants UI. Water = index 4 — ne JAMAIS mettre l'UI sur l'index 4. Référence Notes/Ui/CONVENTION_layers_unity.md.

ALWAYS — Travail prefab UI lourd en 3 phases séparées, une phase par thread ou prompt : (1) hiérarchie GameObjects seule, (2) composants Image/Button/TMP/Layout seulement, (3) wiring SerializeField et événements seulement. Attendre validation auteur entre chaque phase.

ALWAYS — Fin de chaque phase : Save. Lister fichiers et GameObjects modifiés. STOP. Ne pas lancer Simulate, Play Mode, ni demander confirmation visuelle.

ALWAYS — Prompts : chemins exacts des fichiers cibles. Dire "do not rescan whole project". Réutiliser scripts existants sauf instruction contraire. Garder chaque prompt sous 3500 caractères.

ALWAYS — Avant wiring scène : la scène ou le prefab cible DOIT être ouvert dans l'Editor (workaround path — voir Notes/Bezi/README_bezi.md § scène ouverte).

ALWAYS — Navigation runtime : transitions via SceneNavigator.ShowScene. Pas de SceneManager.LoadScene direct depuis UI gameplay.

ALWAYS — Popups : PopupId + binding ScreenPopupBinding + ScreenPopupHost. Pas d'instanciation popup ad hoc dans les feature screens.

IMPORTANT — Scripts de référence (ne pas dupliquer la logique) : UIManager, SceneNavigator, ScreenPopupHost, InventoryScreenController, ShopItemPopupController.

IMPORTANT — Chemins prefabs UI courants : Assets/Prefabs/Ui/ — scènes : Assets/Scenes/Bootstrap.unity, NavigationHUD.unity, HomeScene.unity, FirstLvl.unity.

IMPORTANT — Specs et prompts détaillés : Notes/Ui/PROMPTS_Bezi_*.md et Notes/Ui/ARBRE_*.md dans le repo GitHub (brancher GitHub MCP si disponible).
```

---

## Règles complémentaires (Pages, pas Rules)

À mettre dans une **Page** Bezi et @mentionner depuis Rules si trop long :

| Sujet | Fichier repo |
|-------|----------------|
| Layers UI | `Notes/Ui/CONVENTION_layers_unity.md` |
| Popups génériques | `Notes/Ui/popup_generique.md` |
| Navigation scènes / UIManager | `Notes/Ui/SceneUiLoadManagement.md` |
| Workarounds Bezy (path prefab, scène ouverte) | `Notes/Bezi/README_bezi.md` |
| File polish Bezy | `Notes/Ui/TODO_Bezy_polish_semaine.md` |

---

## Option GitHub MCP (recommandé)

1. Workspace Settings → MCPs → installer **GitHub MCP** (ou demander à Bezi : « install the GitHub MCP »).
2. Token en lecture sur `madhackademie/RaymanInifiteRunner`.
3. Exemple de prompt : « Lis `Notes/Ui/PROMPTS_Bezi_*.md` sur main pour la tâche [BZ-XXX] et exécute Phase 1 uniquement. »

Bezi lit le repo ; l'auteur lance toujours l'exécution dans Bezi.
