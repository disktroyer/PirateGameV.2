using UnityEngine;

public class PlayerQTEController : MonoBehaviour
{
    public QTEWheelUI qteUI;
    private TrampaTentaculo currentTrap;

    public void StartQTE(TrampaTentaculo trap)
    {
        currentTrap = trap;
        qteUI.Show(this);
    }

    public void OnSuccess()
    {
        qteUI.Hide();

        if (currentTrap != null)
        {
            currentTrap.ReleasePlayer(gameObject);
            currentTrap = null;
        }
    }

    public void OnFail()
    {
        // sonido / reintento lo maneja la UI
        Debug.Log("Fallo QTE");
    }
}       