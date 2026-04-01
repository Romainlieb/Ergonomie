using UnityEngine;

public class TreeHit : MonoBehaviour
{
    public int hitsRequired = 3;        // 3 hits
    private int currentHits = 0;

    public GameObject woodPrefab;       // Fire_Wood
    public Transform spawnPoint;        // spawn location
    private bool hasSpawned = false;    // să apară doar o singură dată

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Axe"))
        {
            currentHits++;

            if (currentHits >= hitsRequired && !hasSpawned)
            {
                SpawnWood();
            }
        }
    }

    void SpawnWood()
    {
        hasSpawned = true;

        GameObject wood = Instantiate(woodPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = wood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 2f + transform.forward * 1.5f, ForceMode.Impulse);
        }
    }
}