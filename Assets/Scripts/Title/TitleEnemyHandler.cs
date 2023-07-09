using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleEnemyHandler : MonoBehaviour
{

    private HealthHandler _healthHandler;

    private void Awake()
    {
        _healthHandler = GetComponent<HealthHandler>();
    }

    private void OnMouseDown()
    {

        GameController.Instance.GetPooledExplosionObject((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition)), 0.4f);
        _healthHandler.TakeDamage(1);
    }

}
