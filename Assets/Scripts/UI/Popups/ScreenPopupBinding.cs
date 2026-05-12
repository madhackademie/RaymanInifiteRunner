using System;
using UnityEngine;

/// <summary>
/// Association runtime entre un ecran UIManager, un identifiant de popup et son prefab.
/// </summary>
[Serializable]
public class ScreenPopupBinding
{
    [Tooltip("Identifiant de l'ecran cible (utiliser ScreenId.*).")]
    public string screenId;

    [Tooltip("Identifiant unique du popup (utiliser PopupId.*).")]
    public string popupId;

    [Tooltip("Prefab instancie a la demande dans le ScreenPopupHost de l'ecran.")]
    public GameObject popupPrefab;
}
