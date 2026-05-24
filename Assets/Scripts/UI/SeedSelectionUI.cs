using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI de selection de graines pour la plantation.
///
/// Role principal :
/// - ouvrir un panneau quand le joueur clique une cellule vide ;
/// - lister uniquement les graines effectivement possedees par le joueur ;
/// - basculer proprement entre 2 etats visuels :
///   1) inventaire vide (message + bouton Shop),
///   2) inventaire avec graines (slots interactifs) ;
/// - lancer la plantation (directe ou via preview) quand une graine est choisie.
///
/// Cette classe est volontairement "orchestrateur UI" :
/// la logique metier de plantation reste dans <see cref="BiofiltreManager"/>.
/// </summary>
public class SeedSelectionUI : MonoBehaviour
{
    // Message standard affiche quand aucune graine n'est disponible.
    private const string DefaultEmptyMessage =
        "Aucune graine dans l'inventaire. Ouvrez le Shop (barre du bas) pour en acheter.";

    /// <summary>
    /// Etat visuel global du panneau.
    /// On l'utilise pour eviter les etats hybrides (slots visibles + message empty).
    /// </summary>
    private enum SeedInventoryVisualState
    {
        // Etat initial avant premier BuildSlots.
        Unknown = 0,
        // Aucun slot plantable visible.
        Empty = 1,
        // Au moins une graine plantable visible.
        HasSeeds = 2
    }

    [Header("Panel")]
    // Racine visuelle du popup.
    [SerializeField] private GameObject panel;
    // Titre principal du popup. Assigne en prefab pour eviter les recherches implicites fragiles.
    [SerializeField] private TextMeshProUGUI titleLabel;
    // Bouton de fermeture du popup.
    [SerializeField] private Button closeButton;

    [Header("Seed slots")]
    [Tooltip("Seeds plantable when the player owns stock (seedItem required per entry).")]
    // Catalogue de graines affichables dans le popup.
    [SerializeField] private List<SeedEntry> availableSeeds = new();
    // Prefab d'une ligne de graine.
    [SerializeField] private SeedSlotUI slotPrefab;
    // Parent layout qui contient les slots instancies.
    [SerializeField] private Transform slotsContainer;

    [Header("Empty inventory")]
    // Panneau "inventaire vide". Peut etre null : fallback sur le titre.
    [SerializeField] private GameObject emptyStatePanel;
    // Label du panneau empty.
    [SerializeField] private TextMeshProUGUI emptyStateLabel;
    // Bouton "ouvrir le shop" dans l'etat empty.
    [SerializeField] private Button openShopButton;

    [Header("Placement preview")]
    // Preview de pose optionnelle (ghost de plante).
    [SerializeField] private PlantPlacementPreview placementPreview;

    // Contexte runtime de la cellule cible actuellement ouverte.
    private BiofiltreCell targetCell;
    // Manager de plantation associe a la cellule cible.
    private BiofiltreManager targetManager;
    // Cache du GridManager pour la preview/validation.
    private GridManager gridManager;
    // Inventaire joueur (injecte ou singleton fallback).
    private PlayerInventory playerInventory;

    // Indicateur externe utile pour savoir si le mode preview est actif.
    public bool IsPreviewActive => placementPreview != null && placementPreview.enabled;

    // Slots instancies a la volee, pour nettoyage/rebuild.
    private readonly List<SeedSlotUI> spawnedSlots = new();
    // Sauvegarde du titre "normal" pour le restaurer apres un empty fallback.
    private string defaultPanelTitle = string.Empty;
    // Etat visuel courant (source unique de verite pour l'affichage).
    private SeedInventoryVisualState visualState = SeedInventoryVisualState.Unknown;

    /// <summary>
    /// Initialisation UI et wiring des boutons.
    /// </summary>
    private void Awake()
    {
        // Toujours permettre la fermeture manuelle.
        closeButton.onClick.AddListener(Close);

        // Le bouton shop peut etre absent selon le prefab.
        if (openShopButton != null)
            openShopButton.onClick.AddListener(HandleOpenShopClicked);

        // Au boot, l'empty panel est cache.
        if (emptyStatePanel != null)
            emptyStatePanel.SetActive(false);

        // On memorise le titre par defaut avant toute mutation runtime.
        CacheDefaultPanelTitle();
        visualState = SeedInventoryVisualState.Unknown;

        // Le popup demarre ferme.
        panel.SetActive(false);
    }

    /// <summary>
    /// Nettoyage des subscriptions pour eviter les callbacks sur objet detruit.
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeInventory();
    }

    /// <summary>
    /// Injection externe de la preview (utile quand creee/routee ailleurs).
    /// </summary>
    /// <param name="preview">Composant de preview de plantation.</param>
    public void InjectPlacementPreview(PlantPlacementPreview preview)
    {
        if (preview != null)
            placementPreview = preview;
    }

    /// <summary>
    /// Injection explicite de l'inventaire joueur + rebind evenementiel.
    /// </summary>
    /// <param name="inventory">Inventaire a observer.</param>
    public void InjectPlayerInventory(PlayerInventory inventory)
    {
        UnsubscribeInventory();
        playerInventory = inventory;
        SubscribeInventory();
    }

    /// <summary>
    /// Ouvre le popup sur une cellule cible et reconstruit la liste.
    /// </summary>
    /// <param name="cell">Cellule cliquee.</param>
    /// <param name="manager">Manager biofiltre actif.</param>
    public void Open(BiofiltreCell cell, BiofiltreManager manager)
    {
        targetCell = cell;
        targetManager = manager;
        gridManager = manager.GetComponent<GridManager>();

        // Fallback singleton si l'inventaire n'a pas ete injecte.
        if (playerInventory == null)
            playerInventory = PlayerInventory.Instance;

        // Toujours reconstruire pour refleter le stock le plus recent.
        BuildSlots();
        panel.SetActive(true);
    }

    /// <summary>
    /// Ferme le popup et purge le contexte de cible.
    /// </summary>
    public void Close()
    {
        panel.SetActive(false);
        targetCell = null;
        targetManager = null;
        gridManager = null;
    }

    /// <summary>
    /// Abonnement aux changements d'inventaire (refresh live du popup).
    /// </summary>
    private void SubscribeInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
    }

    /// <summary>
    /// Desabonnement securise.
    /// </summary>
    private void UnsubscribeInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    /// <summary>
    /// Callback inventaire : si le popup est ouvert, on recalcule les slots.
    /// </summary>
    private void HandleInventoryChanged()
    {
        // Pas besoin de rebuild si le panneau est ferme.
        if (!panel.activeSelf)
            return;

        BuildSlots();
    }

    /// <summary>
    /// Reconstruit totalement la liste des graines affichables.
    ///
    /// Logique :
    /// 1) supprime les anciens slots ;
    /// 2) garde uniquement les SeedEntry valides + stock > 0 ;
    /// 3) configure l'interactabilite selon la place disponible dans la grille ;
    /// 4) applique l'etat visuel global (HasSeeds / Empty).
    /// </summary>
    private void BuildSlots()
    {
        ClearSpawnedSlots();

        if (playerInventory == null)
        {
            SetVisualState(SeedInventoryVisualState.Empty, DefaultEmptyMessage);
            return;
        }

        int visibleCount = 0;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (!IsEntryPlantable(entry, out int stock))
                continue;

            SeedSlotUI slot = Instantiate(slotPrefab, slotsContainer);
            slot.Bind(entry, stock);

            bool fits = targetManager != null &&
                        targetCell != null &&
                        targetManager.CanPlace(targetCell.GridCoordinates, entry.plantDefinition);
            // Le bouton de slot n'est cliquable que si la plante rentre ET si stock > 0.
            slot.SetInteractable(fits && stock > 0);

            slot.OnSlotClicked += HandleSeedSelected;
            spawnedSlots.Add(slot);
            visibleCount++;
        }

        if (visibleCount == 0)
            SetVisualState(SeedInventoryVisualState.Empty, DefaultEmptyMessage);
        else
            SetVisualState(SeedInventoryVisualState.HasSeeds);
    }

    /// <summary>
    /// Verifie qu'une entree est exploitable pour l'affichage/plantation.
    /// </summary>
    /// <param name="entry">Entree catalogue de graine.</param>
    /// <param name="stock">Stock calcule dans l'inventaire.</param>
    /// <returns>True si toutes les refs sont valides et stock > 0.</returns>
    private bool IsEntryPlantable(SeedEntry entry, out int stock)
    {
        stock = 0;

        // On refuse toute entree incomplète pour eviter des erreurs runtime.
        if (entry == null || entry.plantDefinition == null || entry.plantPrefab == null || entry.seedItem == null)
            return false;

        stock = playerInventory.Count(entry.seedItem);
        return stock > 0;
    }

    /// <summary>
    /// Point central de bascule visuelle entre "inventaire vide" et "graines disponibles".
    ///
    /// Ce switch est volontairement centralise pour eviter les regressions de synchro UI
    /// (ex: message empty persistant alors que des slots sont visibles).
    /// </summary>
    /// <param name="nextState">Etat cible.</param>
    /// <param name="emptyMessage">Message custom optionnel pour l'etat vide.</param>
    private void SetVisualState(SeedInventoryVisualState nextState, string emptyMessage = null)
    {
        // Optimisation: si on est deja dans un etat non-empty identique, inutile de retraiter.
        // Pour Empty on repasse quand meme afin de refresher le message.
        if (visualState == nextState && nextState != SeedInventoryVisualState.Empty)
            return;

        visualState = nextState;
        bool hasSeeds = nextState == SeedInventoryVisualState.HasSeeds;

        // Les slots sont visibles uniquement en etat HasSeeds.
        if (slotsContainer != null)
            slotsContainer.gameObject.SetActive(hasSeeds);

        // Chemin principal: panneau empty dedie configure dans le prefab.
        if (emptyStatePanel != null)
        {
            emptyStatePanel.SetActive(!hasSeeds);
            if (!hasSeeds && emptyStateLabel != null)
                emptyStateLabel.text = string.IsNullOrEmpty(emptyMessage) ? DefaultEmptyMessage : emptyMessage;

            // Le bouton shop n'a de sens qu'en mode empty.
            if (openShopButton != null)
                openShopButton.gameObject.SetActive(!hasSeeds);

            // Quand on revient en mode HasSeeds, on restaure le titre normal.
            if (hasSeeds)
                RestoreDefaultPanelTitle();

            return;
        }

        // Fallback legacy: pas de panneau empty -> on manipule le titre du panel.
        if (hasSeeds)
        {
            RestoreDefaultPanelTitle();
            return;
        }

        string message = string.IsNullOrEmpty(emptyMessage) ? DefaultEmptyMessage : emptyMessage;
        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
        {
            if (fallbackLabel.text != message)
                defaultPanelTitle = fallbackLabel.text;

            fallbackLabel.text = message;
        }
    }

    /// <summary>
    /// Sauvegarde le texte titre "nominal" du panel.
    /// </summary>
    private void CacheDefaultPanelTitle()
    {
        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
            defaultPanelTitle = fallbackLabel.text;
    }

    /// <summary>
    /// Resolve le label titre utilise en fallback.
    ///
    /// Priorite :
    /// 1) reference explicite `titleLabel` (prefab),
    /// 2) premier TMP direct enfant du panel,
    /// 3) sinon premier TMP trouve dans le panel.
    /// </summary>
    /// <returns>Le label titre resolu, ou null si introuvable.</returns>
    private TextMeshProUGUI ResolveFallbackTitleLabel()
    {
        if (titleLabel != null)
            return titleLabel;

        if (panel == null)
            return null;

        TextMeshProUGUI[] labels = panel.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI label in labels)
        {
            if (label != null && label.transform.parent == panel.transform)
            {
                titleLabel = label;
                return titleLabel;
            }
        }

        if (labels.Length > 0)
            titleLabel = labels[0];

        return titleLabel;
    }

    /// <summary>
    /// Restaure le titre original du panel apres un passage par l'etat empty fallback.
    /// </summary>
    private void RestoreDefaultPanelTitle()
    {
        if (string.IsNullOrEmpty(defaultPanelTitle))
            return;

        TextMeshProUGUI fallbackLabel = ResolveFallbackTitleLabel();
        if (fallbackLabel != null)
            fallbackLabel.text = defaultPanelTitle;
    }

    /// <summary>
    /// Detruit les slots instancies precedemment et retire leurs listeners.
    /// </summary>
    private void ClearSpawnedSlots()
    {
        foreach (SeedSlotUI slot in spawnedSlots)
        {
            slot.OnSlotClicked -= HandleSeedSelected;
            Destroy(slot.gameObject);
        }

        spawnedSlots.Clear();
    }

    /// <summary>
    /// Handler du clic sur un slot de graine.
    ///
    /// Comportement :
    /// - valide le contexte/capacite de stock,
    /// - ferme le popup,
    /// - puis lance plantation directe OU preview selon la config.
    /// </summary>
    /// <param name="entry">Graine selectionnee.</param>
    private void HandleSeedSelected(SeedEntry entry)
    {
        if (targetCell == null || targetManager == null || entry == null)
            return;

        // Si le stock a change entre-temps, on refresh juste l'UI.
        if (playerInventory == null || playerInventory.Count(entry.seedItem) <= 0)
        {
            BuildSlots();
            return;
        }

        BiofiltreCell cell = targetCell;
        BiofiltreManager manager = targetManager;
        GridManager grid = gridManager;

        Close();

        // Mode sans preview: plantation immediate.
        if (placementPreview == null)
        {
            if (!manager.TryPlantSeedAt(cell.GridCoordinates, entry.plantDefinition, entry.plantPrefab, entry.seedItem))
                Debug.LogWarning("[SeedSelectionUI] Plantation impossible (stock ou emplacement).", this);
            return;
        }

        // Mode preview: la consommation finale se fera lors de la confirmation preview.
        placementPreview.Begin(
            entry.plantDefinition,
            entry.plantPrefab,
            entry.seedItem,
            cell,
            grid,
            manager);
    }

    /// <summary>
    /// Ouvre l'ecran Shop depuis l'empty state.
    /// </summary>
    private void HandleOpenShopClicked()
    {
        // On ferme localement et on masque la popup hote ferme si necessaire.
        Close();
        targetManager?.HideFarmSeedSelectionPopup();

        // Flux normal: passer par UIManager.
        if (UIManager.Instance != null && UIManager.Instance.TryShowScreen(ScreenId.Shop))
            return;

        // Warning explicite si le routing UI n'est pas disponible.
        Debug.LogWarning("[SeedSelectionUI] Impossible d'ouvrir le shop (UIManager).", this);
    }

    /// <summary>
    /// Utilitaire de resolution prefab a partir d'une PlantDefinition.
    /// </summary>
    /// <param name="definition">Definition de plante cible.</param>
    /// <param name="prefab">Prefab resolu si trouve.</param>
    /// <returns>True si une correspondance est trouvee.</returns>
    public bool TryGetPlantPrefab(PlantDefinition definition, out GameObject prefab)
    {
        prefab = null;
        if (definition == null)
            return false;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (entry.plantDefinition == definition && entry.plantPrefab != null)
            {
                prefab = entry.plantPrefab;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Utilitaire de resolution PlantDefinition via son plantId.
    /// </summary>
    /// <param name="plantId">Identifiant data-driven de la plante.</param>
    /// <param name="definition">Definition resolue si trouvee.</param>
    /// <returns>True si l'ID est connu dans availableSeeds.</returns>
    public bool TryGetPlantDefinitionById(string plantId, out PlantDefinition definition)
    {
        definition = null;
        if (string.IsNullOrEmpty(plantId))
            return false;

        foreach (SeedEntry entry in availableSeeds)
        {
            if (entry.plantDefinition != null && entry.plantDefinition.plantId == plantId)
            {
                definition = entry.plantDefinition;
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// Donnees d'une entree "graine plantable" exposee dans l'inspector.
///
/// - plantDefinition : data de croissance/recolte.
/// - plantPrefab     : prefab monde instancie lors de la plantation.
/// - seedItem        : item inventaire consomme au moment de planter.
/// </summary>
[Serializable]
public class SeedEntry
{
    public PlantDefinition plantDefinition;
    public GameObject plantPrefab;
    public ItemDefinition seedItem;
}
