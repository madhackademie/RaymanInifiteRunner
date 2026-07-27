# Affichage / feedback monnaie — question ouverte GDD

**Statut :** ouvert (2026-07-27) — lié à `[BZ-POLISH-015]` **PARK UX**.  
**Spec mère :** `Notes/GDD/SPEC_vente_production_boucle_jeu.md` §5.8.

## Contexte

Le punch wallet sur `CurrencyBalanceUI` (shop Header) et `WalletWidget` (inventaire) a été livré techniquement, mais le joueur **ne voit pas** ces widgets au moment où le solde change :

- inventaire ouvert ≠ moment de gain / dépense ;
- à l’achat, le solde Header n’est pas perçu (ou le popup se ferme juste après le débit).

**Déjà utile :** `SaleMoneyBurst` sur les bandeaux vente = feedback **local à l’action** de gain.

## Questions à trancher

1. Le solde doit-il être **toujours visible** (chip sur `NavigationHUD`) ?
2. Ou seulement **au moment de la transaction** (shop / vente) ?
3. Le wallet inventaire reste-t-il **consultation seule** (pas de feedback d’action) ?

## Suites possibles (après décision)

| Décision | Suite technique |
|----------|-----------------|
| HUD monnaie | Bezy chip `NavigationHUD` + réutiliser triggers `Gain`/`Spend` |
| Feedback local | toast/delta `-XX` à l’achat ; vente = garder `SaleMoneyBurst` |
| Solde shop lisible seulement | job Bezy lisibilité Header (pas punch) |

**Ne pas** relancer de polish Bezy wallet tant que ce doc / §5.8 n’est pas tranché.  
Assets/hooks `[BZ-POLISH-015]` conservés en attendant.
