using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXPHandler : MonoBehaviour
{

    public static PlayerXPHandler Instance;
    [HideInInspector] public int upgradesToRender = 0;
    private float _currentXP = 0;
    private int _xpToNextLevel = 5;
    private const int MaxXpToNextLevel = 80;
    private HealthHandler _healthHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _healthHandler = GetComponent<HealthHandler>();
    }

    public void GainXP(float xpAmount)
    {
        _currentXP += xpAmount;
        bool leveledUp = false;
        while (_currentXP >= _xpToNextLevel)
        {
            if (PlayerUpgradeHandler.Instance.GetImpossibleUpgrades().Count == Globals.allUpgrades.Count)
            {
                UIController.Instance.SetGoldXPScaleColor();
                return;
            }
            _currentXP -= _xpToNextLevel;
            _xpToNextLevel = (int)((float)_xpToNextLevel * 1.4f);
            if (_xpToNextLevel > MaxXpToNextLevel)
            {
                _xpToNextLevel = MaxXpToNextLevel;
            }
            upgradesToRender++;
            leveledUp = true;
        }
        if (leveledUp)
        {
            PromptUpgradeChoice();
        }
        UIController.Instance.SetXPScaleByPercentage((float)_currentXP / _xpToNextLevel);
    }

    private void PromptUpgradeChoice()
    {
        SoundManager.Instance.PlayOneShot(AudioType.LEVEL_UP, 0.5f);
        OverlayManager.Instance.ShowScreenOverlayWithLowSortingOrder(0.6f, 0.6f, () =>
        {
            UIController.Instance.AllowPlayerToChooseUpgrade();
        });
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "XPOrb")
        {
            GainXP(col.gameObject.GetComponent<XPOrbHandler>().xpValue);
            GameController.Instance.ReturnXPOrbObjectToPool(col.gameObject);
            SoundManager.Instance.PlayOneShot(AudioType.XP_ORB_PICKUP, 0.5f);
            _healthHandler.FlashColor(new Color(0.8f, 1f, 0.8f));
        }
    }

}
