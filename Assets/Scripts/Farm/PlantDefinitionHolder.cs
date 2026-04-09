using UnityEngine;

/// <summary>
/// Lightweight component that carries a PlantDefinition reference on a plant instance.
/// Set by BiofiltreManager at placement time so harvest interactors can read plant data
/// without coupling to the placement pipeline.
/// </summary>
public class PlantDefinitionHolder : MonoBehaviour
{
    [SerializeField] private PlantDefinition definition;

    /// <summary>The PlantDefinition assigned to this plant instance.</summary>
    public PlantDefinition Definition => definition;

    /// <summary>Assigns the definition at runtime (called by BiofiltreManager after instantiation).</summary>
    public void Initialise(PlantDefinition plantDefinition)
    {
        definition = plantDefinition;
    }
}
