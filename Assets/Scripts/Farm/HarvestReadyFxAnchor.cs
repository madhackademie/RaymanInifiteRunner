using UnityEngine;

/// <summary>
/// Ancre VFX sparkle sous une plante : actif seulement si le stade courant est récoltable.
/// Miroir léger de <see cref="InsectPathAnchor"/> (Flowering).
/// </summary>
public class HarvestReadyFxAnchor : MonoBehaviour
{
    private const int SortingOrderBoostVsPlant = 2;

    [Tooltip("Si vide, récupère tous les ParticleSystem enfants (incl. inactifs).")]
    [SerializeField] private ParticleSystem[] sparkleSystems;

    /// <summary>Active/désactive l’ancre et lance/arrête les particles (Play On Awake est OFF).</summary>
    public void SetFxActive(bool active)
    {
        if (!active)
        {
            StopSystems();
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        CacheSystemsIfNeeded();
        SyncSortingWithPlant(FindAncestorPlantRenderer());
        PlaySystems();
    }

    /// <summary>Sparkles au-dessus du sprite plante (même couche, order + boost).</summary>
    public void SyncSortingWithPlant(SpriteRenderer plantRenderer)
    {
        if (plantRenderer == null)
            return;

        ParticleSystemRenderer[] renderers = GetComponentsInChildren<ParticleSystemRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            ParticleSystemRenderer particleRenderer = renderers[i];
            if (particleRenderer == null)
                continue;

            particleRenderer.sortingLayerID = plantRenderer.sortingLayerID;
            particleRenderer.sortingOrder = plantRenderer.sortingOrder + SortingOrderBoostVsPlant;
        }
    }

    private SpriteRenderer FindAncestorPlantRenderer()
    {
        Transform ancestor = transform.parent;
        while (ancestor != null)
        {
            if (ancestor.TryGetComponent(out SpriteRenderer plantRenderer))
                return plantRenderer;

            ancestor = ancestor.parent;
        }

        return null;
    }

    private void CacheSystemsIfNeeded()
    {
        if (sparkleSystems != null && sparkleSystems.Length > 0)
            return;

        sparkleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    private void PlaySystems()
    {
        if (sparkleSystems == null)
            return;

        for (int i = 0; i < sparkleSystems.Length; i++)
        {
            ParticleSystem ps = sparkleSystems[i];
            if (ps == null)
                continue;

            if (!ps.isPlaying)
                ps.Play(true);
        }
    }

    private void StopSystems()
    {
        CacheSystemsIfNeeded();
        if (sparkleSystems == null)
            return;

        for (int i = 0; i < sparkleSystems.Length; i++)
        {
            ParticleSystem ps = sparkleSystems[i];
            if (ps == null)
                continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sparkleSystems == null || sparkleSystems.Length == 0)
            sparkleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }
#endif
}
