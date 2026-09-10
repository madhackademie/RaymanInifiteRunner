using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BiofiltreLayoutBinder))]
public class BiofiltreLayoutBinderEditor : Editor
{
    private static readonly Vector2[] GizmoCornerBuffer = new Vector2[4];
    private static readonly string[] CornerLabels = { "SW", "SE", "NE", "NW" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var binder = (BiofiltreLayoutBinder)target;
        BiofiltreLayoutDefinition layout = binder.LayoutDefinition;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Bake grille (stratégie B)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "1) Ajuster le quad deckShapeUv sur le layout SO (4 coins UV).\n" +
            "2) Caler IbcSprite + enfant Grid (cellSize inchangé).\n" +
            "3) Bake → écrit columns/rows sur le layout (entiers, axes iso).",
            MessageType.Info);

        using (new EditorGUI.DisabledScope(layout == null))
        {
            if (GUILayout.Button("Bake columns / rows from deck shape (B)"))
                RunBake(binder, layout);
        }
    }

    private static void RunBake(BiofiltreLayoutBinder binder, BiofiltreLayoutDefinition layout)
    {
        var gridManager = binder.GetComponent<GridManager>();
        var fitter = binder.GetComponent<BiofiltreIbcSpriteFitter>();
        if (gridManager == null || fitter == null)
        {
            Debug.LogError("[BiofiltreLayoutBinder] GridManager ou BiofiltreIbcSpriteFitter manquant.", binder);
            return;
        }

        if (!layout.deckShapeUv.IsValid())
        {
            Debug.LogWarning("[BiofiltreLayoutBinder] deckShapeUv invalide — Sync depuis deckNormalized sur le SO.", binder);
            layout.deckShapeUv.SetFromNormalizedRect(layout.deckNormalized);
        }

        SpriteRenderer spriteRenderer = fitter.EditorGetSpriteRenderer();
        if (spriteRenderer == null)
        {
            Debug.LogError("[BiofiltreLayoutBinder] SpriteRenderer IbcSprite introuvable.", binder);
            return;
        }

        if (!BiofiltreDeckShapeUvUtility.TryGetShapeWorldCorners(
                layout.deckShapeUv, spriteRenderer, out Vector2[] worldCorners))
        {
            Debug.LogError("[BiofiltreLayoutBinder] Impossible de projeter le shape en monde.", binder);
            return;
        }

        SerializedObject gridSo = new SerializedObject(gridManager);
        Transform layoutTransform = gridSo.FindProperty("layoutTransform").objectReferenceValue as Transform;
        Vector2 originOffset = gridSo.FindProperty("originOffset").vector2Value;

        bool success = BiofiltreGridLayoutMath.TryBakeColumnRowCountB(
            worldCorners,
            layout.coordinateMode,
            layout.cellSize,
            true,
            originOffset,
            layoutTransform,
            layout.columns,
            layout.rows,
            out int newCols,
            out int newRows,
            out string warning,
            out float minCol,
            out float maxCol,
            out float minRow,
            out float maxRow);

        if (!success)
        {
            if (!string.IsNullOrEmpty(warning))
                Debug.LogWarning($"[BiofiltreLayoutBinder] {warning}", binder);
            return;
        }

        Undo.RecordObject(layout, "Bake biofiltre grid size");
        layout.columns = newCols;
        layout.rows = newRows;
        EditorUtility.SetDirty(layout);

        gridManager.ApplyBiofiltreLayout(layout);
        EditorUtility.SetDirty(gridManager);

        string msg = $"Bake B : {newCols}×{newRows} (cellSize={layout.cellSize}, mode={layout.coordinateMode}).";
        if (!string.IsNullOrEmpty(warning))
            Debug.LogWarning(msg + "\n" + warning, binder);
        else
            Debug.Log(msg, binder);
    }

    private void OnSceneGUI()
    {
        var binder = (BiofiltreLayoutBinder)target;
        BiofiltreLayoutDefinition layout = binder.LayoutDefinition;
        if (layout == null)
            return;

        var fitter = binder.GetComponent<BiofiltreIbcSpriteFitter>();
        SpriteRenderer sr = fitter != null ? fitter.EditorGetSpriteRenderer() : null;
        if (sr == null || sr.sprite == null)
            return;

        if (!layout.deckShapeUv.IsValid())
        {
            layout.deckShapeUv.SetFromNormalizedRect(layout.deckNormalized);
            EditorUtility.SetDirty(layout);
        }

        layout.deckShapeUv.GetCorners(GizmoCornerBuffer);
        Handles.color = new Color(0.2f, 0.85f, 1f, 1f);

        for (int i = 0; i < 4; i++)
        {
            Vector3 world = BiofiltreDeckShapeUvUtility.NormalizedRectUvToWorld(
                sr.sprite, sr.transform, GizmoCornerBuffer[i]);
            world.z = 0f;

            EditorGUI.BeginChangeCheck();
            Vector3 moved = Handles.FreeMoveHandle(
                world,
                HandleUtility.GetHandleSize(world) * 0.08f,
                Vector3.zero,
                Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(layout, "Move deck shape corner");
                if (BiofiltreDeckShapeUvUtility.TryWorldToNormalizedRectUv(
                        sr.sprite, sr.transform, moved, out Vector2 uv))
                {
                    SetCorner(ref layout.deckShapeUv, i, uv);
                    EditorUtility.SetDirty(layout);
                }
            }

            Handles.Label(world + Vector3.up * 0.15f, CornerLabels[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 a = BiofiltreDeckShapeUvUtility.NormalizedRectUvToWorld(
                sr.sprite, sr.transform, GizmoCornerBuffer[i]);
            Vector3 b = BiofiltreDeckShapeUvUtility.NormalizedRectUvToWorld(
                sr.sprite, sr.transform, GizmoCornerBuffer[(i + 1) % 4]);
            Handles.DrawLine(a, b, 2f);
        }
    }

    private static void SetCorner(ref BiofiltreDeckShapeUv shape, int index, Vector2 uv)
    {
        switch (index)
        {
            case 0: shape.southWest = uv; break;
            case 1: shape.southEast = uv; break;
            case 2: shape.northEast = uv; break;
            case 3: shape.northWest = uv; break;
        }
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawDeckShapeGizmo(BiofiltreLayoutBinder binder, GizmoType gizmoType)
    {
        if (binder.LayoutDefinition == null || !binder.LayoutDefinition.deckShapeUv.IsValid())
            return;

        var fitter = binder.GetComponent<BiofiltreIbcSpriteFitter>();
        if (fitter == null)
            return;

        SpriteRenderer sr = fitter.EditorGetSpriteRenderer();
        if (sr == null || sr.sprite == null)
            return;

        binder.LayoutDefinition.deckShapeUv.GetCorners(GizmoCornerBuffer);
        Gizmos.color = new Color(0.2f, 0.85f, 1f, 0.95f);

        for (int i = 0; i < 4; i++)
        {
            Vector2 a = BiofiltreDeckShapeUvUtility.NormalizedRectUvToWorld(
                sr.sprite, sr.transform, GizmoCornerBuffer[i]);
            Vector2 b = BiofiltreDeckShapeUvUtility.NormalizedRectUvToWorld(
                sr.sprite, sr.transform, GizmoCornerBuffer[(i + 1) % 4]);
            Gizmos.DrawLine(a, b);
        }
    }
}
