using UnityEngine;

/// <summary>
/// Conditions et coût pour débloquer un canal via recherche (timer + monnaie).
/// </summary>
[CreateAssetMenu(fileName = "Unlock_", menuName = "Game/Sale Channels/Unlock Definition")]
public class SaleChannelUnlockDefinition : ScriptableObject
{
    [Header("Identite")]
    [SerializeField] private string channelId = SaleChannelId.Sash;
    [SerializeField] private string displayName = "Bandoulière";

    [Header("Prerequis")]
    [SerializeField] private string requiredUnlockedChannelId;
    [SerializeField] private string requiredSaleCountChannelId = SaleChannelId.Neighbor;
    [SerializeField] private int requiredSaleCount;
    [SerializeField] private int requiredItemsSold;
    [SerializeField] private int requiredWalletGold;

    [Header("Recherche")]
    [SerializeField] private int researchCostGold;
    [SerializeField] private float researchDurationHours = 2f;

    public string ChannelId => channelId;
    public string DisplayName => displayName;
    public string RequiredUnlockedChannelId => requiredUnlockedChannelId;
    public string RequiredSaleCountChannelId => requiredSaleCountChannelId;
    public int RequiredSaleCount => Mathf.Max(0, requiredSaleCount);
    public int RequiredItemsSold => Mathf.Max(0, requiredItemsSold);
    public int RequiredWalletGold => Mathf.Max(0, requiredWalletGold);
    public int ResearchCostGold => Mathf.Max(0, researchCostGold);
    public float ResearchDurationSeconds => Mathf.Max(1f, researchDurationHours * 3600f);
}
