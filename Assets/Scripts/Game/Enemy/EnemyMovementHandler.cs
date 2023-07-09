using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementHandler : MonoBehaviour
{

    private HealthHandler _healthHandler;


    private void Awake()
    {
        _healthHandler = GetComponent<HealthHandler>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "UnfriendlyProjectile")
        {
            return;
        }
        float bounce = 50f;
        if (collision.gameObject.tag == "FriendlyProjectile" || collision.gameObject.tag == "NeutralProjectile")
        {
            _healthHandler.TakeDamage(1);
        }
        GetComponent<Rigidbody2D>().AddForce(collision.contacts[0].normal * bounce);
    }

}
