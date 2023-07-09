using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Alliance
{
    PLAYER, ENEMY
}

public class GameController : MonoBehaviour
{

    public static GameController Instance;
    [Header("Object Assignments")]
    public Transform pooledParentTransform;
    public GameObject bulletPrefabObject;
    public GameObject particleExplosionPrefabObject;
    public GameObject enemyPrefabObject;
    public GameObject xpOrbPrefabObject;
    public GameObject bulletAoEPrefabObject;
    private List<GameObject> _pooledBulletObjects = new List<GameObject>();
    private List<GameObject> _pooledBulletExplosionObjects = new List<GameObject>();
    private List<GameObject> _pooledEnemyObjects = new List<GameObject>();
    private List<GameObject> _pooledXPOrbObjects = new List<GameObject>();
    private List<GameObject> _pooledBulletAoEObjects = new List<GameObject>();
    private int _totalEnemiesPooled = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Application.targetFrameRate = 60; // In case it wasn't set in the title, for some reason.
    }

    private void Start()
    {
        SoundManager.Instance.ResetVolumeToDefault();
        SoundManager.Instance.PlayOnLoop(AudioType.MAIN_GAME_MUSIC);
    }

    public void AddPooledBulletObject()
    {
        GameObject obj = Instantiate(bulletPrefabObject);
        obj.SetActive(false);
        _pooledBulletObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public GameObject GetPooledBulletObject(Vector3 positionToSpawnAt, Quaternion rotationToSpawnAt)
    {
        if (_pooledBulletObjects.Count == 0)
        {
            AddPooledBulletObject();
        }
        GameObject obj = _pooledBulletObjects[0];
        obj.transform.position = positionToSpawnAt;
        obj.transform.rotation = rotationToSpawnAt;
        _pooledBulletObjects.RemoveAt(0);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnBulletObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pooledBulletObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public void AddPooledExplosionObject()
    {
        GameObject obj = Instantiate(particleExplosionPrefabObject);
        obj.SetActive(false);
        _pooledBulletExplosionObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public GameObject GetPooledExplosionObject(Vector3 positionToSpawnAt, float particleScale)
    {
        if (_pooledBulletExplosionObjects.Count == 0)
        {
            AddPooledExplosionObject();
        }
        GameObject obj = _pooledBulletExplosionObjects[0];
        _pooledBulletExplosionObjects.RemoveAt(0);
        obj.transform.position = positionToSpawnAt;
        obj.transform.localScale = new Vector2(particleScale, particleScale);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnExplosionObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pooledBulletExplosionObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public void AddPooledEnemyObject()
    {
        GameObject obj = Instantiate(enemyPrefabObject);
        _totalEnemiesPooled++;
        obj.SetActive(false);
        _pooledEnemyObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public GameObject GetPooledEnemyObject()
    {
        if (_pooledEnemyObjects.Count == 0)
        {
            AddPooledEnemyObject();
        }
        GameObject obj = _pooledEnemyObjects[0];
        _pooledEnemyObjects.RemoveAt(0);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnEnemyObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pooledEnemyObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public int GetNumberOfAliveEnemies()
    {
        return _totalEnemiesPooled - _pooledEnemyObjects.Count;
    }

    public void AddPooledXPOrbObject()
    {
        GameObject obj = Instantiate(xpOrbPrefabObject);
        obj.SetActive(false);
        _pooledXPOrbObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public GameObject GetPooledXPOrbObject()
    {
        if (_pooledXPOrbObjects.Count == 0)
        {
            AddPooledXPOrbObject();
        }
        GameObject obj = _pooledXPOrbObjects[0];
        _pooledXPOrbObjects.RemoveAt(0);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnXPOrbObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pooledXPOrbObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public void AddPooledBulletAoEObject()
    {
        GameObject obj = Instantiate(bulletAoEPrefabObject);
        obj.SetActive(false);
        _pooledBulletAoEObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

    public GameObject GetPooledBulletAoEObject(Vector3 positionToSpawnAt, float radius)
    {
        if (_pooledBulletAoEObjects.Count == 0)
        {
            AddPooledBulletAoEObject();
        }
        GameObject obj = _pooledBulletAoEObjects[0];
        _pooledBulletAoEObjects.RemoveAt(0);
        obj.transform.position = positionToSpawnAt;
        obj.SetActive(true);
        obj.GetComponent<BulletAoEHandler>().Initialize(radius);
        return obj;
    }

    public void ReturnBulletAoEObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        _pooledBulletAoEObjects.Add(obj);
        obj.transform.SetParent(pooledParentTransform);
    }

}
