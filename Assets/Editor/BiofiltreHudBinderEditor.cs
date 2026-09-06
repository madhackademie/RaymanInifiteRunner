using UnityEditor;
using UnityEngine;

/// <summary>
/// Poignées Scene pour déplacer les rows HUD.
/// En Play : cache mémoire. À la sortie du Play : écriture Edit (puis Ctrl+S).
/// </summary>
[CustomEditor(typeof(BiofiltreHudBinder))]
public class BiofiltreHudBinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "Scene view : sélectionne le Biofiltre (outil Move), pas IbcSprite.\n" +
            "• Jaune = primaire  • Or = étoiles  • Magenta = secondaire\n" +
            "Play : déplace, puis STOP Play, puis Ctrl+S. Unity n'autorise pas Save en Play.",
            MessageType.Info);
    }
}

[InitializeOnLoad]
internal static class BiofiltreHudRowSceneHandles
{
    private static readonly Color PrimaryColor = new(1f, 0.92f, 0.2f, 1f);
    private static readonly Color StarColor = new(1f, 0.55f, 0.1f, 1f);
    private static readonly Color SecondaryColor = new(0.95f, 0.25f, 0.75f, 1f);
    private const float HandleScale = 0.28f;
    private const string PrefabAssetPath = "Assets/Prefabs/World/Biofiltre.prefab";

    private static HudRowLayout pendingLayout;
    private static bool hasPendingLayout;

    private struct HudRowLayout
    {
        public Vector2 primaryAnchor;
        public Vector2 primaryOffset;
        public Vector2 starAnchor;
        public Vector2 starOffset;
        public Vector2 secondaryAnchor;
        public Vector2 secondaryOffset;
    }

    static BiofiltreHudRowSceneHandles()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode || !hasPendingLayout)
            return;

        ApplyLayout(pendingLayout);
        hasPendingLayout = false;
        Debug.Log("[BiofiltreHudBinder] Positions HUD reprises après Play — Ctrl+S en Edit pour sauver.");
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!TryGetBinder(out BiofiltreHudBinder binder, out GridManager grid))
            return;

        grid.RebuildMapperFromInspector();
        Rect worldRect = grid.GetWorldRect();
        var so = new SerializedObject(binder);
        so.Update();

        bool changed = DrawRowHandle(
            so, "primaryNormalizedAnchor", "primaryWorldOffset", worldRect, PrimaryColor, "Primaire");
        changed |= DrawRowHandle(
            so, "starNormalizedAnchor", "starWorldOffset", worldRect, StarColor, "Etoiles");
        changed |= DrawRowHandle(
            so, "secondaryNormalizedAnchor", "secondaryWorldOffset", worldRect, SecondaryColor, "Secondaire");

        if (!changed)
            return;

        so.ApplyModifiedProperties();
        binder.RecalculateHudPositions();

        HudRowLayout layout = ReadLayout(so);
        if (Application.isPlaying)
        {
            pendingLayout = layout;
            hasPendingLayout = true;
        }
        else
            ApplyLayout(layout);
    }

    private static bool TryGetBinder(out BiofiltreHudBinder binder, out GridManager grid)
    {
        binder = null;
        grid = null;
        GameObject go = Selection.activeGameObject;
        if (go == null)
            return false;

        if (go.GetComponent<SpriteRenderer>() != null
            && go.GetComponentInParent<BiofiltreIbcSpriteFitter>() != null
            && go.GetComponent<BiofiltreIbcSpriteFitter>() == null)
            return false;

        binder = go.GetComponent<BiofiltreHudBinder>()
                 ?? go.GetComponentInParent<BiofiltreHudBinder>();
        if (binder == null)
            return false;

        grid = binder.GetComponent<GridManager>();
        return grid != null;
    }

    private static bool DrawRowHandle(
        SerializedObject so,
        string anchorProp,
        string offsetProp,
        Rect worldRect,
        Color color,
        string label)
    {
        Vector2 anchor = so.FindProperty(anchorProp).vector2Value;
        Vector2 offset = so.FindProperty(offsetProp).vector2Value;
        Vector3 pos = new(
            worldRect.x + anchor.x * worldRect.width + offset.x,
            worldRect.y + anchor.y * worldRect.height + offset.y,
            0f);

        float size = HandleUtility.GetHandleSize(pos) * HandleScale;
        Handles.color = color;
        Handles.DrawSolidDisc(pos, Vector3.forward, size * 0.4f);
        Handles.Label(pos + Vector3.up * (size * 1.2f), label);

        EditorGUI.BeginChangeCheck();
        Vector3 dragged = Handles.Slider2D(
            pos, Vector3.forward, Vector3.right, Vector3.up, size, Handles.DotHandleCap, Vector2.zero);
        if (!EditorGUI.EndChangeCheck())
            return false;

        float width = Mathf.Max(0.01f, worldRect.width);
        float height = Mathf.Max(0.01f, worldRect.height);
        so.FindProperty(anchorProp).vector2Value = new Vector2(
            (dragged.x - worldRect.x - offset.x) / width,
            (dragged.y - worldRect.y - offset.y) / height);
        GUI.changed = true;
        return true;
    }

    private static HudRowLayout ReadLayout(SerializedObject so)
    {
        return new HudRowLayout
        {
            primaryAnchor = so.FindProperty("primaryNormalizedAnchor").vector2Value,
            primaryOffset = so.FindProperty("primaryWorldOffset").vector2Value,
            starAnchor = so.FindProperty("starNormalizedAnchor").vector2Value,
            starOffset = so.FindProperty("starWorldOffset").vector2Value,
            secondaryAnchor = so.FindProperty("secondaryNormalizedAnchor").vector2Value,
            secondaryOffset = so.FindProperty("secondaryWorldOffset").vector2Value,
        };
    }

    private static void ApplyLayout(HudRowLayout layout)
    {
        BiofiltreHudBinder[] sceneBinders = Object.FindObjectsByType<BiofiltreHudBinder>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < sceneBinders.Length; i++)
            WriteLayout(sceneBinders[i], layout);

        GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
        if (prefabRoot == null)
            return;

        BiofiltreHudBinder prefabBinder = prefabRoot.GetComponent<BiofiltreHudBinder>();
        if (prefabBinder != null)
            WriteLayout(prefabBinder, layout);
    }

    private static void WriteLayout(BiofiltreHudBinder binder, HudRowLayout layout)
    {
        var so = new SerializedObject(binder);
        so.Update();
        so.FindProperty("primaryNormalizedAnchor").vector2Value = layout.primaryAnchor;
        so.FindProperty("primaryWorldOffset").vector2Value = layout.primaryOffset;
        so.FindProperty("starNormalizedAnchor").vector2Value = layout.starAnchor;
        so.FindProperty("starWorldOffset").vector2Value = layout.starOffset;
        so.FindProperty("secondaryNormalizedAnchor").vector2Value = layout.secondaryAnchor;
        so.FindProperty("secondaryWorldOffset").vector2Value = layout.secondaryOffset;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(binder);
    }
}
