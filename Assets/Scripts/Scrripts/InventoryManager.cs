// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class InventoryManager : MonoBehaviour
// {
//     public Image slot1Image;
//     public Image slot2Image;
//     public TextMeshProUGUI mensajeTMP;

//     private ItemData[] items = new ItemData[2];

//     public bool AddItem(ItemData newItem)
//     {
//         for (int i = 0; i < items.Length; i++)
//         {
//             if (items[i] == null)
//             {
//                 items[i] = newItem;
//                 UpdateUI();
//                 ShowMessage("Recogiste " + newItem.itemName);
//                 return true;
//             }
//         }
//         ShowMessage("Inventario lleno");
//         return false;
//     }

//     public void DropFirstItem()
//     {
//         if (items[0] != null)
//         {
//             ShowMessage("Soltaste " + items[0].itemName);
//             items[0] = items[1];
//             items[1] = null;
//             UpdateUI();
//         }
//     }

//     void UpdateUI()
//     {
//         slot1Image.sprite = items[0] ? items[0].icon : null;
//         slot1Image.color = items[0] ? Color.white : new Color(1,1,1,0.3f);
//         slot2Image.sprite = items[1] ? items[1].icon : null;
//         slot2Image.color = items[1] ? Color.white : new Color(1,1,1,0.3f);
//     }

//     public bool HasItem(string name)
//     {
//         foreach (var i in items)
//             if (i != null && i.itemName == name) return true;
//         return false;
//     }

//     void ShowMessage(string m)
//     {
//         if (mensajeTMP != null)
//         {
//             mensajeTMP.text = m;
//             CancelInvoke(nameof(ClearMsg));
//             Invoke(nameof(ClearMsg), 2f);
//         }
//     }
//     void ClearMsg() => mensajeTMP.text = "";
// }

