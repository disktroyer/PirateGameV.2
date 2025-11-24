using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public string gameScene = "Nivel1";  // tu escena del nivel

    public void Retry()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
