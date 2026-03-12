using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    public string finalCompletionMessage = "All tasks complete!";

    private int currentTaskIndex = 0;

    void Start()
    {
        UpdateUI();
    }

    public void OnZoneEntered(GameObject zoneEntered)
    {
        // Check if the zone entered is the one we are currently looking for
        if (currentTaskIndex < taskList.Count && zoneEntered == taskList[currentTaskIndex].targetZone)
        {
            currentTaskIndex++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (currentTaskIndex < taskList.Count)
        {
            instructionText.text = taskList[currentTaskIndex].instruction;
        }
        else
        {
            instructionText.text = finalCompletionMessage;
        }
    }
}