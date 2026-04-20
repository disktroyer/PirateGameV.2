using UnityEngine;

public class CuchilloActivator : Interactable
{
    public override void Interact(GameObject player)
    {
        // Buscar la trampa de lámpara en la escena y activarla
        TrampaLampara trampa = FindObjectOfType<TrampaLampara>();
        if (trampa != null)
        {
            trampa.ActivarTrampaManual();
            Debug.Log("Trampa de lámpara activada por el cuchillo");
        }
        else
        {
            Debug.LogWarning("No se encontró la trampa de lámpara en la escena");
        }

        // Opcional: destruir el cuchillo después de usarlo
        Destroy(gameObject);
    }
}