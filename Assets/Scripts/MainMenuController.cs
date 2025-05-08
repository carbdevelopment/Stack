using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject gameplayHUD;
    public GameManager gameManager;

    void Start()
    {
        mainMenuUI.SetActive(true);
        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);

        if (gameManager != null)
            gameManager.SetGameActive(false);
    }

    public void OnPlayButton()
    {
        mainMenuUI.SetActive(false);
        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);

        if (gameManager != null)
            gameManager.SetGameActive(true);
    }

    public void OnQuitButton() => Application.Quit();
}