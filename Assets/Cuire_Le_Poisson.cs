using UnityEngine;

public class CuirePoisson : MonoBehaviour
{
    public Color couleurCuit = new Color(0.5f, 0.3f, 0.2f); // Marron par défaut

    void OnTriggerEnter(Collider autre)
    {
        if (autre.CompareTag("Player"))
        {
            // On récupère le Renderer du poisson
            Renderer rend = autre.GetComponent<Renderer>();

            // On récupère TOUTE la liste des matériaux (obligatoire pour modifier un index)
            Material[] mats = rend.materials;

            // On change directement le premier (l'index 0)
            mats[0].color = couleurCuit;

            // On redonne la liste mise à jour au poisson
            rend.materials = mats;
        }
    }
}