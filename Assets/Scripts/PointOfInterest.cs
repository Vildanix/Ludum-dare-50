using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] (int, int) poiCoordinates;
    [SerializeField, TextArea] string description;

    [SerializeField] List<EventReward> Rewards;

    public void SetupEvent((int, int) coords)
    {
        poiCoordinates = coords;
    }

    public (int, int) GetCoordinates()
    {
        return poiCoordinates;
    }

    public string GetDescription()
    {
        return description;
    }

    public IReadOnlyList<EventReward> GetRewards()
    {
        return Rewards.AsReadOnly();
    }

}
