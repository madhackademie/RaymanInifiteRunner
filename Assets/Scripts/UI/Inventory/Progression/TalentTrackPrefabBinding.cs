using System;
using UnityEngine;

/// <summary>
/// Liaison trackId -> prefab arbre pour l'overlay inventaire.
/// </summary>
[Serializable]
public class TalentTrackPrefabBinding
{
    [SerializeField] private string trackId = ProgressionTrackId.Commerce;
    [SerializeField] private TalentTreeLayoutRoot treePrefab;

    public string TrackId => trackId;
    public TalentTreeLayoutRoot TreePrefab => treePrefab;

    public bool IsValid => !string.IsNullOrEmpty(trackId) && treePrefab != null;
}
