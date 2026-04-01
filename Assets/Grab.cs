
using UnityEngine;
using System.Collections;

public class FishCooking : MonoBehaviour
{
    [Header("Paramètres de cuisson")]
    public float cookingTime = 5f; // Temps en secondes pour cuire
    public Color cookedColor = new Color(0.5f, 0.3f, 0.1f); // Couleur marron/dorée
    
    private bool isCooked = false;
    private Coroutine cookingRoutine;
    private Renderer fishRenderer;

    void Start()
    {
        fishRenderer = GetComponent<Renderer>();
    }

    // Déclenché quand le poisson entre dans le trigger du feu
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire") && !isCooked)
        {
            Debug.Log("Le poisson commence à cuire...");
            cookingRoutine = StartCoroutine(CookFish());
        }
    }

    // Déclenché si on retire le poisson du feu avant la fin
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire") && cookingRoutine != null)
        {
            Debug.Log("Cuisson interrompue !");
            StopCoroutine(cookingRoutine);
        }
    }

    IEnumerator CookFish()
    {
        yield return new WaitForSeconds(cookingTime);
        
        // Changer l'apparence
        fishRenderer.material.color = cookedColor;
        isCooked = true;
        
        Debug.Log("Le poisson est cuit !");
        
        // Optionnel : Ajouter un petit effet sonore ou des particules ici
    }
}