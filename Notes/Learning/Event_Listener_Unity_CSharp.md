# Event / Listener en Unity et C#

## 1) Idee simple

- Un **event** est un signal: "quelque chose vient de se passer".
- Un **listener** est un morceau de code qui "ecoute" ce signal et reagit.

Exemple mental:
- La plante dit: "Je suis mature et on m'a cliquee."
- L'inventaire ecoute, essaie d'ajouter l'item, puis renvoie succes ou echec.

---

## 2) Pourquoi c'est utile

- Evite de faire des scripts trop colles entre eux.
- Rend le code plus modulaire (chaque script a un role clair).
- Facilite les tests et l'evolution (UI, audio, effets, quetes peuvent ecouter le meme evenement).

---

## 3) Deux approches en Unity

### A. UnityEvent (Inspector friendly)

- Simple pour brancher des callbacks via l'Inspector.
- Pratique pour UI et prototypes rapides.

```csharp
using UnityEngine;
using UnityEngine.Events;

public class Crop : MonoBehaviour
{
    public UnityEvent onMature;

    public void BecomeMature()
    {
        onMature?.Invoke();
    }
}
```

### B. event Action (C# pur, plus robuste en code)

- Mieux pour architecture de gameplay et communication entre systems.
- Plus clair pour controler l'abonnement/desabonnement.

```csharp
using System;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public event Action<Crop> OnHarvestRequested;
    [SerializeField] private bool isMature;

    private void OnMouseDown()
    {
        if (!isMature) return;
        OnHarvestRequested?.Invoke(this);
    }
}
```

---

## 4) Exemple adapte a ton cas (recolte + inventaire)

### Etape 1 - L'objet recoltable emet une demande

```csharp
public event Action<Crop, ItemId, int> OnHarvestRequested;
```

Quand on clique et que la plante est mature:
- On emet `OnHarvestRequested(this, itemId, amount)`.

### Etape 2 - L'inventaire tente l'ajout

Expose une methode unique:

```csharp
InventoryAddResult TryAdd(ItemId itemId, int amount);
```

Regles recommandees:
1. Chercher une pile existante compatible et remplir.
2. Si reste > 0, chercher slot vide et creer nouvelle pile.
3. Si pas assez de place: retour `InventoryFull`.

### Etape 3 - Decision de recolte

- Si `TryAdd` reussit: l'objet change d'etat (`Harvested` ou `Seed` selon design).
- Si echec (`InventoryFull`): popup UI "Inventaire plein" et l'objet reste `Mature`.

---

## 5) Pattern d'abonnement propre

Toujours s'abonner en `OnEnable` et se desabonner en `OnDisable` pour eviter les appels fantomes.

```csharp
private void OnEnable()
{
    crop.OnHarvestRequested += HandleHarvestRequest;
}

private void OnDisable()
{
    crop.OnHarvestRequested -= HandleHarvestRequest;
}
```

---

## 6) Erreurs frequentes

- Oublier le desabonnement (`-=`) -> callbacks multiples ou erreurs null.
- Mettre toute la logique dans un seul script monolithique.
- Confondre "event declenche" et "etat valide" (toujours verifier `isMature`).
- Ne pas definir de resultat clair d'inventaire (succes / plein / partiel).

---

## 7) Mini plan d'implementation (safe)

1. Ajouter des etats de plante: `Seed`, `Growing`, `Mature`.
2. Activer le clic seulement en `Mature`.
3. Creer `Inventory.TryAdd(...)` avec un enum resultat.
4. Brancher popup UI si resultat `InventoryFull`.
5. Tester 3 cas:
   - pile existante,
   - nouveau slot,
   - inventaire plein.

---

## 8) Vocabulaire utile

- **Publisher**: script qui emet l'evenement.
- **Subscriber / Listener**: script qui ecoute l'evenement.
- **Callback / Handler**: methode appelee quand l'evenement arrive.
- **Decouplage**: reduction des dependances directes entre scripts.

