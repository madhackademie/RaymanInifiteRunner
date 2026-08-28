using UnityEditor;
using UnityEngine;

/// <summary>
/// Scene gizmo preview for bed scale tuning (no SpriteRenderer in edit mode).
/// </summary>
public static class BiofiltreBedScenePreview
{
    private static Material previewMaterial;

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Active)]
    private static void DrawBedGizmo(BiofiltreGridVisualizer visualizer, GizmoType gizmoType)
    {
        if (visualizer == null || Application.isPlaying)
            return;

        // Gizmo = seule preview en édition. Si BedSprite traîne, ne pas superposer un second rendu.
        if (HasLeftoverBedSprite(visualizer.transform))
            return;

        if (!visualizer.TryGetBedWorldTransform(out Sprite sprite, out Vector3 position, out float scale))
            return;

        DrawSprite(sprite, position, scale);
    }

    private static void DrawSprite(Sprite sprite, Vector3 position, float scale)
    {
        if (sprite == null || sprite.texture == null)
            return;

        Material material = GetPreviewMaterial();
        material.mainTexture = sprite.texture;

        Mesh mesh = SpriteToMesh(sprite);
        Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * scale);
        material.SetPass(0);
        Graphics.DrawMeshNow(mesh, matrix);
        Object.DestroyImmediate(mesh);
    }

    private static Material GetPreviewMaterial()
    {
        if (previewMaterial != null)
            return previewMaterial;

        Shader shader = Shader.Find("Sprites/Default");
        previewMaterial = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        return previewMaterial;
    }

    private static Mesh SpriteToMesh(Sprite sprite)
    {
        Vector2[] srcVerts = sprite.vertices;
        Vector3[] verts = new Vector3[srcVerts.Length];
        for (int i = 0; i < srcVerts.Length; i++)
            verts[i] = srcVerts[i];

        ushort[] srcTris = sprite.triangles;
        int[] tris = new int[srcTris.Length];
        for (int i = 0; i < srcTris.Length; i++)
            tris[i] = srcTris[i];

        Mesh mesh = new Mesh { hideFlags = HideFlags.HideAndDontSave };
        mesh.vertices = verts;
        mesh.uv = sprite.uv;
        mesh.triangles = tris;
        return mesh;
    }

    private static bool HasLeftoverBedSprite(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).name == "BedSprite")
                return true;
        }

        return false;
    }
}
