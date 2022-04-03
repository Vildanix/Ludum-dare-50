using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stats : MonoBehaviour
{
    [System.Serializable]
    public class IntEvent : UnityEvent<int> {}

    [Header("Stats")]
    [SerializeField, Min(0)] int defaultEnergy = 100;
    [SerializeField, Min(0)] int defaultSupplies = 30;
    [SerializeField, Min(0)] int defaultHitPoints = 10;
    [SerializeField, Min(0)] int energy = 50;
    [SerializeField, Min(0)] int supplies = 30;
    [SerializeField, Min(0)] int hitPoints = 0;
    [SerializeField, Min(0)] int maxEnergy = 100;
    [SerializeField, Min(0)] int maxSupplies = 30;
    [SerializeField, Min(0)] int maxHitPoints = 10;

    public UnityEvent onEnergyLost;
    public UnityEvent onSupplyLost;
    public UnityEvent onHitpointsLost;
    [SerializeField] public IntEvent onEnergyChange;
    [SerializeField] public IntEvent onSupplyChange;
    [SerializeField] public IntEvent onHitpointsChange;
    [SerializeField] public IntEvent onMaxEnergyChange;
    [SerializeField] public IntEvent onMaxSupplyChange;
    [SerializeField] public IntEvent onMaxHitpointsChange;

    private void Awake()
    {
        Initialize();
    }

    public void ApplyPointOfInterestResult(PointOfInterest poi)
    {
        var rewards = poi.GetRewards();
        foreach (var reward in rewards)
        {
            switch(reward.rewardType)
            {
                case EventReward.REWARD_TYPE.ENERGY:
                    ChangeEnergy(reward.amount);
                    break;
                case EventReward.REWARD_TYPE.SUPPLIES:
                    ChangeSupplies(reward.amount);
                    break;
                case EventReward.REWARD_TYPE.SHIP:
                    ChangeHitpoints(reward.amount);
                    break;
            }
        }
    }

    public void Initialize()
    {
        energy = defaultEnergy;
        supplies = defaultSupplies;
        hitPoints = defaultHitPoints;

        maxSupplies = defaultSupplies;
        maxEnergy = defaultEnergy;
        maxHitPoints = defaultHitPoints;

        onEnergyChange.Invoke(energy);
        onSupplyChange.Invoke(supplies);
        onHitpointsChange.Invoke(hitPoints);

        onMaxEnergyChange.Invoke(maxEnergy);
        onMaxSupplyChange.Invoke(maxSupplies);
        onMaxHitpointsChange.Invoke(maxHitPoints);
    }

    public void ChangeEnergy(int volume)
    {
        energy += volume;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        if (energy == 0)
        {
            onEnergyLost.Invoke();
        }
        onEnergyChange.Invoke(energy);
    }

    public void ChangeSupplies(int volume)
    {
        supplies += volume;
        supplies = Mathf.Clamp(supplies, 0, maxSupplies);
        if (supplies == 0)
        {
            onSupplyLost.Invoke();
        }
        onSupplyChange.Invoke(supplies);
    }

    public void ChangeHitpoints(int volume)
    {
        hitPoints += volume;
        hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
        if (hitPoints == 0)
        {
            onHitpointsLost.Invoke();
        }
        onHitpointsChange.Invoke(hitPoints);
    }

    public void ChangeMaxEnergy(int volume)
    {
        maxEnergy += volume;
        energy += volume;
        onEnergyChange.Invoke(energy);
        onMaxEnergyChange.Invoke(maxEnergy);
    }

    public void ChangeMaxSupplies(int volume)
    {
        maxSupplies += volume;
        supplies += volume;
        onSupplyChange.Invoke(supplies);
        onMaxSupplyChange.Invoke(maxSupplies);
    }

    public void ChangeMaxHitPoints(int volume)
    {
        maxHitPoints += volume;
        hitPoints += volume;
        onHitpointsChange.Invoke(hitPoints);
        onMaxHitpointsChange.Invoke(maxHitPoints);
    }
}
