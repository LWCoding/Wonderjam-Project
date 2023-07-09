using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHandler : MonoBehaviour
{
    [Header("Object Assignments")]
    public Transform bulletSpawnTransform;
    private EnemyController _enemyController;
    private CircleCollider2D _circCol;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
        _circCol = GetComponent<CircleCollider2D>();
    }

    public void ShootBullet(float rotationDiff = 0)
    {
        SoundManager.Instance.PlayOneShot(AudioType.ENEMY_BULLET_SHOOT, 0.35f);
        SpawnBullet(90 + rotationDiff);
    }

    private void SpawnBullet(float rotationDiff)
    {
        GameObject bulletObj = GameController.Instance.GetPooledBulletObject(bulletSpawnTransform.position, _enemyController.spaceshipObject.transform.rotation);
        bulletObj.transform.Rotate(0, 0, Random.Range(-_enemyController.bulletSpreadAngle, _enemyController.bulletSpreadAngle));
        bulletObj.transform.Rotate(0, 0, rotationDiff);
        bulletObj.GetComponent<BulletHandler>().BulletMove(_enemyController.bulletSpeed, _enemyController.bulletLifetime, new Color(0.75f, 0.5f, 0.13f), "Enemy", _circCol);
    }

}
