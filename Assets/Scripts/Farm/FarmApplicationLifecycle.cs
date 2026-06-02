using UnityEngine;

/// <summary>
/// MonoBehaviour persistant pour <see cref="OnApplicationPause"/> / perte de focus (mobile + desktop).
/// </summary>
public class FarmApplicationLifecycle : MonoBehaviour
{
    private static FarmApplicationLifecycle instance;

    public static void EnsureExists()
    {
        if (instance != null)
            return;

        var host = new GameObject(nameof(FarmApplicationLifecycle));
        DontDestroyOnLoad(host);
        instance = host.AddComponent<FarmApplicationLifecycle>();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            FarmPersistenceCoordinator.FlushActiveFarm();
        else
            FarmPersistenceCoordinator.ApplyOfflineOnActiveFarm();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            FarmPersistenceCoordinator.FlushActiveFarm();
        else
            FarmPersistenceCoordinator.ApplyOfflineOnActiveFarm();
    }
}
