using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosionHandler : MonoBehaviour
{

    private ParticleSystem _pSystem;

    private void Awake()
    {
        _pSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        _pSystem.Emit(8);
        StartCoroutine(DisappearAfterParticlesCoroutine());
    }

    private IEnumerator DisappearAfterParticlesCoroutine()
    {
        yield return new WaitForSeconds(1);
        GameController.Instance.ReturnExplosionObjectToPool(gameObject);
    }

}
