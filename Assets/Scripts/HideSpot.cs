using UnityEngine;

public class HideSpot : Interactable
{
    private bool isHidden = false;
    public Transform hidePoint; // posición dentro del armario
    private bool isPlayerNearby = false;
    private GameObject player;
    public Animator ClosetAnimator;

    void Start()
    {
        ClosetAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            HidePlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.gameObject;
            ClosetAnimator = player.GetComponent<Animator>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }

    private void HidePlayer()
    {
        ClosetAnimator.SetTrigger("EnterCloset");

    }

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
            hideController.Hide(hidePoint, this);
            isHidden = true;
        }
        else
        {
            hideController.Unhide();
            isHidden = false;
        }
    }
}
