// using UnityEngine;

// public class TentacleTrap : MonoBehaviour
// {
//     private bool activated = false;

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (activated) return;

//         if (other.CompareTag("Player"))
//         {
//             activated = true;

//             PlayerController player = other.GetComponent<PlayerController>();
//             PlayerQTEController qte = other.GetComponent<PlayerQTEController>();

//             if (player != null && qte != null)
//             {
//                 player.SetMovement(false);
//                 qte.StartQTE(this);
//             }
//         }
//     }

//     public void ReleasePlayer(GameObject playerObj)
//     {
//         PlayerController player = playerObj.GetComponent<PlayerController>();

//         if (player != null)
//             player.SetMovement(true);

//         Destroy(gameObject);
//     }
// }
