using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventReward", menuName = "ScriptableObjects/CreateEventReward", order = 1)]
public class EventReward : ScriptableObject
{
    public enum REWARD_TYPE { ENERGY, SUPPLIES, SHIP}

    public REWARD_TYPE rewardType;
    public int amount;
    public string description;
    /*
    public REWARD_TYPE GetRewardType()
    {
        return rewardType;
    }

    public int GetRewardAmount()
    {
        return amount;
    }

    public string GetDescription()
    {
        return description;
    }
    */
}
