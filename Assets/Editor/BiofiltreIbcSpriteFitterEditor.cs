using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector du fitter + poignées Scene cyan pour la grille seulement.
/// Le sprite IBC se règle à la main (enfant IbcSprite).
/// </summary>
[CustomEditor(typeof(BiofiltreIbcSpriteFitter))]
public class BiofiltreIbcSpriteFitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox(
            "Sprite : sélectionne l'enfant IbcSprite, outils Move / Rotate / Scale.\n" +
            "Grille : sélectionne Biofiltre (pas IbcSprite), outil Move.\n" +
            "• Point cyan à droite = taille grille\n" +
            "• Point cyan au centre = déplacer la grille\n" +
            "Le Play ne recalcule plus le sprite.",
            MessageType.Info);

        if (GUILayout.Button("Fit sprite to grid (once)"))
        {
            var fitter = (BiofiltreIbcSpriteFitter)target;
            Undo.RecordObject(fitter, "Fit biofiltre sprite");
            fitter.FitToGrid();
            EditorUtility.SetDirty(fitter);
        }
    }
}

[InitializeOnLoad]
internal static class BiofiltreLayoutSceneHandles
{
    private static readonly Color GridColor = new(0.15f, 0.95f, 1f, 1f);
    private const float MinCellSize = 0.05f;
    private const float HandleScale = 0.28f;

    static BiofiltreLayoutSceneHandles()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!TryGetGridForHandles(out GridManager grid))
            return;

        grid.RebuildMapperFromInspector();
        Rect gridRect = grid.GetWorldRect();
        DrawDiamond(gridRect, GridColor);
        Handles.Label((Vector3)gridRect.center + Vector3.up * 0.2f, "Grille");

        bool changed = DrawAxisScaleDot(gridRect, out float gridRatio);
        if (changed)
            ApplyCellSize(grid, gridRatio);

        changed = DrawMoveDot(grid, gridRect);
        if (changed || GUI.changed)
            RefreshGridVisual(grid);
    }

    private static bool TryGetGridForHandles(out GridManager grid)
    {
        grid = null;
        GameObject go = Selection.activeGameObject;
        if (go == null)
            return false;

        // Laisse les outils Unity sur IbcSprite (pas de gizmos qui volent le drag).
        if (go.GetComponent<SpriteRenderer>() != null
            && go.GetComponentInParent<BiofiltreIbcSpriteFitter>() != null
            && go.GetComponent<BiofiltreIbcSpriteFitter>() == null)
            return false;

        grid = go.GetComponent<GridManager>() ?? go.GetComponentInParent<GridManager>();
        return grid != null && go.GetComponentInParent<BiofiltreIbcSpriteFitter>() != null;
    }

    private static bool DrawAxisScaleDot(Rect rect, out float ratio)
    {
        ratio = 1f;
        Vector3 east = new(rect.xMax, rect.center.y, 0f);
        float size = HandleUtility.GetHandleSize(east) * HandleScale;
        Handles.color = GridColor;
        Handles.DrawSolidDisc(east, Vector3.forward, size * 0.35f);

        EditorGUI.BeginChangeCheck();
        Vector3 dragged = Handles.Slider2D(
            east, Vector3.forward, Vector3.right, Vector3.up, size, Handles.DotHandleCap, Vector2.zero);
        if (!EditorGUI.EndChangeCheck())
            return false;

        float oldHalf = rect.width * 0.5f;
        float newHalf = Mathf.Abs(dragged.x - rect.center.x);
        ratio = newHalf / Mathf.Max(0.01f, oldHalf);
        GUI.changed = true;
        return true;
    }

    private static bool DrawMoveDot(GridManager grid, Rect gridRect)
    {
        Vector3 center = gridRect.center;
        float size = HandleUtility.GetHandleSize(center) * HandleScale;
        Handles.color = GridColor;
        Handles.DrawSolidDisc(center, Vector3.forward, size * 0.4f);

        EditorGUI.BeginChangeCheck();
        Vector3 dragged = Handles.Slider2D(
            center, Vector3.forward, Vector3.right, Vector3.up, size, Handles.DotHandleCap, Vector2.zero);
        if (!EditorGUI.EndChangeCheck())
            return false;

        var so = new SerializedObject(grid);
        so.Update();
        SerializedProperty shiftProp = so.FindProperty("originShiftCells");
        Undo.RecordObject(grid, "Move biofiltre grid");
        Vector2 delta = (Vector2)dragged - (Vector2)center;
        float cellW = gridRect.width / Mathf.Max(1, grid.Columns);
        float cellH = gridRect.height / Mathf.Max(1, grid.Rows);
        Vector2 shift = shiftProp.vector2Value;
        shift.x += delta.x / Mathf.Max(0.01f, cellW);
        shift.y += delta.y / Mathf.Max(0.01f, cellH);
        shiftProp.vector2Value = shift;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(grid);
        GUI.changed = true;
        return true;
    }

    private static void ApplyCellSize(GridManager grid, float ratio)
    {
        var so = new SerializedObject(grid);
        so.Update();
        SerializedProperty cellProp = so.FindProperty("instanceCellSize");
        Undo.RecordObject(grid, "Resize biofiltre grid");
        cellProp.floatValue = Mathf.Max(MinCellSize, cellProp.floatValue * ratio);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(grid);
    }

    private static void RefreshGridVisual(GridManager grid)
    {
        grid.RebuildMapperFromInspector();
        if (!Application.isPlaying)
            return;

        BiofiltreGridVisualizer viz = grid.GetComponent<BiofiltreGridVisualizer>();
        viz?.GenerateGrid();
        grid.GetComponent<BiofiltreHudBinder>()?.RecalculateHudPositions();
    }

    private static void DrawDiamond(Rect aabb, Color color)
    {
        Vector2 c = aabb.center;
        Vector3[] corners =
        {
            new(c.x, aabb.yMax, 0f),
            new(aabb.xMax, c.y, 0f),
            new(c.x, aabb.yMin, 0f),
            new(aabb.xMin, c.y, 0f),
            new(c.x, aabb.yMax, 0f),
        };
        Handles.color = color;
        Handles.DrawAAPolyLine(4f, corners);
    }
}
