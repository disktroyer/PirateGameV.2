// using UnityEngine;

// public class PlayerInteraction : MonoBehaviour
// {
//     public float interactRange = 1.5f;
//     public KeyCode interactKey = KeyCode.E;

//     void Update()
//     {
//         if (Input.GetKeyDown(interactKey))
//             TryInteract();
//     }

//     void TryInteract()
//     {
//         Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);
//         foreach (var hit in hits)
//         {
//             IInteractable interactable = hit.GetComponent<IInteractable>();
//             if (interactable != null)
//             {
//                 interactable.Interact(gameObject);
//                 return;
//             }
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireSphere(transform.position, interactRange);
//     }
// }

