using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestScript : Interactable
{
    [Header("Animación")]
    public Animator animator;
    public string openAnimationTrigger = "Open";
    public string closedStateName = "Closed"; // Estado inicial cerrado
    public string requiredItemName = "llave_0";
    public string noKeyMessage = "Necesitas la llave para abrir el cofre";
    public float openAnimationDelay = 2f;

    [Header("End Game")]
    public string endGameSceneName = "VICTORY";

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
        if (inv == null || !inv.ContieneItem(requiredItemName))
        {
            Debug.Log(noKeyMessage);
            return;
        }

        // Reproducir animación solamente si el jugador tiene la llave
        if (animator != null && !string.IsNullOrEmpty(openAnimationTrigger))
        {
            animator.SetTrigger(openAnimationTrigger);
        }

        // Cambiar a escena end game después de un delay (para que termine la animación)
        Invoke("LoadEndGameScene", openAnimationDelay);
    }

    private void LoadEndGameScene()
    {
        SceneManager.LoadScene(endGameSceneName);
    }
}