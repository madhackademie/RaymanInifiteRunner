using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup UI reutilisable pour afficher un feedback court lie aux ressources.
/// Peut servir au shop, au craft, aux upgrades ou a toute action avec cout.
/// </summary>
public class ResourceFeedbackPopupUI : MonoBehaviour
{
    private const string DefaultInsufficientResourcesMessage =
        "Vous n'avez pas assez de ressources pour cette action.";

    [Header("Bindings UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private Button closeButton;

    [Header("Behaviour")]
    [SerializeField] private string insufficientResourcesMessage = DefaultInsufficientResourcesMessage;
    [SerializeField] [Min(0f)] private float autoHideDelay = 2.5f;
    [SerializeField] private bool hideOnAwake = true;

    private Coroutine hideCoroutine;
    private Button hookedCloseButton;

    private void Awake()
    {
        HookCloseButton();

        if (hideOnAwake)
            SetVisible(false);
    }

    private void OnEnable()
    {
        HookCloseButton();
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

        SetVisible(true);
        transform.SetAsLastSibling();

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        if (autoHideDelay > 0f)
            hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    public void Hide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        SetVisible(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(autoHideDelay);
        hideCoroutine = null;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        GameObject target = root != null ? root : gameObject;
        target.SetActive(visible);
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
