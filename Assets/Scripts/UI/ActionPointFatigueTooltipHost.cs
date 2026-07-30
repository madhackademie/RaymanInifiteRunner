using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Panneau tooltip partagé pour les zones fatigue de la barre PA.
/// Fade Bezy : triggers Animator FadeIn / FadeOut (~0.12 s).
/// </summary>
public class ActionPointFatigueTooltipHost : MonoBehaviour
{
    private const string FadeInTriggerName = "FadeIn";
    private const string FadeOutTriggerName = "FadeOut";
    private const float FadeOutDurationSeconds = 0.12f;

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private TextMeshProUGUI bodyLabel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 12f);
    [SerializeField] private Animator panelAnimator;

    private static readonly int FadeInTriggerHash = Animator.StringToHash(FadeInTriggerName);
    private static readonly int FadeOutTriggerHash = Animator.StringToHash(FadeOutTriggerName);

    private Coroutine hideRoutine;
    private bool isShown;

    private void Awake()
    {
        ResolveAnimatorIfNeeded();
        HideImmediate();
    }

    private void OnDisable()
    {
        StopHideRoutine();
        isShown = false;
    }

    public void Show(ActionPointFatigueTier tier, RectTransform anchor)
    {
        if (panelRoot == null)
            return;

        ResolveAnimatorIfNeeded();
        StopHideRoutine();

        if (titleLabel != null)
            titleLabel.text = ActionPointFatigueUiCopy.GetZoneTooltipTitle(tier);

        if (bodyLabel != null)
            bodyLabel.text = ActionPointFatigueUiCopy.GetZoneTooltipBody(tier);

        panelRoot.SetActive(true);
        PositionNearAnchor(anchor);
        isShown = true;
        PlayFadeIn();
    }

    public void Hide()
    {
        if (panelRoot == null || !panelRoot.activeSelf)
        {
            isShown = false;
            return;
        }

        isShown = false;
        PlayFadeOut();
        StopHideRoutine();
        hideRoutine = StartCoroutine(HideAfterFade());
    }

    private IEnumerator HideAfterFade()
    {
        yield return new WaitForSecondsRealtime(FadeOutDurationSeconds);
        hideRoutine = null;

        if (!isShown && panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void HideImmediate()
    {
        StopHideRoutine();
        isShown = false;
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void PlayFadeIn()
    {
        if (panelAnimator == null)
            return;

        panelAnimator.ResetTrigger(FadeOutTriggerHash);
        panelAnimator.SetTrigger(FadeInTriggerHash);
    }

    private void PlayFadeOut()
    {
        if (panelAnimator == null)
            return;

        panelAnimator.ResetTrigger(FadeInTriggerHash);
        panelAnimator.SetTrigger(FadeOutTriggerHash);
    }

    private void ResolveAnimatorIfNeeded()
    {
        if (panelAnimator != null || panelRoot == null)
            return;

        panelAnimator = panelRoot.GetComponent<Animator>();
    }

    private void StopHideRoutine()
    {
        if (hideRoutine == null)
            return;

        StopCoroutine(hideRoutine);
        hideRoutine = null;
    }

    private void PositionNearAnchor(RectTransform anchor)
    {
        if (panelRect == null || anchor == null)
            return;

        Canvas canvas = anchor.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners);
        Vector2 center = (corners[0] + corners[2]) * 0.5f;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(eventCamera, center),
                eventCamera,
                out Vector2 localPoint))
        {
            panelRect.anchoredPosition = localPoint + screenOffset;
        }
    }
}
