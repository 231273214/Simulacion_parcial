using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string missionID;
    public string title;
    [TextArea] public string description;
    public MissionType missionType;
    public int requiredAmount;
    public MissionReward[] rewards;
    public MissionState state = MissionState.Available;


    [System.NonSerialized] public int currentProgress;
}

public enum MissionType
{
    Elimination,   
    Collection,     
    Survival,       
    Delivery      
}

public enum MissionState
{
    Available,
    Active,
    Paused,
    Completed,
    Claimed
}

[System.Serializable]
public class MissionReward
{
    public RewardType type;
    public int amount;
}

public enum RewardType
{
    HealthKit,
    Ammo,
    WeaponUpgrade,
    Shield
}
