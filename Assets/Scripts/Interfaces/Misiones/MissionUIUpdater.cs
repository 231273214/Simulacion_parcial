using UnityEngine;
using TMPro;
using System.Collections;

public class MissionUIUpdater : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject missionUIPanel;
    public TextMeshProUGUI missionTitleText;
    public TextMeshProUGUI missionProgressText;

    private MissionData currentMission;
    private Coroutine updateCoroutine;

    void Start()
    {
        if (missionUIPanel != null)
            missionUIPanel.SetActive(false);
    }

    public void ShowMissionUI(MissionData mission)
    {
        currentMission = mission;

        if (missionUIPanel != null)
            missionUIPanel.SetActive(true);

        if (missionTitleText != null)
            missionTitleText.text = mission.title;

        // Iniciar actualización en tiempo real
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(UpdateMissionProgressUI());
    }

    public void HideMissionUI()
    {
        if (missionUIPanel != null)
            missionUIPanel.SetActive(false);

        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);
    }

    IEnumerator UpdateMissionProgressUI()
    {
        while (currentMission != null && currentMission.state == MissionState.Active)
        {
            UpdateProgressText();
            yield return new WaitForSeconds(0.5f); // Actualizar cada medio segundo
        }
    }

    void UpdateProgressText()
    {
        if (missionProgressText != null && currentMission != null)
        {
            string progressText = "";

            switch (currentMission.missionType)
            {
                case MissionType.Elimination:
                    progressText = $"Zombies eliminados: {currentMission.currentProgress}/{currentMission.requiredAmount}";
                    break;
                case MissionType.Survival:
                    int minutes = Mathf.FloorToInt(currentMission.requiredAmount / 60);
                    int seconds = Mathf.FloorToInt(currentMission.requiredAmount % 60);
                    progressText = $"Tiempo restante: {minutes:00}:{seconds:00}";
                    break;
                case MissionType.Collection:
                    progressText = $"Objetos recolectados: {currentMission.currentProgress}/{currentMission.requiredAmount}";
                    break;
            }

            missionProgressText.text = progressText;
        }
    }
}
