using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Draws the biofiltre grid as GL lines directly on the camera output.
/// Works in URP via <see cref="RenderPipelineManager.endCameraRendering"/>.
/// Attach this component to the same GameObject as <see cref="GridManager"/>.
/// </summary>
[RequireComponent(typeof(GridManager))]
public class GridLinesRenderer : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private string cameraTag = "MainCamera";

    private GridManager gridManager;
    private Material    lineMaterial;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        CreateLineMaterial();
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    private void OnDestroy()
    {
        if (lineMaterial != null)
            Destroy(lineMaterial);
    }

    // ── Rendering ─────────────────────────────────────────────────────────────

    private void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (!cam.CompareTag(cameraTag))
            return;

        DrawGrid();
    }

    private void DrawGrid()
    {
        if (gridManager == null || lineMaterial == null)
            return;

        int     cols     = gridManager.Columns;
        int     rows     = gridManager.Rows;
        Vector2 cellSize = gridManager.CellSizeWorld;
        Vector2 origin   = gridManager.WorldOrigin;

        float totalWidth  = cols * cellSize.x;
        float totalHeight = rows * cellSize.y;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        // Vertical lines (col 0 … cols inclusive)
        for (int col = 0; col <= cols; col++)
        {
            float x = origin.x + col * cellSize.x;
            GL.Vertex3(x, origin.y,               0f);
            GL.Vertex3(x, origin.y - totalHeight,  0f);
        }

        // Horizontal lines (row 0 … rows inclusive)
        for (int row = 0; row <= rows; row++)
        {
            float y = origin.y - row * cellSize.y;
            GL.Vertex3(origin.x,              y, 0f);
            GL.Vertex3(origin.x + totalWidth, y, 0f);
        }

        GL.End();
        GL.PopMatrix();
    }

    // ── Material ──────────────────────────────────────────────────────────────

    private void CreateLineMaterial()
    {
        // Unlit, vertex-colored shader — guaranteed to exist in every Unity install
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null)
        {
            Debug.LogError("[GridLinesRenderer] Could not find 'Hidden/Internal-Colored' shader.", this);
            return;
        }

        lineMaterial = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        lineMaterial.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull",      (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite",    0);
        lineMaterial.SetInt("_ZTest",     (int)UnityEngine.Rendering.CompareFunction.Always);
    }
}
