using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Globals
{

    public static List<Enemy> allEnemies = new List<Enemy>();
    public static List<Upgrade> allUpgrades = new List<Upgrade>();
    private static bool globalsInitialized = false;

    private static void Initialize()
    {

        // Find all enemies in the "Enemies" folder (through Resources)
        // and add them to the `allEnemies` variable.
        allEnemies = Resources.LoadAll<Enemy>("ScriptableObjects/Enemies").ToList();

        // Find all upgrades in the "Upgrades" folder (through Resources)
        // and add them to the `allUpgrades` variable.
        allUpgrades = Resources.LoadAll<Upgrade>("ScriptableObjects/Upgrades").ToList();

        // After everything is initialized, set to true.
        globalsInitialized = true;

    }

    // Gets a enemy by name.
    public static Enemy GetEnemy(EnemyType type)
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        Enemy foundEnemy = null;
        allEnemies.ForEach((enemy) =>
        {
            if (enemy.type == type)
            {
                foundEnemy = enemy;
                return;
            }
        });
        if (!foundEnemy)
        {
            Debug.Log("Could not find enemy (" + type + ") in Globals.cs!");
        }
        return foundEnemy;
    }

    public static Enemy GetRandomEnemy()
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        return allEnemies[Random.Range(0, allEnemies.Count)];
    }

    public static Enemy GetRandomEnemyByLevelCap(int currLevel)
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        List<Enemy> possibleEnemies = allEnemies.FindAll((e) => e.requiredLevelToSpawn <= currLevel);
        return possibleEnemies[Random.Range(0, possibleEnemies.Count)];
    }

    // Gets a enemy by name.
    public static Upgrade GetUpgrade(UpgradeType type)
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        Upgrade foundUpgrade = null;
        allUpgrades.ForEach((upgrade) =>
        {
            if (upgrade.type == type)
            {
                foundUpgrade = upgrade;
                return;
            }
        });
        if (!foundUpgrade)
        {
            Debug.Log("Could not find upgrade (" + type + ") in Globals.cs!");
        }
        return foundUpgrade;
    }

    public static Upgrade GetRandomUpgrade(List<Upgrade> upgradeBlacklist)
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        List<Upgrade> possibleUpgrades = allUpgrades.FindAll(ug => !upgradeBlacklist.Contains(ug));
        if (possibleUpgrades.Count == 0)
        {
            Debug.Log("GetRandomUpgrade in Globals.cs returned null because there are no valid upgrades!");
            return null;
        }
        return possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
    }

    public static List<UpgradeType> GetAllUpgradeTypes()
    {
        if (!globalsInitialized)
        {
            Initialize();
        }
        List<UpgradeType> upgradeTypes = new List<UpgradeType>();
        foreach (Upgrade upgrade in allUpgrades)
        {
            upgradeTypes.Add(upgrade.type);
        }
        return upgradeTypes;
    }

}
