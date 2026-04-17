# LoadingScreen — illustration et intégration

**Statut** : guide opérationnel · **Priorité** : P1 (prochaine session auteur, 2026-04-17)

## Contexte code

- **`Assets/Scripts/UI/LoadingScreen.cs`** : barre (`Image` fill via `anchorMax.x`), texte pourcentage (`TextMeshProUGUI`), fade via **`CanvasGroup`** sur le root piloté par `Hide()`.
- **`Assets/Scripts/Core/GameBootstrap.cs`** : référence **`[SerializeField] LoadingScreen loadingScreen`** ; séquence `NavigationHUD` (0–50 %) puis `FirstLvl` (50–100 %), durée mini, puis `loadingScreen.Hide()`.
- **Scène** : `Assets/Scenes/Bootstrap.unity` (entrée build index 0) — vérifier l’assignation **`GameBootstrap.loadingScreen`**.

Le script **ne référence pas encore** d’image décorative dédiée : l’illustration se branche **dans la hiérarchie** (recommandé pour le proto) ou via un futur `[SerializeField]` si tu veux piloter visibilité / sprite depuis le code.

## Création de l’asset (hors Unity)

- Cible visuelle convenue : **poisson + arbre** (voir `Notes/Ui/Todo_ui.md` — ton de travail itératif acceptable).
- Exporter en **PNG** avec **transparence** si fond non plein écran ; résolution raisonnable (ex. 1024–2048 px max côté) pour mobile.

## Import Unity

1. Placer le fichier sous un dossier stable, ex. **`Assets/Art/UI/Loading/`** (créer si besoin).
2. Sélectionner la texture → **Texture Type : Sprite (2D and UI)** si tu utilises un composant **`Image`** ; ajuster **Compression** / **Alpha** selon plateforme cible.
3. **Sprite Mode** : *Single* sauf atlas prévu plus tard.

## Intégration UI (sans toucher au C#)

1. Ouvrir **`Bootstrap.unity`**.
2. Sous le **Canvas** de l’écran de chargement (même parent que le composant `LoadingScreen`), ajouter un **`UI > Image`**.
3. Assigner le **Sprite** sur `Image.sprite` ; régler **Anchor** / **RectTransform** pour centrer ou remplir (éviter de masquer la barre : ordre des **siblings** : illustration **derrière** barre + texte, ou **devant** avec transparence partielle).
4. Vérifier que le **`CanvasGroup`** utilisé par `Hide()` couvre bien tout l’overlay de chargement (y compris la nouvelle image) pour que le fade sorte l’ensemble.

## Option code (plus tard)

- Ajouter un `[SerializeField] Image splashImage` (ou `RawImage`) sur `LoadingScreen` si tu dois swapper le sprite par thème ou désactiver l’image avant le fade.

## Contrôle qualité

- **Play Mode** : progression 0 → 100 %, pas de flash avant `Hide()`, un seul **`EventSystem`** après chargement du shell.
- **Build dev** : même flux (chargements plus lents → barre lisible).

## Liens

- `Notes/Ui/Todo_ui.md` — section *Bootstrap & LoadingScreen*
- `PROJECT_LOG.md` — entrée **2026-04-17**
