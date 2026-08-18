using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barre d'onglets inventaire (Graines / Consommables / Récoltes / Tout).
/// Bezy câble les Button ; Cursor branche les listeners au runtime.
/// </summary>
[RequireComponent(typeof(LayoutElement))]
public class InventoryFilterTabBar : MonoBehaviour
{
    private const float BarPreferredHeight = 44f;
    private const float BarMinHeight = 40f;
    private const float TabPreferredHeight = 36f;

    public enum TabId
    {
        Seeds,
        Consumables,
        Harvests,
        All
    }

    [Header("Tabs (Bezy wiring)")]
    [SerializeField] private Button tabSeeds;
    [SerializeField] private Button tabConsumables;
    [SerializeField] private Button tabHarvests;
    [SerializeField] private Button tabAll;

    /// <summary>Onglet sélectionné ; défaut = premier onglet jeu (Graines).</summary>
    public TabId ActiveTab { get; private set; } = TabId.Seeds;

    public event Action<TabId> TabChanged;

    private void Awake()
    {
        ApplyCompactLayout();
        WireTab(tabSeeds, TabId.Seeds);
        WireTab(tabConsumables, TabId.Consumables);
        WireTab(tabHarvests, TabId.Harvests);
        WireTab(tabAll, TabId.All);
    }

    /// <summary>Force une barre compacte (évite l'étirement vertical du layout).</summary>
    private void ApplyCompactLayout()
    {
        LayoutElement barLayout = GetComponent<LayoutElement>();
        barLayout.minHeight = BarMinHeight;
        barLayout.preferredHeight = BarPreferredHeight;
        barLayout.flexibleHeight = 0f;

        if (TryGetComponent(out HorizontalLayoutGroup horizontalLayout))
        {
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.padding.top = 4;
            horizontalLayout.padding.bottom = 4;
        }

        ApplyTabHeight(tabSeeds);
        ApplyTabHeight(tabConsumables);
        ApplyTabHeight(tabHarvests);
        ApplyTabHeight(tabAll);

        RectTransform parent = transform.parent as RectTransform;
        if (parent != null)
            LayoutRebuilder.MarkLayoutForRebuild(parent);
    }

    private static void ApplyTabHeight(Button tabButton)
    {
        if (tabButton == null)
            return;

        LayoutElement tabLayout = tabButton.GetComponent<LayoutElement>();
        if (tabLayout == null)
            tabLayout = tabButton.gameObject.AddComponent<LayoutElement>();

        tabLayout.minHeight = TabPreferredHeight;
        tabLayout.preferredHeight = TabPreferredHeight;
        tabLayout.flexibleHeight = 0f;
    }

    /// <summary>Sélection programmatique (ex. reset à l'ouverture écran).</summary>
    public void SelectTab(TabId tabId, bool notify = true)
    {
        ActiveTab = tabId;
        if (notify)
            TabChanged?.Invoke(tabId);
    }

    private void WireTab(Button button, TabId tabId)
    {
        if (button == null)
            return;

        button.onClick.AddListener(() => SelectTab(tabId));
    }
}
