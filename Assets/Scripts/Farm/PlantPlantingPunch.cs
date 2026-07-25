using System.Collections;
using UnityEngine;

/// <summary>
/// Punch d'apparition à la pose d'une graine : scale 0 → peak → repos.
/// Durées / peak réglables en Inspector (sliders). Bezy branche ce composant sur le prefab plante.
/// </summary>
public class PlantPlantingPunch : MonoBehaviour
{
    private const float DefaultPeakScaleMultiplier = 3f;
    private const float DefaultZoomInDurationSeconds = 2f;
    private const float DefaultZoomOutDurationSeconds = 2.5f;

    [Header("Punch scale")]
    [Tooltip("Multiplicateur de scale au pic (× scale de repos du prefab).")]
    [SerializeField, Range(1f, 5f)] private float peakScaleMultiplier = DefaultPeakScaleMultiplier;

    [Tooltip("Durée du zoom 0 → pic (secondes).")]
    [SerializeField, Range(0.5f, 5f)] private float zoomInDurationSeconds = DefaultZoomInDurationSeconds;

    [Tooltip("Durée du retour pic → scale normale (secondes).")]
    [SerializeField, Range(0.5f, 5f)] private float zoomOutDurationSeconds = DefaultZoomOutDurationSeconds;

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
        Vector3 peak = restLocalScale * peakScaleMultiplier;

        scaleTarget.localScale = Vector3.zero;

        yield return AnimateScale(Vector3.zero, peak, zoomIn);
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
