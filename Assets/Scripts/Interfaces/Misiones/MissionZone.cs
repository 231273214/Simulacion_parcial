using UnityEngine;
using System.Collections;

public class MissionZone : MonoBehaviour
{
    [Header("Configuración Zona")]
    public string missionID;
    public bool activateOnMissionAccept = true;

    [Header("Referencias")]
    public ZombieSpawner zombieSpawner;
    public GameObject missionUI;
    public MissionTimer missionTimer;
    public MissionUIUpdater missionUIUpdater;
    public MissionData missionData;

    private bool isActive = false;
    private bool playerInZone = false;

    void Start()
    {
        // Desactivar todo al inicio
        if(zombieSpawner != null) zombieSpawner.SetGlobalSpawning(false);
        if (missionUI != null) missionUI.SetActive(false);
        if (missionTimer != null) missionTimer.StopTimer();
    }

    void Update()
    {
        if (isActive && playerInZone)
        {
            // La misión está en progreso mientras el player esté en zona
            UpdateMissionProgress();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (isActive)
            {
                StartMissionInZone();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (isActive)
            {
                PauseMissionInZone();
            }
        }
    }

    public void ActivateZone()
    {
        isActive = true;
        Debug.Log($"Zona de misión {missionID} activada");
    }

    void StartMissionInZone()
    {
        // activar sólo este spawner
        if (zombieSpawner != null)
        {
            zombieSpawner.SetGlobalSpawning(true);
        }

        if (missionUIUpdater != null && missionData != null)
            missionUIUpdater.ShowMissionUI(missionData);

        if (missionID == "SURVIVE_3_MINUTES" && missionTimer != null)
            missionTimer.StartTimer(180f);

        if (missionData != null)
            missionData.state = MissionState.Active;

        Debug.Log($"Misión {missionID} iniciada en zona");
    }

    void PauseMissionInZone()
    {
        if (zombieSpawner != null)
            zombieSpawner.SetGlobalSpawning(false);

        if (missionUIUpdater != null)
            missionUIUpdater.HideMissionUI();

        if (missionTimer != null)
            missionTimer.PauseTimer();

        if (missionData != null)
            missionData.state = MissionState.Paused;

        Debug.Log("Misión pausada en zona");
    }


    void CompleteMission()
    {
        if (missionData != null)
            missionData.state = MissionState.Completed;

        if (missionUIUpdater != null)
            missionUIUpdater.HideMissionUI();
    }


    void UpdateMissionProgress()
    {
        // Para misiones de supervivencia, el timer maneja el progreso
        // Para misiones de eliminación, se actualiza automáticamente con las muertes
    }
}
