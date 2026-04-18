using UnityEngine;

/// <summary>
/// Définition d'une entrée du hub de navigation.
/// Chaque entrée correspond à un mode de jeu ou une zone accessible depuis l'écran d'accueil.
/// </summary>
[CreateAssetMenu(fileName = "HubEntry_", menuName = "Game/Hub Entry Data")]
public class MapNodeData : ScriptableObject
{
    [Tooltip("Identifiant unique de l'entrée, utilisé comme clé dans MapProgressionData.")]
    public string nodeId;

    [Tooltip("Nom affiché sur le bouton.")]
    public string displayName;

    [Tooltip("Sous-titre ou description courte affichée sous le nom (optionnel).")]
    public string subtitle;

    [Tooltip("Nom de la scène Unity à charger quand le joueur clique sur ce bouton.")]
    public string targetSceneName;

    [Tooltip("Ordre d'affichage dans la liste de boutons (croissant = en haut).")]
    public int sortOrder = 0;

    [Tooltip("Ce bouton est accessible dès le démarrage d'une nouvelle partie.")]
    public bool unlockedByDefault = false;

    [Tooltip("Sprite affiché sur le bouton quand il est déverrouillé.")]
    public Sprite unlockedSprite;

    [Tooltip("Sprite affiché sur le bouton quand il est verrouillé.")]
    public Sprite lockedSprite;
}
