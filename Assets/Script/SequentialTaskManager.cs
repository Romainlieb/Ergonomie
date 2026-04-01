using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections; // <-- IMPORTANT pour le minuteur

public class SequentialTaskManager : MonoBehaviour
{
    [System.Serializable]
    public class TaskStep
    {
        public string instruction;
        public GameObject targetZone;
    }

    [Header("UI Components")]
    public TextMeshProUGUI instructionText;

    [Header("Task Sequence")]
    public List<TaskStep> taskList = new List<TaskStep>();
    public string finalCompletionMessage = "Toutes les tâches sont terminées !";

    private int currentTaskIndex = 0;
    private Coroutine chrono; // Garde en mémoire notre minuteur

    void Start()
    {
        UpdateUI();
    }

    public void OnZoneEntered(GameObject zoneEntered)
    {
        // Vérifie si la zone touchée est bien celle qu'on attend
        if (currentTaskIndex < taskList.Count && zoneEntered == taskList[currentTaskIndex].targetZone)
        {
            currentTaskIndex++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Met à jour le texte avec la bonne instruction
        if (currentTaskIndex < taskList.Count)
        {
            instructionText.text = taskList[currentTaskIndex].instruction;
        }
        else
        {
            instructionText.text = finalCompletionMessage;
        }

        // --- LE SYSTÈME DE 8 SECONDES ---
        // Si un compte à rebours est déjà en cours, on le coupe pour ne pas cacher le nouveau message trop vite
        if (chrono != null)
        {
            StopCoroutine(chrono);
        }
        // On lance le nouveau minuteur de 8 secondes
        chrono = StartCoroutine(CacherTexteApresDelai(8f));
    }

    // Le minuteur en arrière-plan
    IEnumerator CacherTexteApresDelai(float tempsAAttendre)
    {
        yield return new WaitForSeconds(tempsAAttendre); // Attend exactement le temps demandé
        instructionText.text = ""; // Vide le texte pour le faire disparaître
    }
}