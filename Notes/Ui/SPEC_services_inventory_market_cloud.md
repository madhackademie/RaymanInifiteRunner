# SPEC — Services Inventory/Market Cloud (runtime agnostique UI)

Objectif: définir les règles techniques obligatoires pour l'inventaire et le market, quel que soit le choix d'affichage (panel UI global ou scène dédiée).

---

## 1) Règles non négociables

1. **UI = vue uniquement**
   - Les scripts UI ne font pas d'appels réseau directs.
   - La UI passe toujours par des services applicatifs.

2. **Services clairs et séparés**
   - `IInventoryService`: cache local + sync cloud inventaire joueur.
   - `IMarketService`: listing, buy, sell, cancel, refresh.

3. **Serveur autoritaire**
   - Toute action sensible (achat, vente, consommation, validation d'item reçu) est validée côté serveur.
   - Le client n'est jamais la source de vérité finale.

4. **Async + états UI explicites**
   - Les écrans exposent un état lisible: `Loading`, `Ready`, `Error`, `Stale`, `Syncing`.
   - Pas de blocage de frame pendant les requêtes.

5. **Résilience réseau**
   - Cache local obligatoire.
   - Retry contrôlé + bouton refresh manuel.
   - Gestion des timeouts, erreurs transitoires et reprise après perte réseau.

---

## 2) Contrats recommandés (minimum)

### `IInventoryService`
- `GetSnapshotAsync()`
- `TryConsumeAsync(itemId, quantity)`
- `RequestSyncAsync()`
- `GetCachedSnapshot()`

Responsabilités:
- lire/écrire le cache local,
- orchestrer la sync cloud,
- exposer des résultats structurés (succès, refus métier, erreur réseau).

### `IMarketService`
- `GetListingsAsync(filter, page)`
- `BuyAsync(listingId, quantity)`
- `SellAsync(itemId, quantity, price)`
- `CancelSellAsync(orderId)`
- `RefreshAsync()`

Responsabilités:
- centraliser les appels cloud pour le market global,
- gérer pagination/filtres,
- renvoyer des erreurs métier explicites (prix changé, listing expiré, stock insuffisant).

---

## 3) Pipeline serveur (CloudScript/PHP/MySQL)

- Le client envoie une intention (buy/sell/consume/claim).
- CloudScript/API serveur valide:
  - identité joueur,
  - droits/règles métier,
  - cohérence inventaire/market,
  - anti-triche de base (quantités, doublons, race conditions).
- Le serveur écrit en base (PHP/MySQL) et renvoie un résultat autoritaire.
- Le client met à jour son cache et la UI selon la réponse.

---

## 4) Décision UI (rappel)

- **Inventaire joueur fréquent**: panel UI global recommandé.
- **Market global interconnecté**: scène/flow dédié recommandé.

Important: ce document reste valide dans les deux cas. La couche service ne dépend pas du conteneur UI.

---

## 5) Checklist d'implémentation

- [ ] **Découplage gameplay -> inventaire (prérequis cloud)** : les scripts gameplay ne doivent plus dépendre directement de `PlayerInventory.Instance`; passer par `IInventoryService` (ou un provider équivalent).
- [ ] **Timing imposé** : ce découplage doit être fait **avant** de brancher la sync cloud en production, ou **au plus tard** dans la première étape de l'implémentation cloud (pas après).
- [ ] Créer interfaces `IInventoryService` et `IMarketService`.
- [ ] Interdire les appels réseau directs depuis les scripts UI.
- [ ] Normaliser les états UI (`Loading/Ready/Error/Stale/Syncing`).
- [ ] Ajouter cache local + stratégie retry.
- [ ] Ajouter refresh manuel côté UI.
- [ ] Faire valider buy/sell/consume/claim côté serveur autoritaire.
- [ ] Journaliser les erreurs réseau/métier (logs exploitables debug).

