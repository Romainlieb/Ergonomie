using UnityEngine;

public class CuirePoisson : MonoBehaviour
{
    // La couleur marron que prendra le poisson
    public Color couleurCuit = new Color(0.5f, 0.3f, 0.2f);

    // Cette fonction s'active quand le poisson entre dans une zone "Trigger"
    void OnTriggerEnter(Collider autre)
    {
        // On vérifie si l'objet touché a le tag "cuisson"
        if (autre.CompareTag("cuisson"))
        {
            // On récupère le moteur de rendu du poisson
            Renderer rend = GetComponent<Renderer>();

            if (rend != null)
            {
                // On récupère ses matériaux
                Material[] mats = rend.materials;

                // On change la couleur du premier matériau (le blanc)
                mats[0].color = couleurCuit;

                // On renvoie les matériaux modifiés au poisson
                rend.materials = mats;

                Debug.Log("Le poisson est dans le feu !");
            }
        }
    }
}