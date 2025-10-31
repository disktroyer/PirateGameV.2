using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Slots")]
    public Image slot1Image;
    public Image slot2Image;
    public TextMeshProUGUI mensajeTMP;

    [Header("Configuración")]
    public float mensajeDuracion = 2f;
    public int maxItems = 2;

    //private ItemData[] items = new ItemData[2];
    //private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();
    private InventoryItem[] items;

    void Start()
    {
        items = new InventoryItem[2];
        ActualizarInventario();
    }

    // Agregar ítem al inventario
    public bool AddItem(ItemData newItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventoryItem(newItem);
                ActualizarInventario();
                MostrarMensaje($"Recogiste {newItem.itemName}");
                return true;
            }
        }

        MostrarMensaje("Inventario lleno");
        return false;
    }

    // Soltar ítem
    public void RemoveItem(int index, int amount = 1)
    {
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            MostrarMensaje($"Soltaste {items[index].item.itemName}");

            //// Instanciar objeto en el mundo
            //if (items[index].prefab != null)
            //{
            //    Instantiate(items[index].prefab, transform.position + Vector3.right, Quaternion.identity);
            //}

            items[index].quantity -= amount;
            if (items[index].quantity <= 0)
            {
                items[index] = null;
            }
            ActualizarInventario();
        }
    }

    // Actualizar imágenes de los slots
    void ActualizarInventario()
    {
        slot1Image.sprite = items[0] != null ? items[0].item.icon : null;
        slot1Image.color = items[0] != null ? Color.white : new Color(1, 1, 1, 0.3f);

        slot2Image.sprite = items[1] != null ? items[1].item.icon : null;
        slot2Image.color = items[1] != null ? Color.white : new Color(1, 1, 1, 0.3f);
    }

    // Mostrar mensaje temporal
    void MostrarMensaje(string mensaje)
    {
        if (mensajeTMP != null)
        {
            mensajeTMP.text = mensaje;
            CancelInvoke(nameof(LimpiarMensaje));
            Invoke(nameof(LimpiarMensaje), mensajeDuracion);
        }
    }

    void LimpiarMensaje()
    {
        if (mensajeTMP != null)
            mensajeTMP.text = "";
    }

    // Controles para soltar objetos
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            RemoveItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            RemoveItem(1);

    }

    public bool ContieneItem(string itemName)
    {
        foreach (var i in items)
        {
            if (i != null && i.item.itemName == itemName)
                return true;
        }
        return false;
    }

    public bool ContieneItem(ItemData itemName)
    {
        foreach (var i in items)
        {
            if (i != null && i.item == itemName)
                return true;
        }
        return false;
    }

    private class InventoryItem
    {
        public readonly ItemData item;
        public int quantity;

        public InventoryItem(ItemData item)
        {
            this.item = item;
            this.quantity = item.useAmount;
        }

        public InventoryItem(ItemData item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }
}

