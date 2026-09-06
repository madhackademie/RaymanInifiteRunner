# HUD biofiltre — pas d’instantiate runtime

**Date :** 2026-09-06  
**Branche :** `feature/biofiltre-isometric`  
**Décision auteur :** l’instance HUD créée **au Play** ne permet pas de travailler.

---

## Ce qui ne marche pas

`BiofiltreHudBinder` fait `Instantiate(hudPrefab)` dans `Start()`. Conséquences :

- Les rows (primaire / étoiles / secondaire) **n’existent pas en Edit**. Impossible de les bouger, tourner, scaler comme un vrai objet de scène.
- En Play, Unity **interdit Save**. Les poignées Scene + cache « recopie à la sortie du Play » restent fragiles (facile de perdre le calage).
- Recalcul d’ancres normalisées (`GetWorldRect`) se bat avec un placement à l’œil sur la cuve iso.

**Constat playtest :** ce pipeline (instantiate + binder + gizmos Play) **ne marche pas** pour caler le HUD sur le biofiltre iso.

---

## Décision

**Préférer des prefabs / objets en dur** dans `Biofiltre.prefab` (enfants de la racine), plutôt qu’une copie runtime.

Cible :

```
Biofiltre
├── IbcSprite
├── Grid
├── Plants
└── BiofiltreHud          ← nested / enfant réel (pas Instantiate)
    ├── TopIsoLine
    │   ├── PrimaryRow
    │   └── StarRow
    └── SecondaryRow
```

L’auteur déplace les rows **en Prefab Mode ou dans la scène**, comme n’importe quel transform. Play = ce qui est déjà posé.

---

## Prochaine session (Cursor + Bezy)

ID suivi : **`[P0-FARM-BIOHUD-NEST-001]`**

1. **Bezy** : neste `Assets/Prefabs/Ui/Farm/BiofiltreHud.prefab` sous `Assets/Prefabs/World/Biofiltre.prefab` (ne pas unpack les rows). Wiring `BiofiltreHudView` inchangé.
2. **Cursor** : `BiofiltreHudBinder` **arrête d’instancier**. Il référence l’enfant déjà là (fail closed si manquant). Plus de recopie Play → Edit.
3. Pose à la main des 3 rows sur la cuve iso (bandes gauche / droite / bas-gauche).

Ne pas relancer un job « gizmos d’ancres en Play » : abandonné.

Réf. existante : `Notes/Farm/CABLAGE_biofiltre_ibc_grille_bezi.md` § HUD world.
