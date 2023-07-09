using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnLocation
{
    public float radius;
    public Vector3 position;
}

public class EnemySpawnerController : MonoBehaviour
{

    public static EnemySpawnerController Instance;
    [Header("Object Assignments")]
    public Transform enemyParentTransform;
    [SerializeField] private List<SpawnLocation> enemySpawnLocations;
    private const float _minDistanceFromPlayerToSpawn = 12;
    private const float _maxDistanceFromPlayerToSpawn = 36;
    private float _wavesSpawned = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        int currLevel = PlayerUpgradeHandler.Instance.currLevel;
        // If the # of alive enemies is greater than the player's level minus two (min one), don't spawn more
        if (GameController.Instance.GetNumberOfAliveEnemies() >= Mathf.Max(1, currLevel - 2))
        {
            yield return new WaitForSeconds(2);
            StartCoroutine(SpawnEnemyCoroutine());
            yield break;
        }
        // Or else, spawn in a new wave of enemies proportional to the player's level
        _wavesSpawned++;
        for (int i = 0; i < Mathf.Min(4, Mathf.CeilToInt((float)currLevel / 5)); i++)
        {
            AttemptSpawnEnemyAtRandomLocation();
        }
        yield return new WaitForSeconds(7.5f);
        StartCoroutine(SpawnEnemyCoroutine());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (SpawnLocation loc in enemySpawnLocations)
        {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(loc.position, Vector3.back, loc.radius);
        }
    }
# endif

    private void AttemptSpawnEnemyAtRandomLocation(int attempts = 0)
    {
        // If we've tried too many times to spawn an enemy, just stop.
        if (attempts > 5)
        {
            return;
        }
        // Find a random spawn location and try to spawn the enemy there.
        // Ensure the spawn location is close enough to the player.
        List<SpawnLocation> possibleSpawns = new List<SpawnLocation>();
        Vector3 playerPosition = PlayerMovementHandler.Instance.transform.position;
        enemySpawnLocations.ForEach((loc) =>
        {
            if (Vector3.Distance(loc.position, playerPosition) < _maxDistanceFromPlayerToSpawn)
            {
                possibleSpawns.Add(loc);
            }
        });
        if (possibleSpawns.Count == 0) { return; }
        SpawnLocation spawnLoc = possibleSpawns[Random.Range(0, possibleSpawns.Count)];
        Vector3 attemptedPosition = spawnLoc.position;
        attemptedPosition += new Vector3(Random.Range(-spawnLoc.radius, spawnLoc.radius),
                                        Random.Range(-spawnLoc.radius, spawnLoc.radius),
                                        0);
        EnemyType randomEnemyType = Globals.GetRandomEnemyByLevelCap(PlayerUpgradeHandler.Instance.currLevel).type;
        float distanceFromPlayer = Vector3.Distance(attemptedPosition, playerPosition);
        // If this area is too close to the player, or an enemy can't be spawned, try again.
        if (distanceFromPlayer < _minDistanceFromPlayerToSpawn)
        {
            AttemptSpawnEnemyAtRandomLocation(attempts + 1);
        }
        else if (!AttemptSpawnEnemyAtLocation(attemptedPosition, randomEnemyType))
        {
            AttemptSpawnEnemyAtRandomLocation(attempts + 1);
        }
    }

    public bool AttemptSpawnEnemyAtLocation(Vector3 spawnPosition, EnemyType type, bool canUseAbility = true)
    {
        if (Physics2D.OverlapPoint(spawnPosition))
        {
            return false;
        }
        SpawnEnemyAtLocation(spawnPosition, type, canUseAbility);
        return true;
    }

    public void SpawnEnemyAtLocation(Vector3 spawnPosition, EnemyType type, bool canUseAbility)
    {
        GameObject enemyObject = GameController.Instance.GetPooledEnemyObject();
        enemyObject.transform.position = spawnPosition;
        enemyObject.transform.SetParent(enemyParentTransform);
        Enemy enemyInfo = Globals.GetEnemy(type);
        enemyObject.GetComponent<EnemyController>().Initialize(enemyInfo);
        enemyObject.GetComponent<EnemyBehaviorHandler>().canUseAbility = canUseAbility;
    }

    public EnemyType GetRandomEnemyType()
    {
        return Globals.GetRandomEnemy().type;
    }

}
