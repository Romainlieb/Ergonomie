using UnityEngine;

public class CAP_Lancer : MonoBehaviour
{
    public GameObject CAP;
    public GameObject Flotteur; // Assurez-vous que cet objet est assigné dans l'inspecteur
    public float throwForceMultiplier = 5.0f; // Force de lancement

    private bool trackingSwipe = false;
    private Vector3 swipeStartPos;
    private float swipeStartTime;
    private float swipeThreshold = 0.5f; // Distance minimale pour détecter un lancer
    private float swipeTime = 1.0f; // Temps maximum pour un lancer

    void Start()
    {
    }

    void Update()
    {
        DetectSwipe();
    }

    void DetectSwipe()
    {
        Vector3 handPos = CAP.transform.position;

        if (!trackingSwipe)
        {
            trackingSwipe = true;
            swipeStartPos = handPos;
            swipeStartTime = Time.time;
        }
        else
        {
            float swipeDist = Vector3.Distance(handPos, swipeStartPos);
            float swipeDuration = Time.time - swipeStartTime;

            if (swipeDist >= swipeThreshold && swipeDuration <= swipeTime)
            {
                Vector3 swipeDirection = (handPos - swipeStartPos).normalized;
                SpawnAndThrowFlotteur(swipeDirection);
                trackingSwipe = false;
            }
            else if (swipeDuration > swipeTime)
            {
                // Reset si le mouvement est trop lent
                trackingSwipe = false;
            }
        }
    }

    void SpawnAndThrowFlotteur(Vector3 direction)
    {
        // Le flotteur spawn exactement à la position actuelle de la canne à pêche (CAP)
        Vector3 spawnPosition = CAP.transform.position;
        GameObject flotteurInstance = Instantiate(Flotteur, spawnPosition, Quaternion.identity);
        Rigidbody rb = flotteurInstance.GetComponent<Rigidbody>();
        if (rb == null) rb = flotteurInstance.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction * throwForceMultiplier; // Utilisation de linearVelocity pour appliquer la force
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Flotteur lancé avec direction: " + direction + " et force: " + throwForceMultiplier);
    }
}