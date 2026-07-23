# Contrôle QA — Polish ShopItemPopup `[CT-SHOP-002]`

**Quand :** après Bezy **Phase 3** (transitions + wiring `animator` / `canvasGroup`) — ou dès que le lot Ph.1–3 est dans le projet.  
**Branche :** `feature/points-actions` (ou `main` après merge).  
**Statut batch :** cocher aussi dans `Notes/Todo_playtest.md` → Batch D.  
**Statut ID :** `Notes/Todo_project.md` `[CT-SHOP-002]`.

**Objectif :** valider que le polish (layers, lisibilité, anim) n’a rien cassé, et qu’**aucune UI shop/HUD ne fuit sur Bootstrap**.

---

## 0. Contrôle éditeur (avant Play)

- [ ] Prefab `Assets/Prefabs/Ui/ShopItemPopup.prefab`
  - [ ] Layer **UI (5)** partout (pas Default 0 / Water 4)
  - [ ] `ShopItemPopupView.animator` → Animator racine (**pas None**)
  - [ ] `ShopItemPopupView.canvasGroup` → CanvasGroup racine (**pas None**)
  - [ ] Autres refs (boutons, textes, ConfirmOverlay, wallet) **pas None**
  - [ ] `Root` (contenu) **inactif** par défaut sur le prefab *ou* cohérent avec le pipeline popup (pas de carte visible « en dur » en scène)
  - [ ] `ConfirmOverlay` **inactif** par défaut
  - [ ] `QuantityText` fontSize ≥ 22
- [ ] Clips `ShopItemPopup_Open` / `_Close` : path `Root/Card`, scale + slide, **pas** d’anim alpha CanvasGroup carte
- [ ] Scène `Bootstrap.unity` : uniquement LoadingScreen / GameBootstrap (pas d’instance `ShopItemPopup` posée en scène)

---

## 1. Bootstrap — pas de fuite UI (priorité auteur)

> Symptôme signalé : **UI visible / active pendant Bootstrap**. À confirmer après Phase 3.

**Procédure :**

1. [ ] Quitter Play Mode si actif ; éventuellement Domaine Reload / redémarrer Unity si DDOL suspect.
2. [ ] Ouvrir **uniquement** `Bootstrap` → Play.
3. [ ] Pendant le loading :
   - [ ] **Seul** le LoadingScreen (splash + barre) est visible
   - [ ] **Pas** de carte shop, ConfirmOverlay, NavigationHUD, wallet, ni popup flottante
4. [ ] Après chargement → shell / Home / FirstLvl :
   - [ ] Aucun `ShopItemPopup` resté ouvert
   - [ ] Hierarchy : chercher `ShopItemPopup` / `ConfirmOverlay` — si présents sous DDOL/`UIManager`, doivent être **inactifs** tant que le shop n’est pas ouvert

**Si KO :** noter Hierarchy (chemin complet) + capture. Cause probable : `Root`/`gameObject` resté actif après hide, ou instance template activée en scène / au boot. **Ne pas** masquer en cachant le LoadingScreen.

---

## 2. Shop — ouverture / fermeture (transitions)

1. [ ] HUD → onglet **Shop** (ou chemin habituel).
2. [ ] Ouvrir un item → popup :
   - [ ] Slide + léger scale sur `Card` (Open)
   - [ ] Backdrop assombri
   - [ ] Carte **opaque** (pas de fade global CanvasGroup qui blanchit la carte)
3. [ ] Fermer (Close / backdrop selon flux) :
   - [ ] Anim Close soft
   - [ ] Popup disparaît ; **plus rien** du popup à l’écran
4. [ ] Rouvrir un autre item → OK (pas de doublon, pas de carte coincée hors écran)

---

## 3. Shop — quantité + confirmation

1. [ ] `−` / `+` / `Max` / saisie quantité : valeurs cohérentes, boutons cliquables
2. [ ] CTA **Confirmer** → `ConfirmOverlay` visible (backdrop ~0.6, panel lisible)
3. [ ] **Annuler** overlay → retour carte, overlay off
4. [ ] **Confirmer achat** → flux nominal (achat OK ou feedback existant) ; overlay off après
5. [ ] Wallet Header : solde lisible / se met à jour si prévu

---

## 4. Régressions rapides (smoke)

- [ ] Fermer le shop screen → pas de popup orpheline
- [ ] Changer d’onglet HUD (Ferme / Inventaire / Vente) → pas de flash `ShopItemPopup`
- [ ] Relancer Play depuis Bootstrap → §1 toujours OK (pas de fuite)

---

## Résultat session

| Zone | OK / KO | Note |
|------|---------|------|
| Éditeur prefab / layers / wiring | | |
| Bootstrap sans UI parasite | | |
| Open / Close anim | | |
| Quantité + ConfirmOverlay | | |
| Smoke navigation | | |

**Décision :**  
- [ ] `[CT-SHOP-002]` validé → cocher `Todo_project` + Batch D  
- [ ] Bug Bootstrap / hide → ticket (ex. `Root` actif, wiring Phase 3, ou host popup) avant close

**Réfs code :**  
`ShopItemPopupView.Show/Hide`, `SetContentActive`, `ScreenPopupHost`, `UIManager` runtime popup bindings.
