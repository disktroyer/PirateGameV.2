using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestScript : Interactable
{
    [Header("Animación")]
    public Animator animator;
    public string openAnimationTrigger = "Open";
    public string closedStateName = "Closed"; // Estado inicial cerrado

    [Header("End Game")]
    public string endGameSceneName = "EndGame";

    void Start()
    {
        // Asegurarse de que esté en estado cerrado al inicio
        if (animator != null && !string.IsNullOrEmpty(closedStateName))
        {
            animator.Play(closedStateName);
        }
    }

    public override void Interact(GameObject actor)
    {
        var inv = actor.GetComponent<InventoryManager>();
        if (inv == null || !inv.ContieneItem("llave"))
        {
            Debug.Log("Necesitas la llave para abrir el cofre");
            return;
        }

        // Reproducir animación
        if (animator != null && !string.IsNullOrEmpty(openAnimationTrigger))
        {
            animator.SetTrigger(openAnimationTrigger);
        }

        // Cambiar a escena end game después de un delay (para que termine la animación)
        Invoke("LoadEndGameScene", 2f); // Ajusta el tiempo según la duración de la animación
    }

    private void LoadEndGameScene()
    {
        SceneManager.LoadScene(endGameSceneName);
    }
}