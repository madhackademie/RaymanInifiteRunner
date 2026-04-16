# TODO UI — backlog

**Navigation scènes / Inventaire / Market (HUD global, Additive, sync-async)** : `GUIDE_scenes_navigation_Unity_inventaire_market.md`.

**Prochain chantier prioritaire — HUD / UI Manager additive** : `ARCHI_hud_ui_manager_additive.md`.

## HUD / UI Manager additive

### Décision de travail recommandée
- `NavigationHUD.unity` devient la scène shell UI globale.
- Créer un `UIManager` dans `Assets/Scripts/Systems`.
- Précharger `Inventaire` en additif, puis afficher/masquer son root avec `SetActive`.
- Laisser `NavigationHUD` comme vue HUD et déplacer l'orchestration de navigation dans le manager global.

### Prompt simple pour Bezi

```text
Créer une UI globale persistante partagée entre toutes les scènes du projet Unity.
Utiliser `NavigationHUD.unity` comme scène shell UI additive.
Créer un `UIManager` global qui précharge les scènes UI fréquentes en additif au démarrage, en particulier `Inventaire`, puis affiche/masque leurs roots avec `SetActive` pour rendre la navigation quasi instantanée.
Garder un seul `EventSystem`, laisser `NavigationHUD` comme vue HUD, et déplacer la logique de navigation globale dans le `UIManager`.
```

### Tâches
- [ ] Ajouter `NavigationHUD.unity` au build settings si cette scène devient une dépendance runtime.
- [ ] Définir le bootstrap de chargement initial : menu -> shell UI -> `FirstLvl`.
- [ ] Créer `UIManager` avec une API simple de type `ShowScreen`, `HideScreen`, `ShowGameplayHUD`.
- [ ] Débrancher la logique de chargement de scènes directement depuis `NavigationHUD`.
- [ ] Précharger `Inventaire` une seule fois et identifier son root UI principal.
- [ ] Remplacer l'ouverture/fermeture fréquente de l'inventaire par `SetActive(true/false)`.
- [ ] Garantir un unique `EventSystem` entre scènes additives.
- [ ] Vérifier que `InventoryUI.Bind(PlayerInventory.Instance)` fonctionne encore avec un écran préchargé puis masqué.
- [ ] Préparer le même pattern pour `Market` afin d'éviter un second refactor plus tard.
- [ ] Réutiliser le prompt Bezi ci-dessus pour lancer l'implémentation du shell UI global.

## LanguageManager / TextMeshPro (transféré depuis `Decision_ui.md`)

### Décisions à respecter (rappel)
- `country` ≠ `language` : pays pour market/pub ; langue choisie par le joueur (override).
- Au boot : `language = defaultLanguageFromCountry(country)` sauf si override joueur déjà sauvegardé.
- Changement d’option langue → `OnLanguageChanged` → mise à jour de tous les TMP localisés.

### Tâches
- [ ] Définir l’énum / identifiants `Language` et `Country` (ou strings normalisées) selon le besoin prototype.
- [ ] Implémenter `defaultLanguageFromCountry(country)` (table de mapping).
- [ ] Persister le choix joueur (`language` override) — au minimum PlayerPrefs en proto, save plus tard si besoin.
- [ ] Créer un `LanguageManager` (singleton ou service injecté) avec :
  - [ ] propriété `CurrentLanguage`
  - [ ] événement `OnLanguageChanged(Language)`
  - [ ] méthode `GetText(string key)` avec **fallback** (langue courante → EN → afficher la key brute si absent).
- [ ] Créer un composant `LocalizedTMPText` sur les `TextMeshProUGUI` :
  - [ ] champ `key` (string stable)
  - [ ] abonnement à `OnLanguageChanged` + refresh au `OnEnable`.
- [ ] Documenter la **convention de keys** (`BTN_PLAY`, `TITLE_FARM`, etc.) dans `Spec_ui.md` ou ici.
- [ ] **Prototype** : table `language → key → string` (en dur ou ScriptableObject minimal).
- [ ] **Final** : migrer vers ScriptableObject ou JSON (même schéma clé/valeur).
- [ ] **DoD** : changer la langue dans les options met à jour **tous** les textes TMP localisés **sans** recréer la scène.

### Hors scope immédiat (référence)
- Intégration détection pays “réelle” (SDK / store) — peut rester mockée jusqu’au besoin market.
