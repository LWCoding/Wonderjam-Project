using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TitleController : MonoBehaviour
{

    public static TitleController Instance;
    [Header("Object Assignments")]
    public GameObject enemyPrefabObject;
    public Transform enemyParentTransform;
    public Transform enemyTargetTransform;
    public bool canPlayerInteract = true;

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
        Application.targetFrameRate = 60;
        SoundManager.Instance.ResetVolumeToDefault();
        StartCoroutine(SummonEnemyCoroutine());
    }

    private IEnumerator SummonEnemyCoroutine()
    {
        SummonEnemy();
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(SummonEnemyCoroutine());
    }

    public void SummonEnemy()
    {
        GameObject enemy = GameController.Instance.GetPooledEnemyObject();
        // Initialize enemy information
        Enemy enemyInfo = Globals.GetRandomEnemy();
        enemy.AddComponent<TitleEnemyHandler>();
        enemy.GetComponent<EnemyController>().Initialize(enemyInfo, false);
        // Set transform values
        enemy.transform.SetParent(enemyParentTransform);
        enemy.transform.position = new Vector3(-12.5f, Random.Range(-2, 0), 0);
        enemy.GetComponent<EnemyController>().GetComponent<AIDestinationSetter>().target = enemyTargetTransform;
        StartCoroutine(TrackEnemyDistanceCoroutine(enemy));
    }

    private IEnumerator TrackEnemyDistanceCoroutine(GameObject enemyObj)
    {
        while (Vector3.Distance(enemyObj.transform.position, enemyTargetTransform.position) > 8)
        {
            yield return null;
        }
        enemyObj.GetComponent<HealthHandler>().TakeDamage(999, false);
    }

}
