using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    FASTER_SHOOTING, BETTER_BARREL, FARTHER_FIRING, AUTO_AIM, HEAVIER_BULLETS, EXPLOSIVE_PELLETS
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{

    [Header("Basic Stats")]
    public string upgradeName;
    public string upgradeDescription;
    public Sprite previewSprite;
    [Header("Upgrade Type")]
    public UpgradeType type;
    [Header("Upgrade Values")]
    public List<float> upgradeValues = new List<float>();

    public int GetMaxLevel()
    {
        return upgradeValues.Count;
    }

    public float GetUpgradeValue(int level)
    {
        level--; // Level 1 is the starting level
        if (level < 0 || level >= GetMaxLevel())
        {
            Debug.Log("TRIED TO GET LEVEL " + level.ToString() + " FOR " + upgradeName + " UPGRADE, WHICH DOES NOT EXIST! (Upgrade.cs)");
        }
        return upgradeValues[level];
    }

}
