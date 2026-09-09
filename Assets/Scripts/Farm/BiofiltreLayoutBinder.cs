using UnityEngine;

/// <summary>
/// Applique un <see cref="BiofiltreLayoutDefinition"/> au <see cref="GridManager"/>
/// et au <see cref="BiofiltreIbcSpriteFitter"/> avant l'init grille (Awake).
/// </summary>
[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(BiofiltreIbcSpriteFitter))]
public class BiofiltreLayoutBinder : MonoBehaviour
{
    [SerializeField] private BiofiltreLayoutDefinition layoutDefinition;

    public BiofiltreLayoutDefinition LayoutDefinition => layoutDefinition;

    private void Awake()
    {
        if (layoutDefinition == null)
            return;

        GetComponent<GridManager>().ApplyBiofiltreLayout(layoutDefinition);
        GetComponent<BiofiltreIbcSpriteFitter>().ApplyBiofiltreLayout(layoutDefinition);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (layoutDefinition == null || !isActiveAndEnabled)
            return;

        if (!Application.isPlaying)
        {
            GetComponent<GridManager>().ApplyBiofiltreLayout(layoutDefinition);
            GetComponent<BiofiltreIbcSpriteFitter>().ApplyBiofiltreLayout(layoutDefinition);
        }
    }
#endif
}
