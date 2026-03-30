using UnityEngine;

public class PlaceableTrap : Interactable
{
    public string requiredItem;
    public GameObject trapPrefab;

    public override void Interact(GameObject actor)
    {
        InventoryManager inv = actor.GetComponent<InventoryManager>();

        if (!inv.ContieneItem(requiredItem))
        {
            Debug.Log("No tienes el ítem para colocar esta trampa");
            return;
        }

        Instantiate(trapPrefab, actor.transform.position, Quaternion.identity);
        Debug.Log("Trampa colocada");
    }
}
