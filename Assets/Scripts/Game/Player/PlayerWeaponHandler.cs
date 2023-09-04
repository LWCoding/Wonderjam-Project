using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum WeaponType
{
    BASIC
}

public class PlayerWeaponHandler : MonoBehaviour
{

    public static PlayerWeaponHandler Instance;
    [Header("Object Assignments")]
    public WeaponType weaponType;
    public Transform bulletSpawnTransform;
    [Header("Compile-Time Variables")]
    public float bulletSpeed;
    public float bulletLifetime;
    public float turretSwivelSpeed;
    public float delayBetweenShots;
    public float bulletSpreadAngle;
    public float bulletAoERadius;
    private CircleCollider2D _circCol;
    private GameObject _turretObject;
    private Vector2 _turretObjectInitialScale;
    private float _timeSinceLastShot = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _circCol = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        _turretObject = PlayerController.Instance.turretObject;
        _turretObjectInitialScale = _turretObject.transform.localScale;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    // Rotate the turret slowly towards the mouse position using the
    // RotateTowards method on Quaternions.
    private void Update()
    {
        Vector3 mousePos = GetWorldPositionOnPlane(Input.mousePosition, 0) - transform.position;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        if (90 < Mathf.Abs(angle) && Mathf.Abs(angle) < 180)
        {
            _turretObject.transform.localScale = new Vector3(_turretObjectInitialScale.x, -_turretObjectInitialScale.y, 1);
        }
        else
        {
            _turretObject.transform.localScale = new Vector3(_turretObjectInitialScale.x, _turretObjectInitialScale.y, 1);
        }
        _turretObject.transform.rotation = Quaternion.RotateTowards(_turretObject.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), turretSwivelSpeed * Time.deltaTime);
    }

    public void ShootBullet()
    {
        if (Time.time - _timeSinceLastShot < delayBetweenShots) { return; }
        _timeSinceLastShot = Time.time;
        switch (weaponType)
        {
            case WeaponType.BASIC:
                SoundManager.Instance.PlayOneShot(AudioType.PLAYER_BULLET_SHOOT);
                SpawnBullet();
                break;
        }
    }

    private void SpawnBullet(float rotationDiff = 0)
    {
        GameObject bulletObj = GameController.Instance.GetPooledBulletObject(bulletSpawnTransform.position, _turretObject.transform.rotation);
        bulletObj.transform.Rotate(0, 0, Random.Range(-bulletSpreadAngle, bulletSpreadAngle));
        bulletObj.transform.Rotate(0, 0, rotationDiff);
        bulletObj.GetComponent<BulletHandler>().BulletMove(bulletSpeed, bulletLifetime, new Color(0.64f, 1, 0.9f), "Player", _circCol);
        if (bulletAoERadius > 0)
        {
            bulletObj.GetComponent<BulletHandler>().OnDestroy += ((bulletPos) =>
            {
                GameController.Instance.GetPooledBulletAoEObject(bulletPos, bulletAoERadius);
            });
        }
    }

}
