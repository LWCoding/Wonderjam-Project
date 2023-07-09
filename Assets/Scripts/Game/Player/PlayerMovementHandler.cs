using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovementHandler : MonoBehaviour
{

    public static PlayerMovementHandler Instance;
    private Rigidbody2D rb;
    [SerializeField] private float _moveSpeed = 3.5f;
    private HealthHandler _healthHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        _healthHandler = GetComponent<HealthHandler>();
    }

    private void Update()
    {
        // Pressing WASD or arrow keys moves the character.
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity -= new Vector2(_moveSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity += new Vector2(_moveSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity += new Vector2(0, _moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity -= new Vector2(0, _moveSpeed * Time.deltaTime);
        }
        // Pressing space shoots a bullet.
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            PlayerWeaponHandler.Instance.ShootBullet();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "FriendlyProjectile")
        {
            return;
        }
        float bounce = 50f;
        // If the player hits a wall, damage them.
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "BreakableWall")
        {
            _healthHandler.TakeDamage(1);
            GameController.Instance.GetPooledExplosionObject(transform.position, 0.6f);
            bounce += 75f;
        }
        // If the wall is breakable, damage it!
        if (collision.gameObject.tag == "BreakableWall")
        {
            Vector3 hitPosition = Vector3.zero;
            Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
            ContactPoint2D hit = collision.contacts[0];
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            if (collision.gameObject.tag == "BreakableWall")
            {
                DestructibleTileHandler.Instance.ShootTileAt(tilemap.WorldToCell(hitPosition), 5);
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
        // If the player is hit by an enemy bullet, damage the player.
        if (collision.gameObject.tag == "UnfriendlyProjectile" || collision.gameObject.tag == "NeutralProjectile")
        {
            _healthHandler.TakeDamage(1);
        }
        rb.AddForce(collision.contacts[0].normal * bounce);
    }

}
