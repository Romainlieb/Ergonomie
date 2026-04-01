using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public SequentialTaskManager manager;

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si ce qui entre est soit le joueur, soit un objet tenu (comme le poisson)
        // Pour simplifier : si quelque chose entre dans la zone, on prévient le gestionnaire
        if (other.CompareTag("Player") || other.CompareTag("cuisson_objet") || other.name.Contains("Hand"))
        {
            manager.OnZoneEntered(gameObject);
        }
    }
}