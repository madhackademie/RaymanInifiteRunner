# Règles C# Bezy — RaymanInfiniteRunner (Unity 6)

À `@` dans tout thread Bezy qui **modifie du C#**. Ne pas rescanner tout le projet. Un sujet par thread.

**Workspace Bezy (hors repo)** : si une règle dit encore « Cursor fournit tout le scripts C# », la remplacer par `Notes/Bezi/WORKSPACE_RULES_paste_in_bezi.md` (C# **visuel** = Bezy ; métier = Cursor).

## Périmètre

**OK :** un ou deux scripts view/HUD déjà cités ; `Transform` / `RectTransform` ; sprite, couleur, TMP, `SetActive`, `CanvasGroup` ; `SerializeField` + cache ; layout ; `Animator.SetBool` / `SetTrigger` ; coller un pattern visuel déjà présent dans le même fichier.

**INTERDIT sans demande explicite de l’auteur :**
- `SceneNavigator`, `UIManager`, `ScreenPopupHost`, `PopupId`, bindings popup
- nouveaux services / interfaces / persistance
- inventer un système, un singleton, un `FindObjectOfType` en `Update`
- Shader / nouveaux fichiers hors liste du prompt
- Simulate, Play Mode, « confirm it looks good »

Si le prompt demande plus que ça : **STOP**, lister ce qui manque, ne pas élargir.

## Conventions (comme Cursor)

- Unity 6, `MonoBehaviour`, une classe = un fichier, nom = fichier
- `[SerializeField] private` — pas de champs `public` de données
- Pas de `Update()` inutile ; préférer événements / `OnEnable`
- Cache des refs dans `Awake` ou `OnEnable` (pas de `Find` en boucle)
- `PascalCase` types/méthodes, `camelCase` variables
- Constantes nommées — pas de magie (`1.2f` nu) sauf copie d’une valeur déjà dans le fichier
- Méthodes courtes ; commentaires seulement sur la logique non évidente
- UI : layer **5** (jamais 4 = Water) — `Notes/Ui/CONVENTION_layers_unity.md`
- Navigation scènes : ne pas appeler `SceneManager.LoadScene` ; ne pas ajouter de chemin popup parallèle

## Exécution

1. Lire **uniquement** les fichiers / GameObjects `@` ou listés dans le prompt.
2. Modifier exactement ce qui est demandé.
3. Réutiliser le code existant du fichier (même helpers, mêmes constantes).
4. Prefab/scène : n’éditer le YAML que si le prompt le dit ; sinon C# seulement.
5. Fin : **Save. Lister fichiers + méthodes touchées. STOP.**

Pas de merge « j’en profite pour refactor ». Pas de nouvelle architecture.
