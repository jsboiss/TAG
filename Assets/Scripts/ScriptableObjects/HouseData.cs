using UnityEngine;

[CreateAssetMenu(fileName = "NewHouse", menuName = "Game Data/House")]
public class HouseData : BuildingData
{
    public HouseData()
    {
        health = 100;
    }
}
