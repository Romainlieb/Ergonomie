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
    public string finalCompletionMessage = "Toutes les t�ches sont termin�es !";

    private int currentTaskIndex = 0;
    private Coroutine chrono; // Garde en m�moire notre minuteur

    void Start()
    {
        UpdateUI();
    }

    public void OnZoneEntered(GameObject zoneEntered)
    {
        // V�rifie si la zone touch�e est bien celle qu'on attend
        if (currentTaskIndex < taskList.Count && zoneEntered == taskList[currentTaskIndex].targetZone)
        {
            currentTaskIndex++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Met � jour le texte avec la bonne instruction
        if (currentTaskIndex < taskList.Count)
        {
            instructionText.text = taskList[currentTaskIndex].instruction;
        }
        else
        {
            instructionText.text = finalCompletionMessage;
        }

        // --- LE SYST�ME DE 8 SECONDES ---
        // Si un compte � rebours est d�j� en cours, on le coupe pour ne pas cacher le nouveau message trop vite
        if (chrono != null)
        {
            StopCoroutine(chrono);
        }
        // On lance le nouveau minuteur de 8 secondes
        chrono = StartCoroutine(CacherTexteApresDelai(15f));
    }

    // Le minuteur en arri�re-plan
    IEnumerator CacherTexteApresDelai(float tempsAAttendre)
    {
        yield return new WaitForSeconds(tempsAAttendre); // Attend exactement le temps demand�
        instructionText.text = ""; // Vide le texte pour le faire dispara�tre
    }
}