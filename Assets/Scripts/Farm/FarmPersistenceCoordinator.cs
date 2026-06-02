using UnityEngine;

/// <summary>
/// Flush ferme à la fermeture / mise en arrière-plan du jeu (shell persistant).
/// Les scènes désactivées ne reçoivent pas <see cref="MonoBehaviour.OnApplicationQuit"/>.
/// </summary>
public static class FarmPersistenceCoordinator
{
    private static BiofiltreManager activeManager;
    private static bool lifecycleHooked;

    public static void Register(BiofiltreManager manager)
    {
        if (manager == null)
            return;

        activeManager = manager;
        EnsureLifecycleHooks();
    }

    public static void Unregister(BiofiltreManager manager)
    {
        if (activeManager == manager)
            activeManager = null;
    }

    /// <summary>Sauvegarde immédiate si la ferme est la scène active en mémoire.</summary>
    public static void FlushActiveFarm()
    {
        if (activeManager == null)
            return;

        activeManager.FlushPersistence();
    }

    /// <summary>Recalcule la croissance offline au retour focus / reprise (jeu toujours en ferme).</summary>
    public static void ApplyOfflineOnActiveFarm()
    {
        if (activeManager == null)
            return;

        activeManager.ApplyOfflinePersistence();
    }

    private static void EnsureLifecycleHooks()
    {
        if (lifecycleHooked)
            return;

        lifecycleHooked = true;
        Application.quitting += FlushActiveFarm;
        FarmApplicationLifecycle.EnsureExists();
    }
}
