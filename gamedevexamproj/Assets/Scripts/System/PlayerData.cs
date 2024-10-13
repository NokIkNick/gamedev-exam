using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int gemCount;
    public float lastCheckpointX;
    public float lastCheckpointY;
    public float lastCheckpointZ;
    public String levelName;

    public PlayerData(PlayerData newData){
        gemCount = newData.gemCount;
        lastCheckpointX = newData.lastCheckpointX;
        lastCheckpointY = newData.lastCheckpointY;
        lastCheckpointZ = newData.lastCheckpointZ;
        levelName = newData.levelName;
    }

    public PlayerData(){}
}
