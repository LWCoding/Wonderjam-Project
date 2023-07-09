using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    [Header("Object Assignments")]
    public GameObject spaceshipObject;
    public GameObject turretObject;
    public GameObject particleSystemObject;
    public int startingHealth;
    private SpriteRenderer _spaceshipSpriteRenderer;
    private SpriteRenderer _turretSpriteRenderer;
    private HealthHandler _healthHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _spaceshipSpriteRenderer = spaceshipObject.GetComponent<SpriteRenderer>();
        _turretSpriteRenderer = turretObject.GetComponent<SpriteRenderer>();
        _healthHandler = GetComponent<HealthHandler>();
        _healthHandler.Initialize(Alliance.PLAYER, startingHealth, _spaceshipSpriteRenderer);
    }

    private void Start()
    {
        _healthHandler.OnKill.AddListener(PlayerDestroyed);
    }

    private void PlayerDestroyed()
    {
        SoundManager.Instance.PlayOneShot(AudioType.ENEMY_SHIP_EXPLODE, 0.7f);
        SoundManager.Instance.PlayOneShot(AudioType.GAME_OVER, 0.5f);
        GameController.Instance.GetPooledExplosionObject(transform.position, 1f);
        GetComponent<PlayerMovementHandler>().enabled = false;
        GetComponent<PlayerWeaponHandler>().enabled = false;
        GetComponent<PlayerXPHandler>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        particleSystemObject.SetActive(false);
        StartCoroutine(ShipFadeAway());
    }

    private IEnumerator ShipFadeAway()
    {
        float currTime = 0;
        float timeToWait = 1.6f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = transform.position - new Vector3(0, 1.5f, 0);
        Color initialColor = Color.white;
        Color targetColor = new Color(1, 1, 1, 0);
        float defaultVolume = SoundManager.Instance.defaultVolume;
        while (currTime < timeToWait)
        {
            SoundManager.Instance.SetVolume(Mathf.Lerp(defaultVolume, 0.3f, currTime / timeToWait));
            transform.position = Vector3.Lerp(initialPosition, targetPosition, currTime / timeToWait);
            _turretSpriteRenderer.color = Color.Lerp(initialColor, targetColor, currTime / timeToWait);
            _spaceshipSpriteRenderer.color = Color.Lerp(initialColor, targetColor, currTime / timeToWait);
            currTime += Time.deltaTime;
            yield return null;
        }
        UIController.Instance.SetHighscoreIfApplicable(); // Update the high score if points > all-time points
        yield return new WaitForSeconds(0.5f);
        OverlayManager.Instance.ShowScreenOverlay(0.7f, 0.8f, () =>
        {
            SceneManager.LoadScene("Title");
        });
    }

}
