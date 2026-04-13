# PlayerInventory — accès par singleton et ordre d’`Awake`

## Référence dans l’Inspector

Le champ `playerInventory` n’est plus un `[SerializeField]` : le **drag & drop dans l’Inspector n’est plus nécessaire**.

La référence est obtenue via **`PlayerInventory.Instance`** dans `Awake` (côté consommateur, ex. `BiofiltreManager`). Cela fonctionne dans **toutes les scènes** tant que le **GameManager** (ou l’objet qui porte `PlayerInventory`) est **chargé avant** le `BiofiltreManager`.

## Ordre d’exécution à surveiller

Unity **ne garantit pas** l’ordre d’`Awake` entre **GameObjects différents** par défaut.

En particulier : **`PlayerInventory.Awake`** doit s’exécuter **avant** **`BiofiltreManager.Awake`** pour que `Instance` soit initialisé quand le biofiltre y accède.

Si un **warning** ou un comportement incorrect apparaît en jeu (référence nulle, singleton non prêt) :

1. Ouvrir **Edit → Project Settings → Script Execution Order**
2. Donner une priorité **plus basse** (plus tôt) à **`PlayerInventory`** qu’à **`BiofiltreManager`** (les scripts avec un nombre **négatif** s’exécutent avant ceux à `0` par défaut).

Ainsi l’ordre d’initialisation reste prévisible sans dépendre de l’ordre des objets dans la hiérarchie.
