using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MissionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject missionPanel;
    public Transform missionListParent;
    public GameObject missionEntryPrefab;

    void Update()
    {
        // Mostrar/ocultar con TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            missionPanel.SetActive(!missionPanel.activeSelf);
            if (missionPanel.activeSelf)
            {
                UpdateMissionUI();
            }
        }
    }

    public void UpdateMissionUI()
    {
        // Limpiar lista
        foreach (Transform child in missionListParent)
        {
            Destroy(child.gameObject);
        }

        // Mostrar misiones activas
        //foreach (MissionData mission in MissionManager.Instance.activeMissions)
        //{
            //GameObject entry = Instantiate(missionEntryPrefab, missionListParent);
            //Text missionText = entry.GetComponentInChildren<Text>();

            //string progress = $"{mission.currentProgress}/{mission.objectives[0].requiredAmount}";
            //missionText.text = $"{mission.title}\nProgreso: {progress}";
        //}
    }
}