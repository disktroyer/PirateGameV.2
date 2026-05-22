using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossTriggerTrap : MonoBehaviour
{
    [Header("Configuración de Boss Trigger")]
    public string bossTag = "Boss";
    public TrapInteractable trapInteractable;

    private void Reset()
    {
        trapInteractable = GetComponent<TrapInteractable>();
    }

    private void Awake()
    {
        if (trapInteractable == null)
            trapInteractable = GetComponent<TrapInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (trapInteractable == null || !collision.CompareTag(bossTag))
            return;

        trapInteractable.TriggerBoss(collision.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}
