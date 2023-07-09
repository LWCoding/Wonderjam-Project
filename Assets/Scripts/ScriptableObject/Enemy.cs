using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    STAR, BEE, BEETLE, BAT
}

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
public class Enemy : ScriptableObject
{

    public EnemyType type;
    public Sprite sprite;
    [Header("Basic Stats")]
    public int maxHealth = 10;
    public int xpDropAmount = 5;
    public int pointsGainedOnKill = 1000;
    public int requiredLevelToSpawn = 0;
    [Header("Behavior")]
    public float moveSpeed = 2.5f;
    public float swivelSpeed = 4;
    public float followDistance = 4f;
    public float followRange = 16f;
    public float shootingRange = 8f;
    public float abilityRange = 0f;
    [Header("Projectiles")]
    public float bulletSpeed = 10;
    public float bulletLifetime = 0.6f;
    public float delayBetweenShots = 0.4f;
    public float bulletSpreadAngle = 10;

}
