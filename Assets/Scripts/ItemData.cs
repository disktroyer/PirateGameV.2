using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Item", menuName = "Inventario/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Identidad")]
    public string itemName;
    public ItemType itemType = ItemType.Generic;
    public Sprite icon;
    public GameObject prefab;

    [Tooltip("Cuántas unidades se consumen al usar este item.")]
    public int useAmount = 1;
}
