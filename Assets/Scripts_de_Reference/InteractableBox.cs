using UnityEngine;
using Oculus.Interaction; // If using Meta's Interaction SDK

public class InteractableTask : MonoBehaviour
{
    public SequentialTaskManager manager;

    // Call this method when the object is picked up
    // You can link this in the Unity Inspector
    public void OnObjectPickedUp()
    {
        if (manager != null)
        {
            manager.OnZoneEntered(this.gameObject);
        }
    }
}