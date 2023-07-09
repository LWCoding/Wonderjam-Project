using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    ENEMY_BULLET_SHOOT, ENEMY_SHIP_DAMAGED, PLAYER_BULLET_SHOOT, PLAYER_SHIP_DAMAGED,
    ENEMY_SHIP_EXPLODE, MAIN_GAME_MUSIC, BEE_SHIP_ALARM, XP_ORB_PICKUP, GAME_OVER,
    LEVEL_UP, UI_HOVER, UI_CLICK, X_BLOCK_BREAK, X_BLOCK_HIT, BOOM_BOX_EXPLODE
}

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;
    [Header("Music Assignments")]
    public AudioClip gameMusic;
    [Header("Audio Assignments")]
    public AudioClip enemyBulletShootSFX;
    public AudioClip enemyShipDamagedSFX;
    public AudioClip enemyShipExplodeSFX;
    public AudioClip playerBulletShootSFX;
    public AudioClip playerShipDamagedSFX;
    public AudioClip beeShipAlarmSFX;
    public AudioClip xpOrbPickupSFX;
    public AudioClip gameOverSFX;
    public AudioClip levelUpSFX;
    public AudioClip uiHoverSFX;
    public AudioClip uiClickSFX;
    public AudioClip xBlockBreakSFX;
    public AudioClip xBlockHitSFX;
    public AudioClip boomBoxExplodeSFX;
    private AudioSource _audioSource;
    public float defaultVolume = 0.8f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        _audioSource = GetComponent<AudioSource>();
        ResetVolumeToDefault();
    }

    public void PlayOnLoop(AudioType type)
    {
        switch (type)
        {
            case AudioType.MAIN_GAME_MUSIC:
                _audioSource.clip = gameMusic;
                _audioSource.loop = true;
                _audioSource.Play();
                return;
        }
    }

    public void ResetVolumeToDefault()
    {
        _audioSource.volume = defaultVolume;
    }

    public void SetVolume(float volume)
    {
        _audioSource.volume = volume;
    }

    public void PlayOneShot(AudioType type, float volume = 0.5f)
    {
        switch (type)
        {
            case AudioType.ENEMY_BULLET_SHOOT:
                _audioSource.PlayOneShot(enemyBulletShootSFX, volume);
                return;
            case AudioType.ENEMY_SHIP_DAMAGED:
                _audioSource.PlayOneShot(enemyShipDamagedSFX, volume);
                return;
            case AudioType.ENEMY_SHIP_EXPLODE:
                _audioSource.PlayOneShot(enemyShipExplodeSFX, volume);
                return;
            case AudioType.PLAYER_BULLET_SHOOT:
                _audioSource.PlayOneShot(playerBulletShootSFX, volume);
                return;
            case AudioType.PLAYER_SHIP_DAMAGED:
                _audioSource.PlayOneShot(playerShipDamagedSFX, volume);
                return;
            case AudioType.BEE_SHIP_ALARM:
                _audioSource.PlayOneShot(beeShipAlarmSFX, volume);
                return;
            case AudioType.XP_ORB_PICKUP:
                _audioSource.PlayOneShot(xpOrbPickupSFX, volume);
                return;
            case AudioType.GAME_OVER:
                _audioSource.PlayOneShot(gameOverSFX, volume);
                return;
            case AudioType.LEVEL_UP:
                _audioSource.PlayOneShot(levelUpSFX, volume);
                return;
            case AudioType.UI_HOVER:
                _audioSource.PlayOneShot(uiHoverSFX, volume);
                return;
            case AudioType.UI_CLICK:
                _audioSource.PlayOneShot(uiClickSFX, volume);
                return;
            case AudioType.X_BLOCK_BREAK:
                _audioSource.PlayOneShot(xBlockBreakSFX, volume);
                return;
            case AudioType.X_BLOCK_HIT:
                _audioSource.PlayOneShot(xBlockHitSFX, volume);
                return;
            case AudioType.BOOM_BOX_EXPLODE:
                _audioSource.PlayOneShot(boomBoxExplodeSFX, volume);
                return;
        }
    }

}
