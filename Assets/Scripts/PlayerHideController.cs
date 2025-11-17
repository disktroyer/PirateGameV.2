using Cainos.PixelArtTopDown_Basic;
using UnityEngine;

public class PlayerHideController : MonoBehaviour
{
    private bool isHidden = false;
    private Vector3 lastPositionBeforeHide;

    public bool IsHidden => isHidden;

    private TopDownCharacterController movement;
    private SpriteRenderer sprite;

    void Start()
    {
        movement = GetComponent<TopDownCharacterController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void EnterHideSpot(Transform hidePoint)
    {
        isHidden = true;

        lastPositionBeforeHide = transform.position;
        transform.position = hidePoint.position;

        // Opciones:
        movement.enabled = false;  // Jugador no se mueve
        sprite.enabled = false;    // El jugador se oculta visualmente

        Debug.Log("Jugador escondido");
    }

    public void ExitHideSpot()
    {
        isHidden = false;
        transform.position = lastPositionBeforeHide;

        movement.enabled = true;
        sprite.enabled = true;

        Debug.Log("Jugador salió del escondite");
    }
}
