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
        UnityEngine.UI.Button continuarBtn = GameObject.Find("BotónContinuar").GetComponent<UnityEngine.UI.Button>();
        continuarBtn.interactable = File.Exists(path);
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
