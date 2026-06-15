using UnityEditor;
using UnityEngine;

/// <summary>
/// Wiring overlay talents : binding Track_Commerce + nettoyage TreeContent.
/// </summary>
public static class TalentTreeOverlayWiring
{
    private const string InventoryScreenPath = "Assets/Prefabs/Ui/InventoryScreen.prefab";
    private const string TrackCommercePath = "Assets/Prefabs/Ui/Progression/Trees/Track_Commerce.prefab";
    private const string CommerceTrackId = "track.commerce";

    [MenuItem("Rayman/UI/Wire Track Commerce Binding (overlay)")]
    public static void WireTrackCommerceBinding()
    {
        var trackPrefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(TrackCommercePath);
        var layoutRoot = trackPrefabRoot != null
            ? trackPrefabRoot.GetComponent<TalentTreeLayoutRoot>()
            : null;

        if (layoutRoot == null)
        {
            Debug.LogError(
                $"[TalentTree] Prefab introuvable ou sans TalentTreeLayoutRoot : {TrackCommercePath}");
            return;
        }

        if (trackPrefabRoot.GetComponent<RectTransform>() == null)
        {
            Debug.LogWarning(
                "[TalentTree] Track_Commerce n'a pas de RectTransform sur la racine — " +
                "corrige le prefab avant playtest (UI → Empty).");
        }

        var screenRoot = PrefabUtility.LoadPrefabContents(InventoryScreenPath);
        try
        {
            var overlay = screenRoot.GetComponentInChildren<TalentTreeOverlayController>(true);
            if (overlay == null)
            {
                Debug.LogError("[TalentTree] TalentTreeOverlayController introuvable.");
                return;
            }

            var serialized = new SerializedObject(overlay);
            SerializedProperty bindings = serialized.FindProperty("trackPrefabBindings");
            if (bindings == null)
            {
                Debug.LogError("[TalentTree] Propriete trackPrefabBindings introuvable.");
                return;
            }

            bindings.arraySize = 1;
            SerializedProperty entry = bindings.GetArrayElementAtIndex(0);
            entry.FindPropertyRelative("trackId").stringValue = CommerceTrackId;
            entry.FindPropertyRelative("treePrefab").objectReferenceValue = layoutRoot;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            RemoveEmbeddedTrackInstances(screenRoot.transform);

            PrefabUtility.SaveAsPrefabAsset(screenRoot, InventoryScreenPath);
            AssetDatabase.SaveAssets();
            Debug.Log(
                "[TalentTree] Binding OK : track.commerce → Track_Commerce. " +
                "Instances Track_Commerce sous TreeContent supprimees.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(screenRoot);
        }
    }

    private static void RemoveEmbeddedTrackInstances(Transform root)
    {
        Transform treeContent = FindChildRecursive(root, "TreeContent");
        if (treeContent == null)
            return;

        for (int i = treeContent.childCount - 1; i >= 0; i--)
        {
            Transform child = treeContent.GetChild(i);
            if (child.name.Contains("Track_Commerce"))
                Object.DestroyImmediate(child.gameObject);
        }
    }

    private static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindChildRecursive(parent.GetChild(i), childName);
            if (found != null)
                return found;
        }

        return null;
    }
}
