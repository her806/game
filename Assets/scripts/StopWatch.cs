using UnityEngine;
using TMPro;

public class StopWatch : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText;
    
    private float currentTime;
    private bool isRunning;

    void Start()
    {
        ResetStopwatch();
        StartStopwatch();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime += Time.deltaTime;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (stopwatchText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);

        stopwatchText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void StartStopwatch() => isRunning = true;
    public void PauseStopwatch() => isRunning = false;
    
    public void ResetStopwatch()
    {
        currentTime = 0f;
        UpdateUI();
    }
}