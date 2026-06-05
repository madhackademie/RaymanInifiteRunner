using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wiring Phase 3 inventaire halo — menu Unity (pas Bezy).
/// </summary>
public static class InventoryHaloPrefabWiring
{
    private const string InventoryScreenPath = "Assets/Prefabs/Ui/InventoryScreen.prefab";

    [MenuItem("Rayman/UI/Wire Inventory Halo (Phase 3)")]
    public static void WirePhase3()
    {
        var screenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InventoryScreenPath);
        if (screenPrefab == null)
        {
            Debug.LogError($"[InventoryHalo] Prefab introuvable : {InventoryScreenPath}");
            return;
        }

        var root = PrefabUtility.LoadPrefabContents(InventoryScreenPath);
        try
        {
            if (!WireScreen(root))
            {
                Debug.LogWarning("[InventoryHalo] Wiring incomplet — voir logs.");
                return;
            }

            PrefabUtility.SaveAsPrefabAsset(root, InventoryScreenPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[InventoryHalo] Phase 3 OK — InventoryScreen sauvegardé.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static bool WireScreen(GameObject root)
    {
        var haloPanel = root.GetComponentInChildren<PlayerHaloPanelController>(true);
        var overlay = FindChild(root.transform, "TalentTreeOverlay");
        var inventoryPanel = FindChild(root.transform, "InventoryPanel");
        var filterBar = FindChild(root.transform, "FilterBarPlaceholder");

        if (haloPanel == null || overlay == null || inventoryPanel == null)
        {
            Debug.LogError("[InventoryHalo] Modules manquants (halo / overlay / inventory panel).");
            return false;
        }

        WireOverlay(overlay);
        WireScreenController(root, haloPanel, overlay, inventoryPanel, filterBar);
        return true;
    }

    private static void WireOverlay(GameObject overlayRoot)
    {
        var controller = GetOrAdd<TalentTreeOverlayController>(overlayRoot);

        var canvasGroup = overlayRoot.GetComponent<CanvasGroup>();
        var trackTitle = FindChild(overlayRoot.transform, "TrackTitle")?.GetComponent<TextMeshProUGUI>();
        var bodyText = FindChild(overlayRoot.transform, "BodyText")?.GetComponent<TextMeshProUGUI>();
        if (bodyText == null)
            bodyText = FindChild(overlayRoot.transform, "BodyPlaceholder")?.GetComponentInChildren<TextMeshProUGUI>(true);

        var backButton = FindChild(overlayRoot.transform, "BackButton")?.GetComponent<Button>();

        SetSerialized(controller, "overlayRoot", overlayRoot);
        SetSerialized(controller, "canvasGroup", canvasGroup);
        SetSerialized(controller, "trackTitleLabel", trackTitle);
        SetSerialized(controller, "bodyPlaceholderLabel", bodyText);
        SetSerialized(controller, "backButton", backButton);
        SetSerialized(controller, "animator", overlayRoot.GetComponent<Animator>());
    }

    private static void WireScreenController(
        GameObject root,
        PlayerHaloPanelController haloPanel,
        GameObject overlayRoot,
        GameObject inventoryPanel,
        GameObject filterBar)
    {
        var controller = GetOrAdd<InventoryScreenController>(root);
        var overlayController = overlayRoot.GetComponent<TalentTreeOverlayController>();
        var bodyGroup = inventoryPanel.GetComponent<CanvasGroup>();

        SetSerialized(controller, "haloPanel", haloPanel);
        SetSerialized(controller, "talentTreeOverlay", overlayController);
        SetSerialized(controller, "inventoryBodyCanvasGroup", bodyGroup);
        SetSerialized(controller, "filterBarPlaceholder", filterBar);
        SetFloat(controller, "inventoryDimAlphaWhenTreeOpen", 0.35f);
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    private static GameObject FindChild(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name)
                return t.gameObject;
        }

        return null;
    }

    private static void SetSerialized(Object target, string propertyName, Object value)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(propertyName);
        if (prop == null)
        {
            Debug.LogWarning($"[InventoryHalo] Propriété absente : {propertyName} sur {target.name}");
            return;
        }

        prop.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string propertyName, float value)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(propertyName);
        if (prop == null)
        {
            Debug.LogWarning($"[InventoryHalo] Propriété absente : {propertyName} sur {target.name}");
            return;
        }

        prop.floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
