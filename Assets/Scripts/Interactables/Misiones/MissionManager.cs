using UnityEngine;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public List<MissionData> allMissions = new List<MissionData>();
    private List<MissionData> activeMissions = new List<MissionData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateAllMissions();
    }

    void CreateAllMissions()
    {
        // MISIÓN 1: Eliminación básica
        allMissions.Add(new MissionData
        {
            missionID = "KILL_10_ZOMBIES",
            title = "Limpieza Inicial",
            description = "Elimina 10 zombies para asegurar el área",
            missionType = MissionType.Elimination,
            requiredAmount = 10,
            rewards = new MissionReward[] {
                new MissionReward { type = RewardType.HealthKit, amount = 2 },
                new MissionReward { type = RewardType.Ammo, amount = 20 }
            }
        });

        // MISIÓN 2: Eliminación avanzada
        allMissions.Add(new MissionData
        {
            missionID = "KILL_25_ZOMBIES",
            title = "Caza Mayor",
            description = "Demuestra tu valía eliminando 25 zombies",
            missionType = MissionType.Elimination,
            requiredAmount = 25,
            rewards = new MissionReward[] {
                new MissionReward { type = RewardType.WeaponUpgrade, amount = 1 },
                new MissionReward { type = RewardType.Ammo, amount = 30 }
            }
        });

        // MISIÓN 3: Supervivencia
        allMissions.Add(new MissionData
        {
            missionID = "SURVIVE_3_MINUTES",
            title = "Supervivencia Extrema",
            description = "Sobrevive 3 minutos en zona infectada",
            missionType = MissionType.Survival,
            requiredAmount = 180, // 3 minutos en segundos
            rewards = new MissionReward[] {
                new MissionReward { type = RewardType.Shield, amount = 1 },
                new MissionReward { type = RewardType.HealthKit, amount = 3 }
            }
        });

        Debug.Log($"Misiones creadas: {allMissions.Count}");
    }

    public List<MissionData> GetAvailableMissions()
    {
        return allMissions.FindAll(m => m.state == MissionState.Available);
    }

    public List<MissionData> GetCompletedMissions()
    {
        return allMissions.FindAll(m => m.state == MissionState.Completed);
    }

    public void AcceptMission(string missionID)
    {
        MissionData mission = allMissions.Find(m => m.missionID == missionID);
        if (mission != null && mission.state == MissionState.Available)
        {
            mission.state = MissionState.Active;
            activeMissions.Add(mission);

            // ACTIVAR la zona correspondiente a esta misión
            ActivateMissionZone(missionID);

            Debug.Log($"Misión aceptada: {mission.title}");
        }
    }

    void ActivateMissionZone(string missionID)
    {
        MissionZone[] zones = FindObjectsOfType<MissionZone>();
        foreach (MissionZone zone in zones)
        {
            if (zone.missionID == missionID)
            {
                zone.ActivateZone();
            }
        }
    }

    public void UpdateMissionProgress(string missionID, int amount)
    {
        MissionData mission = activeMissions.Find(m => m.missionID == missionID);
        if (mission != null)
        {
            mission.currentProgress += amount;
            if (mission.currentProgress >= mission.requiredAmount)
            {
                mission.state = MissionState.Completed;
                Debug.Log($"¡Misión completada: {mission.title}!");
            }
        }
    }

    public void ClaimReward(string missionID)
    {
        MissionData mission = allMissions.Find(m => m.missionID == missionID);
        if (mission != null && mission.state == MissionState.Completed)
        {
            // Dar recompensas al player
            GiveRewards(mission.rewards);
            mission.state = MissionState.Claimed;
            activeMissions.Remove(mission);
        }
    }

    void GiveRewards(MissionReward[] rewards)
    {
        foreach (MissionReward reward in rewards)
        {
            Debug.Log($"Recompensa: {reward.type} x{reward.amount}");
            // Aquí conectar con sistemas del player
        }
    }
}
