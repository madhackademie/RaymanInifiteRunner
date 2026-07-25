using UnityEngine;

/// <summary>
/// Spawns the shared planting dirt / worm particle burst (plant, uproot, harvest).
/// Prefab: <c>Assets/Prefabs/World/VFX/PlantingDirtBurst.prefab</c>.
/// </summary>
public static class FarmDirtBurstVfx
{
    private const float DefaultDestroyDelaySeconds = 2f;

    /// <summary>
    /// Instantiates the burst at <paramref name="worldPosition"/> and plays all child ParticleSystems.
    /// </summary>
    public static void Play(GameObject prefab, Vector3 worldPosition, float destroyDelaySeconds = DefaultDestroyDelaySeconds)
    {
        if (prefab == null)
            return;

        GameObject instance = Object.Instantiate(prefab, worldPosition, Quaternion.identity);
        instance.name = prefab.name;

        ParticleSystem[] systems = instance.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < systems.Length; i++)
            systems[i].Play(withChildren: false);

        float delay = destroyDelaySeconds > 0f ? destroyDelaySeconds : DefaultDestroyDelaySeconds;
        Object.Destroy(instance, delay);
    }
}
