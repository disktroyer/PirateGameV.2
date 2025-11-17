using UnityEngine;

public class HideSpot : Interactable
{
    private bool isHiding = false;

    [Header("Punto donde el jugador se coloca al esconderse")]
    public Transform hidePoint;

    public override void Interact(GameObject player)
    {
        PlayerHideController hideCtrl = player.GetComponent<PlayerHideController>();

        if (hideCtrl == null) return;

        if (!isHiding)
        {
            // Entrar al escondite
            hideCtrl.EnterHideSpot(hidePoint);
            isHiding = true;
        }
        else
        {
            // Salir del escondite
            hideCtrl.ExitHideSpot();
            isHiding = false;
        }
    }
}
