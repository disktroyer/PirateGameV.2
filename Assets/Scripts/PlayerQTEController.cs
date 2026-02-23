using UnityEngine;

public class PlayerQTEController : MonoBehaviour
{
    public QTEWheelUI qteUI;
    private TentacleTrap currentTrap;

    public void StartQTE(TentacleTrap trap)
    {
        currentTrap = trap;
        qteUI.Show(this);
    }

    public void OnSuccess()
    {
        qteUI.Hide();
        currentTrap.ReleasePlayer(gameObject);
        currentTrap = null;
    }

    public void OnFail()
    {
        // Penalizacion opcional (tiempo extra atrapado)
        Debug.Log(" Fallo QTE");
    }
}
