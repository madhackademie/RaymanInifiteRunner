using UnityEngine;

/// <summary>
/// Metadonnees runtime minimales pour sauvegarder/restaurer une plante.
/// </summary>
public class PlantPersistenceMarker : MonoBehaviour
{
    public string PlantId { get; private set; }
    public Vector2Int Anchor { get; private set; }

    public void Initialise(string plantId, Vector2Int anchor)
    {
        PlantId = plantId;
        Anchor = anchor;
    }
}

