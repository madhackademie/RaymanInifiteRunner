using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu UI with Start and Options buttons.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);

        optionsPanel.SetActive(false);
    }

    /// <summary>Loads the game scene.</summary>
    public async void OnStartClicked()
    {
        if (SceneNavigator.Instance == null)
        {
            Debug.LogWarning("[MainMenuUI] SceneNavigator introuvable.");
            return;
        }

        await SceneNavigator.Instance.ShowScene(SceneId.FirstLvl);
    }

    /// <summary>Toggles the options panel visibility.</summary>
    public void OnOptionsClicked()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}
