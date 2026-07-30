using UnityEngine;

/// <summary>
/// Ancre VFX sparkle sous une plante : actif seulement si le stade courant est récoltable.
/// Miroir léger de <see cref="InsectPathAnchor"/> (Flowering).
/// </summary>
public class HarvestReadyFxAnchor : MonoBehaviour
{
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
        PlaySystems();
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
