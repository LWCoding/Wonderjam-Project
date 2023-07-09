using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{

    [Header("Object Assignments")]
    public GameObject spaceshipObject;
    [Header("Compile-Time Variables")]
    public float bulletSpeed;
    public float bulletLifetime;
    public float swivelSpeed;
    public float delayBetweenShots;
    public float bulletSpreadAngle;
    public float initialRotation;
    public float moveSpeed;
    private SpriteRenderer _spaceshipSpriteRenderer;
    private EnemyBehaviorHandler _enemyBehaviorHandler;
    private HealthHandler _healthHandler;
    private const float _xpPositionVariation = 1.2f;

    private void Awake()
    {
        _spaceshipSpriteRenderer = spaceshipObject.GetComponent<SpriteRenderer>();
        _healthHandler = GetComponent<HealthHandler>();
        _enemyBehaviorHandler = GetComponent<EnemyBehaviorHandler>();
        if (PlayerMovementHandler.Instance == null) { return; }
        GetComponent<AIDestinationSetter>().target = PlayerMovementHandler.Instance.gameObject.transform;
    }

    public void Initialize(Enemy enemyInfo, bool dropXPAndPoints = true)
    {
        bulletSpeed = enemyInfo.bulletSpeed;
        bulletLifetime = enemyInfo.bulletLifetime;
        swivelSpeed = enemyInfo.swivelSpeed;
        delayBetweenShots = enemyInfo.delayBetweenShots;
        bulletSpreadAngle = enemyInfo.bulletSpreadAngle;
        _spaceshipSpriteRenderer.sprite = enemyInfo.sprite;
        _healthHandler.Initialize(Alliance.ENEMY, enemyInfo.maxHealth, _spaceshipSpriteRenderer);
        _healthHandler.OnKill.AddListener(() =>
        {
            EnemyDestroyed();
            if (dropXPAndPoints)
            {
                PointsGain(enemyInfo.pointsGainedOnKill);
                XPGain(enemyInfo.xpDropAmount);
            }
        });
        _enemyBehaviorHandler.Initialize(enemyInfo);
    }

    private void XPGain(float amt)
    {
        int numOrbs = Mathf.Clamp(3, Mathf.CeilToInt(amt / 3), 8);
        for (int i = 0; i < numOrbs; i++)
        {
            GameObject obj = GameController.Instance.GetPooledXPOrbObject();
            obj.transform.position = transform.position + new Vector3(Random.Range(-_xpPositionVariation, _xpPositionVariation),
                                                                    Random.Range(-_xpPositionVariation, _xpPositionVariation), 0);
            obj.GetComponent<XPOrbHandler>().xpValue = amt / numOrbs;
        }
    }

    private void PointsGain(int amt)
    {
        if (UIController.Instance == null) { return; }
        UIController.Instance.UpdatePointsText(amt);
    }

    private void EnemyDestroyed()
    {
        GameController.Instance.GetPooledExplosionObject(transform.position, 0.7f);
        GameController.Instance.ReturnEnemyObjectToPool(gameObject);
    }

}
