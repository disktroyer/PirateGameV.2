// using UnityEngine;
// using System.Collections;

// public class PlayerSpawnManager : MonoBehaviour
// {
//     [Header("Animator")]
//     public Animator animator;
//     public string spawnBool = "HasSpawnFinished"; 

//     [Header("Control Scripts")]
//     public MonoBehaviour movementScript;
//     public MonoBehaviour interactionScript;
//     public MonoBehaviour inventoryScript;
//     public MonoBehaviour craftingScript;
//     public MonoBehaviour qteScript;

//     void Start()
//     {
//         // ❌ Desactivamos todo
//         DisableAll();

//         // ✔ Dejamos la animación de spawn
//         if (animator != null)
//         {
//             animator.SetBool(spawnBool, false);

//             // Opcional: iniciar el estado de Spawn
//             animator.Play("Spawn");
//         }
//     }

//     void Update()
//     {
//         if (animator == null) return;

//         // Si el animator ya ha puesto HasSpawnFinished = true
//         if (animator.GetBool(spawnBool))
//         {
//             // Entonces activamos todo
//             EnableAll();

//             // Una vez activado, desactivamos este script para no volver a entrar
//             this.enabled = false;
//         }
//     }

//     void DisableAll()
//     {
//         if (movementScript != null) movementScript.enabled = false;
//         if (interactionScript != null) interactionScript.enabled = false;
//         if (inventoryScript != null) inventoryScript.enabled = false;
//         if (craftingScript != null) craftingScript.enabled = false;
//         if (qteScript != null) qteScript.enabled = false;
//     }

//     void EnableAll()
//     {
//         if (movementScript != null) movementScript.enabled = true;
//         if (interactionScript != null) interactionScript.enabled = true;
//         if (inventoryScript != null) inventoryScript.enabled = true;
//         if (craftingScript != null) craftingScript.enabled = true;
//         if (qteScript != null) qteScript.enabled = true;
//     }

//     public void OnSpawnComplete()
// {
//     animator.SetBool("HasSpawnFinished", true);
// }

// }
