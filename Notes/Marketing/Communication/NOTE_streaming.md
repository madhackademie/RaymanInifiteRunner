# NOTE — Streaming

**Date :** 2026-09-16  
**Hub :** `Notes/Marketing/README_marketing.md`  
**Lié :** `NOTE_devblog.md` · `NOTE_posts_share.md`

Steam diffuse une **vidéo** rattachée à un AppID. Ce n’est pas « le joueur a lancé le jeu Steam ».

## Plateformes actuelles

Facebook, Twitch, YouTube, Kick.

Multistreamer **sans foyer** = chat dilué, présence nulle part.

| Plateforme | Rôle retenu |
|---|---|
| **YouTube** | **Foyer live recommandé** (replay = devlog gratuit, Google, Shorts depuis le VOD). |
| **Twitch** | Restream ok ; foyer alternatif si le chat indie compte plus que le replay. |
| **Facebook** | Audience FR déjà là. Pas un moteur de découverte EN. |
| **Kick** | Restream auto seulement. Zéro effort extra. |
| **Steam Broadcast** | Dès fiche Coming Soon. Unique live qui pousse la wishlist. |
| **itch.io** | **Pas** une plateforme de live. Lien Twitch/YouTube + post Devlog « Live now ». |
| **X** | Mégaphone (J-1 / H-1 / go-live). Pas la maison. **Pas** X Live en principal. |
| **TikTok Live** | Test découverte plus tard, 1 créneau. Pas au setup initial. |

**Règle :** restream technique vers plusieurs sorties, **un seul chat lu** (celui du foyer).

Titres live : *« Biofiltre + grille iso — session N »*, pas *« je code »*.

## Steam — faut-il un build PC ?

**Non.** Pas de sortie PC publique, pas de build jouable en boutique, pour streamer *sur* Steam.

| Objectif | Build PC uploadé Steam ? |
|---|---|
| Live sur la fiche **Coming Soon** | **Non** |
| Wishlist / trailer / News | **Non** |
| Lancer le jeu *depuis* Steam (bêta privée, playtest partenaires) | Oui — depot **privé** suffisant |
| **Steam Next Fest** | Oui : **démo** jouable |
| Vente Steam (Windows / Deck) | Oui, **plus tard** |

Le flux peut être : éditeur Unity, APK via scrcpy, exe Windows **local**. OBS envoie du RTMP ; Valve tague l’**AppID**.

Pour *ce* projet (mobile / tactile), un live Steam = **Coming Soon + OBS**, pas un port PC.

## Steam — prérequis réels

1. Compte **Steamworks** + fiche **Coming Soon** (wishlist).
2. Compte Steam **autorisé** sur l’AppID (membre partenaire, ou licence *release override* tant que le jeu n’est pas sorti).
3. Event **Live-Stream / Broadcast** + compte streamer en whitelist (l’auteur de l’event et le compte qui streame doivent être **amis** Steam pour apparaître dans le picker).
4. OBS (ou Restream) : token RTMP Steam, **Broadcast AppID** = le **jeu de base** (pas un AppID démo).

Délai jusqu’à **~5 min** avant apparition sur la fiche : démarrer un peu tôt.

Docs Valve :

- FAQ unreleased : https://partner.steamgames.com/doc/store/broadcast/faq
- Setup OBS / event : https://partner.steamgames.com/doc/store/broadcast/setting_up
- Vue d’ensemble : https://partner.steamgames.com/doc/store/broadcast
- Streamer une démo vers la fiche du jeu de base : https://partner.steamgames.com/doc/store/broadcast/demo
- Restream → Steam : https://support.restream.io/how-to-stream-games-to-steam

### Démo vs jeu de base (plus tard)

Un live d’une **démo** n’apparaît **pas** tout seul sur la fiche du jeu. Il faut taguer le **Broadcast AppID** du **jeu de base**, et un compte **autorisé à « jouer »** le base game (Steamworks ou override). Ne pas lancer la démo **avec le même compte** que celui qui pousse le RTMP — Steam préférerait l’AppID « en cours de jeu » dans la bibliothèque.

## itch.io

Pas d’équivalent Twitch. Ne pas construire un workflow « stream itch ».

Sur la page : lien foyer live + Devlog *Live now* le jour J. Suffisant.

## X (Twitter)

Culture watch gamedev faible sur X Live (instable). Usage utile :

- clip + lien **15 min avant**
- post **go-live**
- recap GIF **après**

Garder YouTube ou Twitch comme chat principal.

## Ordre d’ajout

1. Foyer YouTube (ou Twitch) + restream existant (FB / Kick / l’autre).
2. **Steam Broadcast** dès Coming Soon.
3. **TikTok Live** = test.
4. Pas itch live, pas X Live en principal.

## Technique (pense-bête OBS)

- Restream / sorties multiples = ok ; modérer **un** chat.
- Steam : keyframe **2 s**, CBR (doc Valve).
- Overlay : titre de session + lien Discord / itch (puis wishlist).
- Mobile : scrcpy ou capture écran téléphone ; le visuel iso compte plus que « c’est un exe Steam ».
