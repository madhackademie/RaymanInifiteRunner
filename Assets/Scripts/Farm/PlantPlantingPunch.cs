using System.Collections;
using UnityEngine;

/// <summary>
/// Punch d'apparition à la pose d'une graine : scale départ → peak → repos.
/// Durées / peak / départ réglables en Inspector (sliders). Bezy branche ce composant sur le prefab plante.
/// </summary>
public class PlantPlantingPunch : MonoBehaviour
{
    private const float DefaultStartScaleMultiplier = 0.6f;
    private const float DefaultPeakScaleMultiplier = 3f;
    private const float DefaultZoomInDurationSeconds = 2f;
    private const float DefaultZoomOutDurationSeconds = 2.5f;

    [Header("Punch scale")]
    [Tooltip("Scale de départ (× scale de repos). 0 = invisible au départ ; ~0.6 = déjà visible avec les particules.")]
    [SerializeField, Range(0f, 1f)] private float startScaleMultiplier = DefaultStartScaleMultiplier;

    [Tooltip("Multiplicateur de scale au pic (× scale de repos du prefab).")]
    [SerializeField, Range(1f, 5f)] private float peakScaleMultiplier = DefaultPeakScaleMultiplier;

    [Tooltip("Durée du zoom départ → pic (secondes).")]
    [SerializeField, Range(0.05f, 5f)] private float zoomInDurationSeconds = DefaultZoomInDurationSeconds;

    [Tooltip("Durée du retour pic → scale normale (secondes).")]
    [SerializeField, Range(0.05f, 5f)] private float zoomOutDurationSeconds = DefaultZoomOutDurationSeconds;

    [Header("Cible")]
    [Tooltip("Transform animé. Si vide : ce transform (racine plante).")]
    [SerializeField] private Transform scaleTarget;

    private Vector3 restLocalScale;
    private Coroutine punchRoutine;

    private void Awake()
    {
        if (scaleTarget == null)
            scaleTarget = transform;

        restLocalScale = scaleTarget.localScale;
    }

    /// <summary>Joue le punch (placement joueur uniquement — pas à la restauration save).</summary>
    public void Play()
    {
        if (scaleTarget == null)
            scaleTarget = transform;

        if (punchRoutine != null)
            StopCoroutine(punchRoutine);

        punchRoutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        float zoomIn = Mathf.Max(0.01f, zoomInDurationSeconds);
        float zoomOut = Mathf.Max(0.01f, zoomOutDurationSeconds);
        float startMul = Mathf.Clamp01(startScaleMultiplier);
        Vector3 start = restLocalScale * startMul;
        Vector3 peak = restLocalScale * peakScaleMultiplier;

        scaleTarget.localScale = start;

        yield return AnimateScale(start, peak, zoomIn);
        yield return AnimateScale(peak, restLocalScale, zoomOut);

        scaleTarget.localScale = restLocalScale;
        punchRoutine = null;
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float durationSeconds)
    {
        float elapsed = 0f;
        while (elapsed < durationSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durationSeconds);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            scaleTarget.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }

        scaleTarget.localScale = to;
    }
}
