using UnityEngine;

/// <summary>
/// Art swap for a biofiltre bed. Grid size stays on <see cref="GridManager"/> (fixed in gameplay).
/// </summary>
[CreateAssetMenu(menuName = "Game/Data/Ferme/Bac biofiltre (skin)", fileName = "BiofiltreBed_")]
public class BiofiltreBedSkin : ScriptableObject
{
    [SerializeField] private Sprite sprite;

    public Sprite Sprite => sprite;
}
