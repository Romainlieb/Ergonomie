using UnityEngine;

public class CuirePoisson : MonoBehaviour
{
    // La couleur marron que prendra le poisson
    public Color couleurCuit = new Color(0.5f, 0.3f, 0.2f);

    // Cette fonction s'active quand le poisson entre dans une zone "Trigger"
    void OnTriggerEnter(Collider autre)
    {
        // On v�rifie si l'objet touch� a le tag "Cuisson"
        if (autre.CompareTag("Cuisson"))
        {
            // On r�cup�re le moteur de rendu du poisson
            Renderer rend = GetComponent<Renderer>();

            if (rend != null)
            {
                // On r�cup�re ses mat�riaux
                Material[] mats = rend.materials;

                // On change la couleur du premier mat�riau (le blanc)
                mats[0].color = couleurCuit;

                // On renvoie les mat�riaux modifi�s au poisson
                rend.materials = mats;

                Debug.Log("Le poisson est dans le feu !");
            }
        }
    }
}