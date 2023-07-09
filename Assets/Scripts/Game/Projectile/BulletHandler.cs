using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class BulletHandler : MonoBehaviour
{

    public string fTag;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _ownCollider;
    private IEnumerator _bulletMoveCoroutine = null;
    public delegate void OnDestroyDelegate(Vector3 position);
    public OnDestroyDelegate OnDestroy;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ownCollider = GetComponent<Collider2D>();
    }

    public void BulletMove(float bulletSpeed, float bulletLifetime, Color bulletColor, string friendlyTag, Collider2D colToIgnore = null)
    {
        OnDestroy = null;
        _spriteRenderer.color = bulletColor;
        fTag = friendlyTag;
        if (colToIgnore != null)
        {
            Physics2D.IgnoreCollision(colToIgnore, _ownCollider);
        }
        Debug.Assert(friendlyTag == "Player" || friendlyTag == "Enemy" || friendlyTag == "Neutral");
        if (fTag == "Player")
        {
            gameObject.tag = "FriendlyProjectile";
        }
        else if (fTag == "Enemy")
        {
            gameObject.tag = "UnfriendlyProjectile";
        }
        else
        {
            gameObject.tag = "NeutralProjectile";
        }
        if (_bulletMoveCoroutine != null)
        {
            StopCoroutine(_bulletMoveCoroutine);
        }
        _bulletMoveCoroutine = BulletMoveCoroutine(bulletSpeed, bulletLifetime);
        StartCoroutine(_bulletMoveCoroutine);
    }

    private IEnumerator BulletMoveCoroutine(float bulletSpeed, float bulletLifetime)
    {
        float startTime = Time.time;
        while (Time.time - startTime < bulletLifetime)
        {
            transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
            yield return null;
        }
        GameController.Instance.ReturnBulletObjectToPool(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == fTag || collision.gameObject.tag == gameObject.tag)
        {
            Physics2D.IgnoreCollision(collision.collider, _ownCollider);
            return;
        }
        if (collision.gameObject.tag == "BreakableWall" || collision.gameObject.tag == "ExplodeableWall")
        {
            Vector3 hitPosition = Vector3.zero;
            Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
            ContactPoint2D hit = collision.contacts[0];
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            if (collision.gameObject.tag == "BreakableWall")
            {
                DestructibleTileHandler.Instance.ShootTileAt(tilemap.WorldToCell(hitPosition));
            }
            else
            {
                tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
                int numBullets = 7;
                for (int i = 0; i < numBullets; i++)
                {
                    GameObject bulletObj = GameController.Instance.GetPooledBulletObject(hitPosition, Quaternion.Euler(0, 0, i * (360 / numBullets)));
                    bulletObj.GetComponent<BulletHandler>().BulletMove(8, 1.5f, new Color(0.75f, 0.5f, 0.13f), "Neutral");
                    SoundManager.Instance.PlayOneShot(AudioType.BOOM_BOX_EXPLODE, 0.08f);
                }
            }
        }
        OnDestroy?.Invoke(transform.position);
        GameController.Instance.GetPooledExplosionObject(transform.position, 0.3f);
        GameController.Instance.ReturnBulletObjectToPool(gameObject);
    }

}
