// using UnityEngine;

// public class StaticTrap : MonoBehaviour, IInteractable
// {
//     public string requiredItem = "Veneno";
//     public int damage = 1;
//     public bool isSet = false;

//     public void Interact(GameObject actor)
//     {
//         if (isSet) return;
//         InventoryManager inv = actor.GetComponent<InventoryManager>();
//         if (inv != null && inv.HasItem(requiredItem))
//         {
//             isSet = true;
//             Debug.Log("Trampa lista: " + requiredItem);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!isSet) return;
//         BossController boss = other.GetComponent<BossController>();
//         if (boss != null)
//             boss.Trap_Stun(3f);
//     }
// }
