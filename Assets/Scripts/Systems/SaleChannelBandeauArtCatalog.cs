using System;
using UnityEngine;

/// <summary>
/// Sprites d'illustration par canal — une seule source de vérité (pas d'override prefab imbriqué).
/// </summary>
[CreateAssetMenu(fileName = "BandeauArtCatalog", menuName = "Game/Sale Channels/Bandeau Art Catalog")]
public class SaleChannelBandeauArtCatalog : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public string channelId;
        public Sprite illustrationSprite;
    }

    [SerializeField] private Entry[] entries = Array.Empty<Entry>();

    public bool TryGetIllustration(string channelId, out Sprite sprite)
    {
        sprite = null;
        if (string.IsNullOrWhiteSpace(channelId) || entries == null)
            return false;

        for (int i = 0; i < entries.Length; i++)
        {
            Entry entry = entries[i];
            if (entry.illustrationSprite == null || string.IsNullOrWhiteSpace(entry.channelId))
                continue;

            if (!string.Equals(entry.channelId, channelId, StringComparison.Ordinal))
                continue;

            sprite = entry.illustrationSprite;
            return true;
        }

        return false;
    }
}
