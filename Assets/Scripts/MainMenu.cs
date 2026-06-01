using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("Escena del juego")]
    public string gameSceneName = "Nivel1";

    private string path;

    void Start()
    {
        path = Application.persistentDataPath + "/save.json";

        // Desactiva botón Continuar si no hay partida
        GameObject bottonObj = GameObject.Find("BotónContinuar");
        if (bottonObj != null)
        {
            UnityEngine.UI.Button continuarBtn = bottonObj.GetComponent<UnityEngine.UI.Button>();
            if (continuarBtn != null)
            {
                continuarBtn.interactable = File.Exists(path);
            }
            else
            {
                Debug.LogWarning("BotónContinuar no tiene componente Button");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró GameObject: BotónContinuar");
        }

        if (CustomCursorManager.Instance != null)
            CustomCursorManager.Instance.ShowMenuCursor();
    }

    public void ContinuarPartida()
    {
        if (SaveSystem.CargarPartida())
            SceneManager.LoadScene(gameSceneName);
        else
            Debug.Log("No hay partida guardada.");
    }

    public void NuevaPartida()
    {
        SaveSystem.NuevaPartida();
        SceneManager.LoadScene(gameSceneName);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
