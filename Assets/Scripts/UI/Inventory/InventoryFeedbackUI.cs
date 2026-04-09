using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays a short feedback message (e.g. "Inventaire plein") for a configurable duration.
/// </summary>
public class InventoryFeedbackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI feedbackLabel;
    [SerializeField] private float displayDuration = 2f;

    private const string InventoryFullMessage = "Inventaire plein !";

    private Coroutine hideCoroutine;

    /// <summary>Shows the "Inventaire plein" message for the configured duration.</summary>
    public void ShowInventoryFull()
    {
        ShowMessage(InventoryFullMessage);
    }

    /// <summary>Shows a custom message for the configured duration.</summary>
    public void ShowMessage(string message)
    {
        if (feedbackLabel == null)
            return;

        feedbackLabel.text = message;
        feedbackLabel.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    // ── Internals ─────────────────────────────────────────────────────────────

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackLabel.gameObject.SetActive(false);
        hideCoroutine = null;
    }
}
