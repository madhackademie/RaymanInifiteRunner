# Playtest live mobile — workflow (conditions réelles)

**Tél. de référence :** Samsung **SM-A137F** (`armeabi-v7a` only — pas d’ARM64).  
**Statut bugs playtest :** `Notes/Todo_project.md` § *Playtest mobile* · dump `Notes/Todo_playtest.md` Batch H.  
Cette note = **comment jouer / logger / itérer sur le téléphone**, pas le statut des bugs.

---

## 1) Logs Unity sur le PC (écran tél. illisible)

`adb` n’est **pas** dans le PATH PowerShell. Script à la racine du repo :

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\adb-unity-logcat.ps1
```

Équivalent :

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" logcat -s Unity
```

Vers un fichier :

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" logcat -s Unity > playtest-unity.log
```

**Quand relancer :** au début d’une session playtest (USB + débogage). Laisser la fenêtre ouverte = pas besoin de relancer entre deux lancements de l’app. Relancer si tu fermes le terminal, débranches, ou `waiting for device`. **Ctrl+C** pour arrêter.

Vérif ABI (ce tél. doit rester `armeabi-v7a`) :

```powershell
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" shell getprop ro.product.cpu.abi
```

---

## 2) Build playtest (temps)

- **Pendant IL2CPP / Gradle :** le tél. peut dormir.
- **Pendant Build And Run (install USB) :** écran allumé, pas de veille.
- **Archis playtest local :** **ARMv7 seul** (décocher ARM64). Recocher ARM64 avant un build **Store**. Ne jamais dropper ARMv7 pour ce SM-A137F.
- Premier IL2CPP dual-archi ≈ 40 min ; ensuite plus court si cache + ARM64 off.
- **Development Build** : overlay erreurs sur le tél. (trop petit) + plus de `Debug.Log` dans logcat. Pour lire : **toujours logcat PC**.

Shader halo sélection (`Farm/SpriteSelectionSilhouette`) : dans Always Included Shaders (2026-09-16). **Prochain APK** obligatoire pour le fix.

---

## 3) Captures utiles (cadrage portrait)

Home : 5 onglets + Plus à droite ; wallet / PA vs notch.  
FirstLvl : biofiltre entier ; croix haut-gauche hors trou caméra ; **pas** de barre nav.

---

## 4) File workflow (à enrichir)

Idées pour les prochaines sessions — **pas** des bugs jeu. Les ajouter ici quand on les tranche.

- Package Unity **Android Logcat** (`Window > Analysis > Android Logcat`) pour coller les logs dans l’éditeur.
- Alias / profil PowerShell pour `adb` dans le PATH.
- Development Build : case à cocher par défaut sur le Build Profile playtest (sans l’oublier au Store).
- Rappel **LOW_MEMORY** A13 : fermer Chrome / autres apps avant playtest.
- Captures : dossier Dump `Assets/Art/Assets Store Dump/Ui/PlaytestMobile/` + date.
- Script « devices + abi + logcat » en une commande.
- Désactiver veille USB (option développeur Android) avant Build And Run.
