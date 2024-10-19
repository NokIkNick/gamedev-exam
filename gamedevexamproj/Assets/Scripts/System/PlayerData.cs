using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int? gemCount;
    public float? lastCheckpointX;
    public float? lastCheckpointY;
    public float? lastCheckpointZ;
    public string? levelName;
    public bool? hasWeapon;
    public int? health;

    public PlayerData(PlayerData newData){
        gemCount = newData.gemCount;
        lastCheckpointX = newData.lastCheckpointX;
        lastCheckpointY = newData.lastCheckpointY;
        lastCheckpointZ = newData.lastCheckpointZ;
        levelName = newData.levelName;
        hasWeapon = newData.hasWeapon;
        health = newData.health;
    }

    public PlayerData(){}
}
