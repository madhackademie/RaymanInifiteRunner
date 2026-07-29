using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup UI reutilisable pour afficher un feedback court lie aux ressources.
/// Peut servir au shop, au craft, aux upgrades ou a toute action avec cout.
/// Open/Close soft via Animator Bezy (triggers Open / Close sur Panel).
/// </summary>
public class ResourceFeedbackPopupUI : MonoBehaviour
{
    private const string DefaultInsufficientResourcesMessage =
        "Vous n'avez pas assez de ressources pour cette action.";
    private const string OpenTriggerName = "Open";
    private const string CloseTriggerName = "Close";
    private const float CloseAnimDurationSeconds = 0.14f;

    private static readonly int OpenTriggerHash = Animator.StringToHash(OpenTriggerName);
    private static readonly int CloseTriggerHash = Animator.StringToHash(CloseTriggerName);

    [Header("Bindings UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Animator panelAnimator;

    [Header("Behaviour")]
    [SerializeField] private string insufficientResourcesMessage = DefaultInsufficientResourcesMessage;
    [SerializeField] [Min(0f)] private float autoHideDelay = 2.5f;
    [SerializeField] private bool hideOnAwake = true;

    private Coroutine hideCoroutine;
    private Coroutine closeAnimCoroutine;
    private Button hookedCloseButton;

    private void Awake()
    {
        HookCloseButton();
        ResolvePanelAnimatorIfNeeded();

        if (hideOnAwake)
            SetVisibleImmediate(false);
    }

    private void OnEnable()
    {
        HookCloseButton();
    }

    private void OnDisable()
    {
        StopHideCoroutine();
        StopCloseAnimCoroutine();
    }

    private void OnDestroy()
    {
        UnhookCloseButton();
    }

    public void ShowInsufficientResources()
    {
        ShowMessage(insufficientResourcesMessage);
    }

    public void ShowMessage(string message)
    {
        string resolvedMessage = string.IsNullOrWhiteSpace(message)
            ? DefaultInsufficientResourcesMessage
            : message;

        if (messageLabel != null)
            messageLabel.text = resolvedMessage;

        StopHideCoroutine();
        StopCloseAnimCoroutine();

        SetVisibleImmediate(true);
        transform.SetAsLastSibling();
        PlayOpen();

        if (autoHideDelay > 0f)
            hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    public void Hide()
    {
        StopHideCoroutine();

        if (!IsRootActive())
        {
            SetVisibleImmediate(false);
            return;
        }

        PlayClose();
        StopCloseAnimCoroutine();
        closeAnimCoroutine = StartCoroutine(HideAfterCloseAnim());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(autoHideDelay);
        hideCoroutine = null;
        Hide();
    }

    private IEnumerator HideAfterCloseAnim()
    {
        yield return new WaitForSecondsRealtime(CloseAnimDurationSeconds);
        closeAnimCoroutine = null;
        SetVisibleImmediate(false);
    }

    private void PlayOpen()
    {
        ResolvePanelAnimatorIfNeeded();
        if (panelAnimator == null)
            return;

        panelAnimator.ResetTrigger(CloseTriggerHash);
        panelAnimator.SetTrigger(OpenTriggerHash);
    }

    private void PlayClose()
    {
        ResolvePanelAnimatorIfNeeded();
        if (panelAnimator == null)
            return;

        panelAnimator.ResetTrigger(OpenTriggerHash);
        panelAnimator.SetTrigger(CloseTriggerHash);
    }

    private void SetVisibleImmediate(bool visible)
    {
        GameObject target = root != null ? root : gameObject;
        if (target.activeSelf != visible)
            target.SetActive(visible);
    }

    private bool IsRootActive()
    {
        GameObject target = root != null ? root : gameObject;
        return target.activeSelf;
    }

    private void ResolvePanelAnimatorIfNeeded()
    {
        if (panelAnimator != null)
            return;

        if (root != null)
        {
            Transform panel = root.transform.Find("Panel");
            if (panel != null)
                panelAnimator = panel.GetComponent<Animator>();
        }

        if (panelAnimator == null)
            panelAnimator = GetComponentInChildren<Animator>(true);
    }

    private void StopHideCoroutine()
    {
        if (hideCoroutine == null)
            return;

        StopCoroutine(hideCoroutine);
        hideCoroutine = null;
    }

    private void StopCloseAnimCoroutine()
    {
        if (closeAnimCoroutine == null)
            return;

        StopCoroutine(closeAnimCoroutine);
        closeAnimCoroutine = null;
    }

    private void HookCloseButton()
    {
        if (closeButton == null || hookedCloseButton == closeButton)
            return;

        UnhookCloseButton();
        closeButton.onClick.AddListener(Hide);
        hookedCloseButton = closeButton;
    }

    private void UnhookCloseButton()
    {
        if (hookedCloseButton == null)
            return;

        hookedCloseButton.onClick.RemoveListener(Hide);
        hookedCloseButton = null;
    }
}
