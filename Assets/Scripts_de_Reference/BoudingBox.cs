using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    // Give your manager script as input in unity interface !
    public SequentialTaskManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager script "Hey, I just stepped into this game object"
            manager.OnZoneEntered(this.gameObject);
        }
    }
}