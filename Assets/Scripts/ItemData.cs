using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Item", menuName = "Inventario/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public int useAmount = 1;
}
