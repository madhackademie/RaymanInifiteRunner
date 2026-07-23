# Prompts Bezy — SaleChannel bandeau cooldown polish `[BZ-POLISH-004]`

**Statut :** Ph.1–2 Bezy + hook Cursor livrés (2026-07-23).  
**Prefab :** `Assets/Prefabs/Ui/SaleChannels/SaleChannelBandeauView.prefab`  
**GUID :** `dac6251613d7f9849a21f9c1598ff676` (inchangé)

---

## Livré Bezy

| Élément | OK |
|---------|----|
| `SaleChannelBandeau.controller` + FadeIn + Pulse | oui |
| CanvasGroup sur `CooldownOverlay` (alpha 0) | oui |
| Animator Unscaled + `IsOnCooldown` | oui |
| Locked : overlay alpha 0.7, LockIcon 56, BientotLabel 24 bold | oui |
| Layer UI 5 | oui |

## Fix Cursor (review)

- `SaleChannelBandeauView` : `SetBool(IsOnCooldown)` + alpha 0→fade ; hide après fin cooldown
- Clip pulse : path corrigé `CooldownOverlay/CooldownLabel` (était `CooldownLabel` → binding cassé)

## Playtest

1. Vendre voisinage → overlay cooldown **fade in** + label timer **pulse**
2. Bandoulière / Vélo : locked plus sombre + « Bientôt » lisible
3. Fin cooldown → overlay disparaît, bandeau recliquable
