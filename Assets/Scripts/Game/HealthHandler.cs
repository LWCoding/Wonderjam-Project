using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{

    public UnityEvent OnHurt;
    public UnityEvent OnKill;
    private Alliance alliance;
    private SpriteRenderer spaceshipSpriteRenderer;
    [SerializeField] private int _currentHealth;
    private int _maxHealth;
    private IEnumerator _hurtAnimationCoroutine = null;
    private Color _damagedColor = new Color(1, 0.5f, 0.5f);
    private const float _invincibilityTime = 0.1f;
    private float _lastDamageTakenTime;

    public void Initialize(Alliance alliance, int maxHealth, SpriteRenderer spriteRendererToAnimate)
    {
        OnHurt = new UnityEvent();
        OnKill = new UnityEvent();
        this.alliance = alliance;
        this._maxHealth = maxHealth;
        this._currentHealth = maxHealth;
        this.spaceshipSpriteRenderer = spriteRendererToAnimate;
        spaceshipSpriteRenderer.color = Color.white;
    }

    public void TakeDamage(int amount, bool playSound = true)
    {
        if (Time.time - _lastDamageTakenTime < _invincibilityTime)
        {
            return;
        }
        _lastDamageTakenTime = Time.time;
        // Subtract damage from health.
        _currentHealth -= amount;
        // Invoke any events that must happen.
        OnHurt.Invoke();
        // Play sound when damaged.
        if (playSound) { SoundManager.Instance.PlayOneShot((alliance == Alliance.PLAYER) ? AudioType.PLAYER_SHIP_DAMAGED : AudioType.ENEMY_SHIP_DAMAGED, 0.2f); }
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            if (playSound) { SoundManager.Instance.PlayOneShot(AudioType.ENEMY_SHIP_EXPLODE, 0.5f); }
            OnKill.Invoke();
        }
        // Update UI if this is the player.
        if (alliance == Alliance.PLAYER)
        {
            UIController.Instance.SetHealthScaleByPercentage((float)_currentHealth / _maxHealth);
        }
        // Flash player if they aren't DEAD
        if (_currentHealth != 0)
        {
            FlashColor(_damagedColor);
        }
        else
        {
            if (_hurtAnimationCoroutine != null)
            {
                StopCoroutine(_hurtAnimationCoroutine);
            }
        }
    }

    public void HealHealth(int amount)
    {
        _currentHealth += amount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void FlashColor(Color c)
    {
        // Play the basic hurt animation coroutine.
        if (_hurtAnimationCoroutine != null)
        {
            StopCoroutine(_hurtAnimationCoroutine);
        }
        _hurtAnimationCoroutine = FlashColorCoroutine(c);
        StartCoroutine(_hurtAnimationCoroutine);
    }

    public IEnumerator FlashColorCoroutine(Color c)
    {
        float currTime = 0;
        float timeToWait = 0.8f;
        while (currTime < timeToWait)
        {
            spaceshipSpriteRenderer.color = Color.Lerp(c, Color.white, currTime / timeToWait);
            currTime += Time.deltaTime;
            yield return null;
        }
    }

}
