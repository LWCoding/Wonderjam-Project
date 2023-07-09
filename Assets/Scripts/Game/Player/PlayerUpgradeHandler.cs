using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeHandler : MonoBehaviour
{

    public static PlayerUpgradeHandler Instance;
    public int currLevel;
    private Dictionary<UpgradeType, int> playerUpgradeLevels = new Dictionary<UpgradeType, int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        InitializeAllLevelsToOne();
        InitializeStatsBasedOffUpgradeLevels();
    }

    private void InitializeAllLevelsToOne()
    {
        currLevel = 1;
        foreach (UpgradeType upgradeType in Globals.GetAllUpgradeTypes())
        {
            playerUpgradeLevels.Add(upgradeType, 1);
        }
    }

    private void InitializeStatsBasedOffUpgradeLevels()
    {
        foreach (UpgradeType upgradeType in playerUpgradeLevels.Keys)
        {
            RenderUpgradeChanges(upgradeType);
        }
    }

    public List<Upgrade> GetImpossibleUpgrades()
    {
        List<Upgrade> possibleUpgrades = new List<Upgrade>();
        foreach (UpgradeType upgradeType in playerUpgradeLevels.Keys)
        {
            Upgrade currUpgrade = Globals.GetUpgrade(upgradeType);
            if (currUpgrade.GetMaxLevel() <= GetUpgradeLevel(upgradeType))
            {
                possibleUpgrades.Add(currUpgrade);
            }
        }
        return possibleUpgrades;
    }

    public int GetUpgradeLevel(UpgradeType upgradeType)
    {
        return playerUpgradeLevels[upgradeType];
    }

    public void AddUpgrade(UpgradeType upgradeType)
    {
        playerUpgradeLevels[upgradeType]++;
        currLevel++;
        RenderUpgradeChanges(upgradeType);
    }

    private void RenderUpgradeChanges(UpgradeType upgradeType)
    {
        int level = playerUpgradeLevels[upgradeType];
        float upgradeValue = Globals.GetUpgrade(upgradeType).GetUpgradeValue(level);
        switch (upgradeType)
        {
            case UpgradeType.FASTER_SHOOTING:
                PlayerWeaponHandler.Instance.delayBetweenShots = upgradeValue;
                return;
            case UpgradeType.FARTHER_FIRING:
                PlayerWeaponHandler.Instance.bulletLifetime = upgradeValue;
                return;
            case UpgradeType.AUTO_AIM:
                PlayerWeaponHandler.Instance.bulletSpreadAngle = upgradeValue;
                return;
            case UpgradeType.BETTER_BARREL:
                PlayerWeaponHandler.Instance.turretSwivelSpeed = upgradeValue;
                return;
            case UpgradeType.HEAVIER_BULLETS:
                PlayerWeaponHandler.Instance.bulletSpeed = upgradeValue;
                return;
            case UpgradeType.EXPLOSIVE_PELLETS:
                PlayerWeaponHandler.Instance.bulletAoERadius = upgradeValue;
                return;
        }
    }

}
