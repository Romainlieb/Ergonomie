using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class CAP_Lancer : MonoBehaviour
{
    [Header("References")]
    public GameObject CAP;
    public GameObject Flotteur;
    public GameObject Poisson; // Objet poisson a spawner au rappel
    public Transform spawnPoint; // Point de spawn du flotteur (optionnel, cherche "Spawn_Flotteur" si null)

    [Tooltip("Assigner le composant HandGrabInteractable present sur l'objet ISDK_HandGrabInteraction de la canne")]
    public HandGrabInteractable handGrabInteractable;

    [Header("Force de lancer")]
    public float throwForceMultiplier = 1.0f;

    [Header("Detection du lancer")]
    public float swipeThreshold = 0.8f; // Vitesse minimale (m/s) pour declencher un lancer
    public float swipeTime = 0.4f;      // Duree max du geste en secondes
    public float throwCooldown = 0.35f;
    public float minSpawnVelocity = 2.0f; // Vitesse minimale requise pour spawner le flotteur (m/s)

    [Header("Rappel du flotteur")]
    public bool enableRecall = true;    // Activer le systeme de rappel
    public float recallThreshold = 0.6f; // Vitesse minimale du mouvement inverse pour declencher le rappel (m/s)
    public float recallForceMultiplier = 1.2f; // Multiplicateur de force pour rappeler le flotteur
    public float recallCompleteDistance = 0.5f; // Distance pour considerer le flotteur rappele (metres)
    public float poissonSpawnHeight = 0.1f; // Hauteur au-dessus des pieds pour spawner le poisson

    [Header("Debug")]
    public bool debugLogs = false;

    private bool isHeldByPlayer = false;
    private GameObject currentFlotteur = null;

    private bool trackingSwipe = false;
    private float swipeStartTime;
    private float lastThrowTime = -999f;
    private Vector3 peakSwipeVelocity;

    private Vector3 lastCapPosition;
    private Vector3 capVelocity;
    private bool hasLastPosition = false;
    private float nextDebugLogTime = 0f;

    private Vector3 lastThrowVelocity; // Velocite initiale du lancer pour calculer le mouvement inverse
    private bool isRecallingFlotteur = false; // Flag pour savoir si le flotteur est en train d'etre rappele

    void Start()
    {
        if (CAP == null)
        {
            CAP = gameObject;
        }

        if (spawnPoint == null && CAP != null)
        {
            Transform spawnChild = CAP.transform.Find("Spawn_Flotteur");
            if (spawnChild != null)
            {
                spawnPoint = spawnChild;
            }
        }
    }

    void OnEnable()
    {
        if (handGrabInteractable != null)
        {
            handGrabInteractable.WhenStateChanged += OnGrabStateChanged;
        }
    }

    void OnDisable()
    {
        if (handGrabInteractable != null)
        {
            handGrabInteractable.WhenStateChanged -= OnGrabStateChanged;
        }
    }

    void OnGrabStateChanged(InteractableStateChangeArgs args)
    {
        bool nowHeld = args.NewState == InteractableState.Select;
        if (nowHeld == isHeldByPlayer)
        {
            return;
        }

        isHeldByPlayer = nowHeld;
        if (!isHeldByPlayer)
        {
            ResetTracking();
            if (!isRecallingFlotteur)
            {
                DestroyCurrentFlotteur();
            }
        }

        if (debugLogs)
        {
            Debug.Log("CAP tenue: " + isHeldByPlayer);
        }
    }

    void Update()
    {
        if (CAP == null || Flotteur == null)
        {
            return;
        }

        if (debugLogs && Time.time >= nextDebugLogTime)
        {
            Debug.Log("CAP_Lancer debug | held=" + isHeldByPlayer + " | speed=" + capVelocity.magnitude.ToString("F2") + " m/s");
            nextDebugLogTime = Time.time + 1f;
        }

        if (!isHeldByPlayer)
        {
            ResetTracking();
            return;
        }

        UpdateCapVelocity();

        // Verifier le rappel du flotteur si celui-ci est actif
        if (currentFlotteur != null && enableRecall)
        {
            UpdateFlotteurRecall();
        }

        DetectSwipe();
    }

    void DetectSwipe()
    {
        float currentSpeed = capVelocity.magnitude;

        if (!trackingSwipe)
        {
            if (currentSpeed < swipeThreshold)
            {
                return;
            }

            trackingSwipe = true;
            swipeStartTime = Time.time;
            peakSwipeVelocity = capVelocity;
        }
        else
        {
            if (currentSpeed > peakSwipeVelocity.magnitude)
            {
                peakSwipeVelocity = capVelocity;
            }

            float swipeDuration = Time.time - swipeStartTime;
            bool gestureEnded = currentSpeed < (swipeThreshold * 0.5f);

            if (swipeDuration <= swipeTime && !gestureEnded)
            {
                return;
            }

            if (Time.time - lastThrowTime >= throwCooldown && peakSwipeVelocity.magnitude >= swipeThreshold)
            {
                SpawnAndThrowFlotteur(peakSwipeVelocity);
                lastThrowTime = Time.time;
            }

            trackingSwipe = false;
        }
    }

    void SpawnAndThrowFlotteur(Vector3 velocity)
    {
        // Verifier la vitesse minimale requise pour spawner
        if (velocity.magnitude < minSpawnVelocity)
        {
            if (debugLogs)
            {
                Debug.Log("Vitesse insuffisante pour spawner: " + velocity.magnitude.ToString("F2") + " < " + minSpawnVelocity);
            }
            return;
        }

        // Detruire le flotteur precedent s'il existe
        DestroyCurrentFlotteur();

        // Determiner le point de spawn (spawnPoint ou CAP)
        Vector3 spawnPosition = (spawnPoint != null) ? spawnPoint.position : CAP.transform.position;

        // Spawner le nouveau flotteur
        currentFlotteur = Instantiate(Flotteur, spawnPosition, Quaternion.identity);

        Rigidbody rb = currentFlotteur.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = currentFlotteur.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = velocity * throwForceMultiplier;
        rb.angularVelocity = Vector3.zero;

        // Sauvegarder la velocite initiale pour le rappel
        lastThrowVelocity = velocity;

        Debug.Log("Flotteur lance a la vitesse: " + rb.linearVelocity.magnitude.ToString("F2") + " m/s");
    }

    void UpdateCapVelocity()
    {
        Vector3 currentPosition = CAP.transform.position;

        if (!hasLastPosition)
        {
            lastCapPosition = currentPosition;
            capVelocity = Vector3.zero;
            hasLastPosition = true;
            return;
        }

        float dt = Time.deltaTime;
        if (dt > 0f)
        {
            capVelocity = (currentPosition - lastCapPosition) / dt;
        }

        lastCapPosition = currentPosition;
    }

    void ResetTracking()
    {
        trackingSwipe = false;
        capVelocity = Vector3.zero;
        hasLastPosition = false;
        peakSwipeVelocity = Vector3.zero;
        isRecallingFlotteur = false;
    }

    void UpdateFlotteurRecall()
    {
        if (currentFlotteur == null)
        {
            return;
        }

        Rigidbody rb = currentFlotteur.GetComponent<Rigidbody>();
        if (rb == null)
        {
            return;
        }

        // Calculer la distance entre le flotteur et la main
        float distanceToHand = Vector3.Distance(currentFlotteur.transform.position, CAP.transform.position);

        // Si le flotteur est rappele et arrive a la main
        if (isRecallingFlotteur && distanceToHand < recallCompleteDistance)
        {
            SpawnPoisson();
            DestroyCurrentFlotteur();
            isRecallingFlotteur = false;
            return;
        }

        // Calcul du mouvement inverse : produit scalaire pour voir si la vélocité de la main va dans la direction opposée au lancer
        float reverseMovement = Vector3.Dot(capVelocity, -lastThrowVelocity.normalized);

        // Si le mouvement inverse est suffisant et que le joueur bouge vers l'arrière
        if (reverseMovement > recallThreshold && !isRecallingFlotteur)
        {
            isRecallingFlotteur = true;
            if (debugLogs)
            {
                Debug.Log("Rappel du flotteur activé | reverseMovement=" + reverseMovement.ToString("F2"));
            }
        }

        // Appliquer la force de rappel si en phase de rappel
        if (isRecallingFlotteur)
        {
            // Calculer la direction de rappel (vers la main)
            Vector3 recallDirection = (CAP.transform.position - currentFlotteur.transform.position).normalized;

            // Appliquer une force dans la direction du rappel
            rb.linearVelocity = recallDirection * capVelocity.magnitude * recallForceMultiplier;
        }
    }

    void SpawnPoisson()
    {
        if (Poisson == null)
        {
            if (debugLogs)
            {
                Debug.LogWarning("Aucun prefab Poisson assigne !");
            }
            return;
        }

        // Calculer la position au pied du joueur
        Vector3 poissonSpawnPosition = new Vector3(
            CAP.transform.position.x,
            CAP.transform.position.y - poissonSpawnHeight,
            CAP.transform.position.z
        );

        // Spawner le poisson
        GameObject spawnedPoisson = Instantiate(Poisson, poissonSpawnPosition, Quaternion.identity);

        Debug.Log("Poisson spawne aux pieds du joueur !");
    }

    void DestroyCurrentFlotteur()
    {
        if (currentFlotteur != null)
        {
            Destroy(currentFlotteur);
            currentFlotteur = null;
        }
    }

    // Peut aussi etre appelee par un UnityEvent externe (ex: autre composant de grab)
    public void SetHeldState(bool held)
    {
        isHeldByPlayer = held;
        if (!held)
        {
            ResetTracking();
        }
    }
}