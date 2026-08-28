using UnityEditor;
using UnityEngine;

/// <summary>
/// Purge objets runtime ferme + garde sélection Inspector (évite SerializedObjectNotCreatableException).
/// </summary>
[InitializeOnLoad]
public static class BiofiltreEditorCleanup
{
    private const string BedSpriteObjectName = "BedSprite";
    private const string GridContainerName     = "Grid";
    private const string PlantsContainerName   = "Plants";
    private const string CellNamePrefix        = "Cell_";

    static BiofiltreEditorCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.delayCall += RunOnceAfterLoad;
    }

    private static void RunOnceAfterLoad()
    {
        if (Application.isPlaying)
            return;

        PurgeAllBiofiltres();
        SanitizeSelection();
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingPlayMode:
                RetargetAllRuntimeFarmSelections();
                SanitizeSelection();
                ForceInspectorRebuild();
                break;

            case PlayModeStateChange.ExitingEditMode:
                PurgeAllBiofiltres();
                SanitizeSelection();
                break;

            case PlayModeStateChange.EnteredEditMode:
                PurgeAllBiofiltres();
                SanitizeSelection();
                ForceInspectorRebuild();
                EditorApplication.delayCall += RunPostEditModeCleanup;
                break;
        }
    }

    private static void RunPostEditModeCleanup()
    {
        if (Application.isPlaying)
            return;

        PurgeAllBiofiltres();
        SanitizeSelection();
        ForceInspectorRebuild();
    }

    [MenuItem("Rayman/Farm/Nettoyer sélection biofiltre")]
    private static void MenuCleanup()
    {
        PurgeAllBiofiltres();
        SanitizeSelection();
        ForceInspectorRebuild();
        Debug.Log("[Biofiltre] Sélection et objets runtime fantômes nettoyés.");
    }

    private static void PurgeBiofiltre(BiofiltreGridVisualizer visualizer)
    {
        if (visualizer == null || Application.isPlaying)
            return;

        Transform root = visualizer.transform;
        RetargetFromBiofiltreRuntime(root);
        DestroyRuntimeFarmObjectsUnder(root);
    }

    private static void PurgeAllBiofiltres()
    {
        BiofiltreGridVisualizer[] all = Object.FindObjectsByType<BiofiltreGridVisualizer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        for (int i = 0; i < all.Length; i++)
            PurgeBiofiltre(all[i]);
    }

    private static void RetargetAllRuntimeFarmSelections()
    {
        BiofiltreGridVisualizer[] all = Object.FindObjectsByType<BiofiltreGridVisualizer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null)
                RetargetFromBiofiltreRuntime(all[i].transform);
        }
    }

    private static void RetargetFromBiofiltreRuntime(Transform biofiltreRoot)
    {
        if (biofiltreRoot == null || !IsAnySelectionUnderRuntimeFarm(biofiltreRoot))
            return;

        Selection.activeGameObject = biofiltreRoot.gameObject;
    }

    private static void SanitizeSelection()
    {
        Object[] selected = Selection.objects;
        if (selected == null || selected.Length == 0)
            return;

        for (int i = 0; i < selected.Length; i++)
        {
            if (selected[i] == null || GetGameObjectFromSelection(selected[i]) == null)
            {
                Selection.activeObject = null;
                GUIUtility.hotControl = 0;
                EditorApplication.RepaintHierarchyWindow();
                return;
            }
        }
    }

    private static void ForceInspectorRebuild()
    {
        ActiveEditorTracker.sharedTracker.ForceRebuild();
        EditorApplication.RepaintHierarchyWindow();
    }

    private static void DestroyRuntimeFarmObjectsUnder(Transform root)
    {
        DestroyDirectChildIfExists(root, BedSpriteObjectName);

        Transform grid = root.Find(GridContainerName);
        if (grid != null)
        {
            for (int i = grid.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(grid.GetChild(i).gameObject);
        }

        Transform plants = root.Find(PlantsContainerName);
        if (plants != null)
        {
            for (int i = plants.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(plants.GetChild(i).gameObject);
        }
    }

    private static void DestroyDirectChildIfExists(Transform root, string childName)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == childName)
                Object.DestroyImmediate(child.gameObject);
        }
    }

    private static bool IsAnySelectionUnderRuntimeFarm(Transform biofiltreRoot)
    {
        Object[] selected = Selection.objects;
        if (selected == null)
            return false;

        for (int i = 0; i < selected.Length; i++)
        {
            GameObject go = GetGameObjectFromSelection(selected[i]);
            if (go != null && IsRuntimeFarmObject(go, biofiltreRoot))
                return true;
        }

        return false;
    }

    private static bool IsRuntimeFarmObject(GameObject go, Transform biofiltreRoot)
    {
        if (go == null || biofiltreRoot == null)
            return false;

        if (go.transform == biofiltreRoot)
            return false;

        if (!go.transform.IsChildOf(biofiltreRoot))
            return false;

        Transform t = go.transform;

        if (t.parent == biofiltreRoot && t.name == BedSpriteObjectName)
            return true;

        Transform grid = biofiltreRoot.Find(GridContainerName);
        if (grid != null && (t == grid || t.IsChildOf(grid)))
            return t != grid || HasGeneratedCellChildren(grid);

        Transform plants = biofiltreRoot.Find(PlantsContainerName);
        if (plants != null && t.IsChildOf(plants) && t != plants)
            return true;

        return t.name.StartsWith(CellNamePrefix, System.StringComparison.Ordinal);
    }

    private static bool HasGeneratedCellChildren(Transform grid)
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            if (grid.GetChild(i).name.StartsWith(CellNamePrefix, System.StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static GameObject GetGameObjectFromSelection(Object selected)
    {
        if (selected == null)
            return null;

        if (selected is GameObject go)
            return go;

        if (selected is Component component)
            return component.gameObject;

        return null;
    }
}
