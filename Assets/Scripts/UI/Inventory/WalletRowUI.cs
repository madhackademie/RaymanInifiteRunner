using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One row inside the WalletWidget expanded panel.
/// Displays an item icon and its current balance.
/// </summary>
public class WalletRowUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI balanceLabel;

    /// <summary>Populates the row with the given item and balance.</summary>
    public void Bind(ItemDefinition item, int balance)
    {
        if (item == null)
            return;

        if (icon != null && item.Icon != null)
            icon.sprite = item.Icon;

        if (balanceLabel != null)
            balanceLabel.text = balance.ToString("N0");
    }
}
