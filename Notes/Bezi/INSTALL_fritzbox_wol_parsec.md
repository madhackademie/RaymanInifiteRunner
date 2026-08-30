# Installation — FRITZ!Box + Windows + Wake-on-LAN + Parsec

**Date :** 2026-08-30  
**Contexte :** réveiller le PC Unity à distance, puis piloter Bezy via Parsec (voir `Notes/Bezi/ETUDE_prompts_bezi_distance.md`).  
**Stack cible :** PC Windows fixe (LAN Ethernet) → FRITZ!Box → MyFRITZ! / VPN → WoL → Parsec → Unity + Bezy.

---

## 1. Rôles de chaque brique

| Outil | Rôle | Démarre le PC ? |
|-------|------|-----------------|
| **FRITZ!Box WoL** | Envoie le paquet magique sur le LAN | ✅ (veille / parfois arrêt) |
| **MyFRITZ! / VPN FRITZ!** | Accès à la box depuis l’extérieur | Indirect (déclenche WoL) |
| **Parsec** | Bureau Windows à distance (Unity, Bezy) | ❌ — PC déjà allumé ou réveillé |
| **Cursor** | Prompts + `BEZY_QUEUE.md` sur Git | ❌ |

**Chaîne :** WoL FRITZ! → attendre Windows (~1–3 min) → Parsec → Bezy.

---

## 2. Prérequis matériels

- PC branché en **Ethernet** sur un port **LAN** de la FRITZ!Box (WoL peu fiable en Wi‑Fi seul).
- OK si le PC passe par un **FRITZ!Repeater** en LAN.
- Compte **MyFRITZ!** gratuit (adresse `xxxxx.myfritz.net`).
- FRITZ!Box avec IPv4 publique ou IPv6 (standard chez la plupart des FAI).

---

## 3. Windows — configuration WoL

### 3.1 Carte réseau Ethernet

1. `Win + R` → `ncpa.cpl` → carte **Ethernet** → **Propriétés** → **Configurer**.
2. Onglet **Gestion de l’alimentation** :
   - Décocher *« Autoriser l’ordinateur à désactiver ce périphérique pour économiser l’énergie »*.
   - Cocher *« Autoriser uniquement un paquet magique à réveiller l’ordinateur »*.
3. Onglet **Avancé** → **Wake on Magic Packet** → **Activé**.

### 3.2 Alimentation Windows

1. **Paramètres** → **Système** → **Alimentation** → mode **Performances élevées** ou **Équilibré** (éviter économie d’énergie agressive).
2. **Paramètres avancés** → **Veille** → autoriser la veille **S3**.
3. Pour les premiers tests : préférer **Mettre en veille** plutôt qu’**Arrêter** (WoL plus fiable).

### 3.3 BIOS / UEFI

Au démarrage (Del / F2 / F12 selon carte mère) :

| Option | Réglage |
|--------|---------|
| Wake on LAN / Power On By PCI-E | **Enabled** |
| ErP / EuP | souvent **Disabled** |
| Fast Boot | **Disabled** si WoL échoue |

Sauvegarder et redémarrer.

### 3.4 Parsec au démarrage (optionnel)

- Paramètres Parsec → lancer au démarrage / en service.
- Unity : lancement manuel ou raccourci **Démarrage** (au choix).

### 3.5 Session Windows après réveil

Parsec a besoin d’une session utilisateur active :

- tester connexion auto **ou** saisie mot de passe via clavier Parsec ;
- verrouillage Windows : vérifier que Parsec peut toujours se connecter.

---

## 4. FRITZ!Box — configuration WoL

### 4.1 MyFRITZ!

1. Interface `http://fritz.box` ou `192.168.178.1`.
2. Assistant **MyFRITZ!** → créer / lier compte AVM.
3. Noter l’adresse **`xxxxx.myfritz.net`**.

### 4.2 Activer WoL pour le PC Unity

1. **Réseau local** (*Heimnetz*) → **Réseau** (*Netzwerk*) → onglet **Connexions réseau**.
2. **(Modifier)** à côté du PC Windows (nom fixe recommandé).
3. Onglet **LAN**.
4. Cocher : **« Démarrer cet appareil automatiquement dès qu’il est accessible depuis Internet »**  
   *(DE : « Dieses Gerät automatiquement starten, wenn es aus dem Internet angesprochen wird »)*.
5. **Appliquer**.

### 4.3 Test local (obligatoire avant remote)

1. Mettre le PC en **veille**.
2. Interface FRITZ! → même fiche PC → bouton **« Démarrer l’appareil »** (*Start Device*).
3. Le PC doit se réveiller en quelques secondes.

Si échec → revoir § 3 (BIOS + carte Ethernet).

**Doc AVM :** [Wake on LAN via Internet](https://en.fritz.com/service/knowledge-base/dok/FRITZ-Box-4050/36_Starting-network-devices-over-the-internet-Wake-on-LAN/)

---

## 5. Accès depuis l’extérieur (téléphone, 4G)

### Option A — VPN FRITZ! (recommandé)

1. FRITZ!Box → **Système** → **Utilisateurs FRITZ!** → utilisateur avec droit **VPN**.
2. Téléphone :
   - **WireGuard** (FRITZ!OS récent) — exporter config depuis la box, **ou**
   - **FRITZ!App Fon** / **FRITZ!Fernzugang** (Windows).
3. Une fois le VPN actif → accès réseau local `192.168.178.x`.
4. Déclencher WoL :
   - interface MyFRITZ! → **Démarrer l’appareil**, **ou**
   - WoL auto si accès Internet configuré (port partagé / VPN).

### Option B — MyFRITZ! navigateur

1. `https://xxxxx.myfritz.net` → connexion box.
2. **Réseau local** → PC → **Démarrer l’appareil**.

### Option C — App WoL + VPN

Après connexion VPN FRITZ!, app **Wake On Lan** (iOS/Android) avec l’**adresse MAC** du PC.

---

## 6. Parsec — installation rapide

1. Installer **Parsec** sur le PC hôte + créer un compte ([parsec.app](https://parsec.app)).
2. Installer le client Parsec sur téléphone / laptop.
3. Même compte → le PC apparaît dans la liste → **Connect**.

**Réglages PC hôte (alimentation) :**

```text
Veille profonde : désactivée sur secteur
Veille écran    : 10–15 min OK
```

PowerShell (admin, session courante — à adapter) :

```powershell
powercfg /change standby-timeout-ac 0
powercfg /change monitor-timeout-ac 15
```

---

## 7. Workflow Bezy complet

| # | Action | Où |
|---|--------|-----|
| 1 | Cursor prépare phase + `BEZY_QUEUE.md` + `git push` | Cloud / téléphone |
| 2 | PC en **veille** (Unity sauvé) | Maison |
| 3 | VPN FRITZ! + **Démarrer l’appareil** (WoL) | Téléphone |
| 4 | Attendre Windows + Parsec | ~1–3 min |
| 5 | Parsec → `git pull` → Bezy `/prefab-ui-3phases` → Keep | Remote |
| 6 | `git commit` + push | Remote |
| 7 | Veille PC (optionnel) | — |
| 8 | Cursor review diff lendemain | Cloud |

**File opérationnelle :** `Notes/Bezi/BEZY_QUEUE.md`

---

## 8. Dépannage FRITZ!Box

| Symptôme | Piste |
|----------|-------|
| WoL auto ne part pas | PC actif il y a **&lt; 15 min** → bouton manuel **Démarrer l’appareil** |
| Aucun réveil | Ethernet ? BIOS WoL ? Magic Packet Windows ? |
| WoL après Arrêt Windows seulement | Préférer **veille** ; certains PC ne supportent pas S5 |
| MyFRITZ! inaccessible | IPv6 / compte MyFRITZ! / double NAT opérateur |
| Parsec ne connecte pas | Windows pas fini de boot ; Parsec pas lancé ; pare-feu |

---

## 9. Sécurité

- Mot de passe fort **FRITZ!Box**, **MyFRITZ!**, compte **VPN**.
- Parsec : activer **2FA**.
- Ne pas exposer **RDP** sur Internet (Parsec suffit).
- WoL + Parsec = accès total au PC — protéger l’accès physique et les comptes.

---

## 10. Alternatives (si WoL FRITZ! insuffisant)

| Solution | Usage |
|----------|--------|
| **Prise connectée** (Shelly, etc.) | Allumer après arrêt propre — **jamais** couper sous Windows actif |
| **PC toujours allumé** | Pas de WoL ; écran éteint ; Parsec direct |
| **Tailscale + RDP** | Alternative à Parsec (latence UI plus élevée) |

---

## 11. Références croisées

| Fichier | Sujet |
|---------|--------|
| `Notes/Bezi/ETUDE_prompts_bezi_distance.md` | Étude distance + stack vocal |
| `Notes/Bezi/BEZY_QUEUE.md` | File phases Bezy |
| `Notes/Bezi/WORKFLOW_skill_prefab_ui.md` | Skill `/prefab-ui-3phases` |
| [FRITZ!Box WoL (AVM)](https://en.fritz.com/service/knowledge-base/dok/FRITZ-Box-4050/36_Starting-network-devices-over-the-internet-Wake-on-LAN/) | Doc officielle |

---

## 12. Checklist première mise en service

- [ ] Ethernet PC → FRITZ!Box LAN
- [ ] BIOS Wake on LAN activé
- [ ] Windows Magic Packet activé
- [ ] FRITZ! : option « démarrer depuis Internet » cochée
- [ ] Test bouton **Démarrer l’appareil** (veille) OK
- [ ] MyFRITZ! + VPN téléphone testés hors domicile
- [ ] Parsec installé hôte + client, connexion OK
- [ ] Test bout en bout : WoL → Parsec → `git pull` → 1 phase Bezy test
