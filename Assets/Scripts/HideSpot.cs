using UnityEngine;

public class HideSpot : Interactable
{
    private bool isHidden = false;
    public Transform hidePoint; // posición dentro del armario

    public override void Interact(GameObject player)
    {
        PlayerHideController hideController = player.GetComponent<PlayerHideController>();
        if (hideController == null)
        {
            Debug.LogWarning("El jugador no tiene PlayerHideController.");
            return;
        }

        if (!isHidden)
        {
            hideController.Hide(hidePoint);
            isHidden = true;
        }
        else
        {
            hideController.Unhide();
            isHidden = false;
        }
    }
}
