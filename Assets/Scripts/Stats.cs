using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stats : MonoBehaviour
{
    [System.Serializable]
    public class IntEvent : UnityEvent<int> {}

    [Header("Stats")]
    [SerializeField, Min(0)] int defaultEnergy = 50;
    [SerializeField, Min(0)] int defaultSupplies = 30;
    //[SerializeField, Min(0)] int defaultJumpFuel = 0;
    [SerializeField, Min(0)] int energy = 50;
    [SerializeField, Min(0)] int supplies = 30;
    //[SerializeField, Min(0)] int jumpFuel = 0;
    [SerializeField, Min(0)] int maxEnergy = 50;
    [SerializeField, Min(0)] int maxSupplies = 30;
    //[SerializeField, Min(0)] int maxJumpFuel = 0;

    public UnityEvent onEnergyLost;
    public UnityEvent onSupplyLost;
    [SerializeField] public IntEvent onEnergyChange;
    [SerializeField] public IntEvent onSupplyChange;
    [SerializeField] public IntEvent onMaxEnergyChange;
    [SerializeField] public IntEvent onMaxSupplyChange;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        energy = defaultEnergy;
        supplies = defaultSupplies;
        maxEnergy = defaultEnergy;
        maxSupplies = defaultSupplies;
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

    public void ChangeMaxEnergy(int volume)
    {
        maxEnergy += volume;
        energy += volume;
        onEnergyChange.Invoke(energy);
        onMaxEnergyChange.Invoke(maxEnergy);
    }

    public void AddSupplies(int volume)
    {
        maxSupplies += volume;
        supplies += volume;
        onSupplyChange.Invoke(supplies);
        onMaxSupplyChange.Invoke(maxSupplies);
    }


}
