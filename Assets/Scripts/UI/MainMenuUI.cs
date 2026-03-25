using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public void OnStartClicked()
    {
        SceneManager.LoadScene("FirstLvl");
        Debug.Log("Start clicked — load game scene here.");
        // SceneManager.LoadScene("GameScene");
    }

    /// <summary>Toggles the options panel visibility.</summary>
    public void OnOptionsClicked()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }
}
