using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Host de popups pour un ecran UI : instanciation lazy de prefabs puis show/hide par identifiant.
/// </summary>
public class ScreenPopupHost : MonoBehaviour
{
    [Serializable]
    private class PopupEntry
    {
        public string popupId;
        public GameObject popupPrefab;
        public Transform parentOverride;
        public bool preloadOnAwake;

        /// <summary>Instance scène déjà vivante (pas de Instantiate depuis prefab).</summary>
        public bool useLiveInstance;

        [NonSerialized] public GameObject instance;
    }

    [Header("Popup roots")]
    [Tooltip("Parent par defaut pour les popups instancies. Si vide, utilise ce transform.")]
    [SerializeField] private Transform defaultPopupRoot;

    [Header("Popup prefabs")]
    [SerializeField] private List<PopupEntry> popupEntries = new();

    private readonly Dictionary<string, PopupEntry> popupRegistry = new();

    private void Awake()
    {
        BuildRegistry();
        PreloadEntriesIfNeeded();
    }

    public bool RegisterRuntimePopup(string popupId, GameObject popupPrefab, Transform parentOverride = null)
    {
        if (string.IsNullOrWhiteSpace(popupId) || popupPrefab == null)
            return false;

        if (popupRegistry.TryGetValue(popupId, out PopupEntry existing))
        {
            if (existing.useLiveInstance && existing.instance != null)
                return true;

            if (existing.instance != null && existing.popupPrefab != popupPrefab)
            {
                Debug.LogWarning(
                    $"[ScreenPopupHost] Popup '{popupId}' deja instancie. " +
                    "Impossible de remplacer le prefab a chaud.",
                    this);
                return false;
            }

            existing.popupPrefab = popupPrefab;
            if (parentOverride != null)
                existing.parentOverride = parentOverride;

            return true;
        }

        var entry = new PopupEntry
        {
            popupId = popupId,
            popupPrefab = popupPrefab,
            parentOverride = parentOverride,
            preloadOnAwake = false
        };

        popupEntries.Add(entry);
        popupRegistry[popupId] = entry;
        return true;
    }

    /// <summary>
    /// Enregistre une instance déjà présente en scène (ex. UI ferme posée dans FirstLvl).
    /// A la priorité sur un prefab du même identifiant si appelé avant toute instanciation.
    /// </summary>
    public bool RegisterRuntimePopupLiveInstance(string popupId, GameObject liveRoot, Transform parentOverride = null)
    {
        if (string.IsNullOrWhiteSpace(popupId) || liveRoot == null)
            return false;

        if (popupRegistry.TryGetValue(popupId, out PopupEntry existing))
        {
            if (existing.instance != null && existing.instance != liveRoot)
            {
                Debug.LogWarning(
                    $"[ScreenPopupHost] Popup '{popupId}' deja enregistre avec une autre instance.",
                    this);
                return false;
            }

            if (existing.instance == liveRoot)
                return true;

            // Slot prefab sans instance : promouvoir l'instance scène (même identifiant).
            existing.useLiveInstance = true;
            existing.popupPrefab = null;
            existing.instance = liveRoot;
            if (parentOverride != null)
                existing.parentOverride = parentOverride;

            return true;
        }

        var entry = new PopupEntry
        {
            popupId = popupId,
            popupPrefab = null,
            parentOverride = parentOverride,
            preloadOnAwake = false,
            useLiveInstance = true,
            instance = liveRoot
        };

        popupEntries.Add(entry);
        popupRegistry.Add(popupId, entry);
        return true;
    }

    public bool TryGetPopup<T>(string popupId, out T popup) where T : Component
    {
        popup = null;
        if (!TryResolveEntry(popupId, out PopupEntry entry))
            return false;

        GameObject instance = EnsureInstance(entry);
        if (instance == null)
            return false;

        popup = instance.GetComponent<T>();
        if (popup != null)
            return true;

        Debug.LogWarning(
            $"[ScreenPopupHost] Le popup '{popupId}' n'a pas de composant {typeof(T).Name}.",
            this);
        return false;
    }

    public bool HasPopup(string popupId)
    {
        if (string.IsNullOrWhiteSpace(popupId))
            return false;

        return popupRegistry.ContainsKey(popupId);
    }

    public bool TryShowPopup<T>(string popupId, out T popup) where T : Component
    {
        popup = null;
        if (!TryGetPopup(popupId, out T resolved))
            return false;

        resolved.gameObject.SetActive(true);
        resolved.transform.SetAsLastSibling();
        popup = resolved;
        return true;
    }

    public bool TryHidePopup(string popupId)
    {
        if (!TryResolveEntry(popupId, out PopupEntry entry))
            return false;

        if (entry.instance == null)
            return false;

        entry.instance.SetActive(false);
        return true;
    }

    private void BuildRegistry()
    {
        popupRegistry.Clear();

        foreach (PopupEntry entry in popupEntries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.popupId))
            {
                Debug.LogWarning("[ScreenPopupHost] PopupEntry invalide ignoree.", this);
                continue;
            }

            if (popupRegistry.ContainsKey(entry.popupId))
            {
                Debug.LogWarning($"[ScreenPopupHost] popupId duplique '{entry.popupId}' ignore.", this);
                continue;
            }

            popupRegistry[entry.popupId] = entry;
        }
    }

    private void PreloadEntriesIfNeeded()
    {
        foreach (PopupEntry entry in popupRegistry.Values)
        {
            if (!entry.preloadOnAwake)
                continue;

            EnsureInstance(entry);
        }
    }

    private bool TryResolveEntry(string popupId, out PopupEntry entry)
    {
        if (popupRegistry.TryGetValue(popupId, out entry))
            return true;

        Debug.LogWarning($"[ScreenPopupHost] Popup inconnu: '{popupId}'.", this);
        return false;
    }

    private GameObject EnsureInstance(PopupEntry entry)
    {
        if (entry.instance != null)
            return entry.instance;

        if (entry.useLiveInstance)
            return entry.instance;

        if (entry.popupPrefab == null)
        {
            Debug.LogWarning($"[ScreenPopupHost] Prefab manquant pour popup '{entry.popupId}'.", this);
            return null;
        }

        entry.instance = Instantiate(entry.popupPrefab, ResolvePopupParent(entry));
        entry.instance.name = entry.popupPrefab.name;
        entry.instance.SetActive(false);
        return entry.instance;
    }

    private Transform ResolvePopupParent(PopupEntry entry)
    {
        if (entry.parentOverride != null)
            return entry.parentOverride;

        if (defaultPopupRoot != null)
            return defaultPopupRoot;

        return transform;
    }
}
