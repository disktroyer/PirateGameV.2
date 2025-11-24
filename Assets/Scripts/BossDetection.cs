using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHideController hide = other.GetComponent<PlayerHideController>();

            // Si está escondido → no hacer nada
            if (hide != null && hide.IsHidden)
                return;

            // De lo contrario → InstaKill
            Debug.Log(" El Boss atrapó al jugador. GAME OVER");

            PlayerDeath(other.gameObject);
        }
    }

    private void PlayerDeath(GameObject player)
    {
        // Aquí puedes:
        // - cargar una pantalla de game over
        // - reiniciar el nivel
        // - desactivar control del jugador

        player.SetActive(false);
       
        SceneManager.LoadScene("GameOver");

    }
}
