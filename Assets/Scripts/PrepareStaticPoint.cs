using UnityEngine;

public class PrepareStaticPoint : Interactable
{
    public StaticTrigger target;

    public override void Interact(GameObject actor)
    {
        if (!GamePhaseManager.Instance.EsEjecucion()) return;

        var inv = actor.GetComponent<InventoryManager>();
        target.Preparar(inv);
    }
}
