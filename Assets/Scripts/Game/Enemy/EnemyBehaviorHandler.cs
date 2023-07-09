using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehaviorHandler : MonoBehaviour
{

    public bool canUseAbility;
    private Enemy _enemyInfo;
    private AIPath _aiPath;
    private EnemyWeaponHandler _enemyWeaponHandler;
    private float _enemyTimer; // Can be used for any means for this specific enemy
    private float _timeSinceLastShot;
    private float _despawnTimer;
    private bool _canMove;
    private HealthHandler _healthHandler;

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _enemyWeaponHandler = GetComponent<EnemyWeaponHandler>();
        _healthHandler = GetComponent<HealthHandler>();
    }

    public void Initialize(Enemy e)
    {
        canUseAbility = true;
        _enemyInfo = e;
        _enemyTimer = 0;
        _aiPath.maxSpeed = e.moveSpeed;
        _aiPath.rotationSpeed = e.swivelSpeed;
        _timeSinceLastShot = Time.time;
        _canMove = true;
        _despawnTimer = 0;
        switch (_enemyInfo.type)
        {
            case EnemyType.STAR:
                transform.rotation = Quaternion.identity;
                _aiPath.enableRotation = false;
                return;
            case EnemyType.BEETLE:
                _healthHandler.OnHurt.AddListener(BeetleStunOnHurt);
                _aiPath.enableRotation = true;
                return;
            default:
                _aiPath.enableRotation = true;
                return;
        }
    }

    private void Update()
    {
        if (PlayerWeaponHandler.Instance == null) { return; }
        float distanceToPlayer = GetDistanceToPlayer();
        // If the enemy has been stagnant for over ten seconds, despawn it.
        _despawnTimer += Time.deltaTime * ((distanceToPlayer > _enemyInfo.followRange) ? 1 : 0);
        if (_despawnTimer > 8)
        {
            GameController.Instance.ReturnEnemyObjectToPool(gameObject);
        }
        if (distanceToPlayer > _enemyInfo.followRange)
        {
            _aiPath.canMove = false;
            return;
        }
        _despawnTimer = 0; // If the enemy is in the follow range, don't let it despawn.
        switch (_enemyInfo.type)
        {
            case EnemyType.BEE:
                // CUSTOM MOVEMENT HANDLER
                if (distanceToPlayer < _enemyInfo.followDistance)
                {
                    // Disable the AIs own movement code
                    _aiPath.canMove = false;
                    Vector3 nextPosition;
                    Quaternion nextRotation;
                    // Calculate how the AI wants to move
                    _aiPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
                    // Modify nextPosition and nextRotation in any way you wish
                    // Actually move the AI
                    _aiPath.FinalizeMovement(transform.position, nextRotation);
                }
                else
                {
                    _aiPath.canMove = _canMove;
                }

                // SHOOTING HANDLER
                if (distanceToPlayer < _enemyInfo.shootingRange)
                {
                    if (Time.time - _timeSinceLastShot >= _enemyInfo.delayBetweenShots)
                    {
                        _timeSinceLastShot = Time.time;
                        _enemyWeaponHandler.ShootBullet();
                    }
                }

                // ABILITY HANDLER (BEE)
                if (canUseAbility && distanceToPlayer < _enemyInfo.abilityRange)
                {
                    // Accumulate time while the player is within the ability range
                    _enemyTimer += Time.deltaTime;
                    // Only allow bee to use this ability once
                    if (_enemyTimer > 5)
                    {
                        _enemyTimer = 0;
                        canUseAbility = false;
                        StartCoroutine(BeeShipAlarmCoroutine());
                    }
                }

                return;

            case EnemyType.STAR:
                // CUSTOM MOVEMENT HANDLER
                if (distanceToPlayer < _enemyInfo.followDistance)
                {
                    // Disable the AIs own movement code
                    _aiPath.canMove = false;
                }
                else
                {
                    _aiPath.canMove = _canMove;
                }

                // SHOOTING HANDLER
                if (distanceToPlayer < _enemyInfo.shootingRange)
                {
                    if (Time.time - _timeSinceLastShot >= _enemyInfo.delayBetweenShots)
                    {
                        _timeSinceLastShot = Time.time;
                        for (int i = 0; i < 8; i++)
                        {
                            _enemyWeaponHandler.ShootBullet(i * 45);
                        }
                    }
                }

                return;

            case EnemyType.BEETLE:

                // CUSTOM MOVEMENT HANDLER
                if (distanceToPlayer < _enemyInfo.followDistance)
                {
                    // Disable the AIs own movement code
                    _aiPath.canMove = false;
                }
                else
                {
                    _aiPath.canMove = _canMove;
                }

                // SHOOTING HANDLER
                if (distanceToPlayer < _enemyInfo.shootingRange)
                {
                    if (Time.time - _timeSinceLastShot >= _enemyInfo.delayBetweenShots)
                    {
                        _timeSinceLastShot = Time.time;
                        _enemyWeaponHandler.ShootBullet();
                    }
                }

                return;

            case EnemyType.BAT:

                // CUSTOM MOVEMENT HANDLER
                if (distanceToPlayer < _enemyInfo.followDistance)
                {
                    // Disable the AIs own movement code
                    _aiPath.canMove = false;
                    Vector3 nextPosition;
                    Quaternion nextRotation;
                    // Calculate how the AI wants to move
                    _aiPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
                    // Modify nextPosition and nextRotation in any way you wish
                    // Actually move the AI
                    _aiPath.FinalizeMovement(transform.position, nextRotation);
                }
                else
                {
                    _aiPath.canMove = _canMove;
                }

                // SHOOTING HANDLER
                if (distanceToPlayer < _enemyInfo.shootingRange)
                {
                    Vector2 toPlayer = PlayerWeaponHandler.Instance.transform.position - transform.position;
                    toPlayer.Normalize();

                    Vector2 enemyForward = transform.up;

                    float angle = Vector2.Angle(toPlayer, enemyForward);
                    if (angle < 30 && Time.time - _timeSinceLastShot >= _enemyInfo.delayBetweenShots)
                    {
                        _timeSinceLastShot = Time.time;
                        _enemyWeaponHandler.ShootBullet();
                    }
                }

                return;
        }
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, PlayerMovementHandler.Instance.transform.position);
    }

    private IEnumerator BeeShipAlarmCoroutine()
    {
        SoundManager.Instance.PlayOneShot(AudioType.BEE_SHIP_ALARM);
        for (int i = 0; i < 4; i++)
        {
            yield return _healthHandler.FlashColorCoroutine(new Color(0.95f, 0.5f, 0.03f));
        }
        yield return new WaitForSeconds(0.5f);
        int enemiesSuccessfullySpawned = 0;
        // Attempt to spawn enemies more times than the number of enemies we want
        for (int i = 0; i < 10; i++)
        {
            if (enemiesSuccessfullySpawned == 2) { break; }
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-4, 4), Random.Range(-4, 4), 0);
            if (EnemySpawnerController.Instance.AttemptSpawnEnemyAtLocation(spawnPosition, EnemyType.BEE, false))
            {
                enemiesSuccessfullySpawned++;
            }
        }
    }

    private void BeetleStunOnHurt()
    {
        StartCoroutine(BeetleStunCoroutine());
    }

    private IEnumerator BeetleStunCoroutine()
    {
        _canMove = false;
        yield return new WaitForSeconds(0.1f);
        _canMove = true;
    }

}
