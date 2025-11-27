using UnityEngine;
using TMPro;
using System.Collections;

public class MissionTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText; // CAMBIADO A TMP
    public GameObject timerPanel;

    private float timeRemaining;
    private bool timerRunning = false;
    private Coroutine timerCoroutine;

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        timerRunning = true;

        if (timerPanel != null) timerPanel.SetActive(true);

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(TimerCountdown());
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void StopTimer()
    {
        timerRunning = false;
        if (timerPanel != null) timerPanel.SetActive(false);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
    }

    IEnumerator TimerCountdown()
    {
        while (timeRemaining > 0 && timerRunning)
        {
            UpdateTimerDisplay();
            yield return new WaitForSeconds(1f);
            timeRemaining--;

            if (timeRemaining <= 0)
            {
                MissionCompleted();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void MissionCompleted()
    {
        Debug.Log("¡Misión de supervivencia completada!");
        MissionManager.Instance.UpdateMissionProgress("SURVIVE_3_MINUTES", 180);
        StopTimer();
    }
}
