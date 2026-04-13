# TODO UI — backlog

**Navigation scènes / Inventaire / Market (HUD global, Additive, sync-async)** : `GUIDE_scenes_navigation_Unity_inventaire_market.md`.

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
