using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseMenuUI;
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    void Start()
    {
        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.HideCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.HideCursor();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.ShowMenuCursor();
    }

    public void GuardarPartida()
    {
        SaveSystem.GuardarPartida();
        Debug.Log("✅ Partida guardada.");
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
