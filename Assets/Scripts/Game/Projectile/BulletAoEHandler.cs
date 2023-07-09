using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAoEHandler : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(float radius)
    {
        transform.localScale = new Vector2(radius, radius);
        StartCoroutine(DealDamageAndDisable());
    }

    private IEnumerator DealDamageAndDisable()
    {
        float currTime = 0;
        float timeToWait = 0.8f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        Color initialColor = _spriteRenderer.color;
        Color targetColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        while (currTime <= timeToWait)
        {
            currTime += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(initialColor, targetColor, currTime / timeToWait);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, currTime / timeToWait);
            yield return null;
        }
        _spriteRenderer.color = initialColor; // Reset color so it shows for other instantiations
        GameController.Instance.ReturnBulletAoEObjectToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Only damage the colliding object if it's not the player
        // (AoE is exclusive to the player)
        if (col.GetComponent<PlayerController>() == null)
        {
            col.GetComponent<HealthHandler>()?.TakeDamage(1);
        }
    }

}
