using UnityEngine;

public class TrapActivator : MonoBehaviour
{
    [Header("Trap Settings")]
    public GameObject trapPrefab;
    public bool spawnOnlyOnce = true;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned && spawnOnlyOnce) return;

        if (other.CompareTag("Boss"))
        {
            SpawnTrap();
        }
    }

    void SpawnTrap()
    {
        Instantiate(trapPrefab, transform.position, Quaternion.identity);
        hasSpawned = true;
    }
}
