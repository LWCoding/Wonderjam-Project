using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{

    public static UIController Instance;
    [Header("Object Assignments")]
    public GameObject upgradeUIPrefabObject;
    public Transform upgradeParentTransform;
    public CanvasGroup upgradeCanvasGroup;
    public TextMeshProUGUI pointsText;
    public Transform healthFillTransform;
    public Transform xpFillTransform;
    public Transform xpOverlayFillTransform;
    private Vector3 _initialXPFillScale;
    private Vector3 _initialHealthFillScale;
    private int _totalPoints = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _initialHealthFillScale = healthFillTransform.localScale;
        _initialXPFillScale = xpFillTransform.localScale;
        upgradeCanvasGroup.gameObject.SetActive(false);
        UpdatePointsText();
        SetHealthScaleByPercentage(1);
        SetXPScaleByPercentage(0);
    }

    public void SetHealthScaleByPercentage(float healthPercentage)
    {
        healthFillTransform.transform.localScale = new Vector3(_initialHealthFillScale.x * healthPercentage, _initialHealthFillScale.y, 0);
    }

    public void SetXPScaleByPercentage(float ammoPercentage)
    {
        xpFillTransform.transform.localScale = new Vector3(_initialXPFillScale.x * ammoPercentage, _initialXPFillScale.y, 0);
    }

    public void SetHighscoreIfApplicable()
    {
        if (_totalPoints > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", _totalPoints);
        }
    }

    public void UpdatePointsText(int newPoints = 0)
    {
        _totalPoints += newPoints;
        pointsText.text = "PTS: " + _totalPoints.ToString();
    }

    public void SetGoldXPScaleColor()
    {
        xpFillTransform.GetComponent<Image>().color = new Color(0.88f, 0.7f, 0.19f);
        xpOverlayFillTransform.GetComponent<Image>().color = new Color(0.94f, 0.88f, 0.02f);
    }

    public void AllowPlayerToChooseUpgrade()
    {
        StartCoroutine(AllowPlayerToChooseUpgradeCoroutine());
    }

    private IEnumerator AllowPlayerToChooseUpgradeCoroutine()
    {
        float currTime = 0;
        float timeToWait = 0.5f;
        upgradeCanvasGroup.gameObject.SetActive(true);
        // Make sure upgrades that are already maxed can't be chosen
        List<Upgrade> blacklistedUpgrades = PlayerUpgradeHandler.Instance.GetImpossibleUpgrades();
        UpgradeChoiceHandler.areUpgradesInteractable = true;
        // Destroy all of the current upgrades (that may have been instantiated in the past)
        for (int i = 0; i < upgradeParentTransform.childCount; i++)
        {
            Destroy(upgradeParentTransform.GetChild(i).gameObject);
        }
        // Initialize all upgrades
        int cardsToInstantiate = Mathf.Min(3, Globals.allUpgrades.Count - blacklistedUpgrades.Count);
        for (int i = 0; i < cardsToInstantiate; i++)
        {
            GameObject upgradeObject = Instantiate(upgradeUIPrefabObject);
            upgradeObject.transform.SetParent(upgradeParentTransform);
            Upgrade randomUpgrade = Globals.GetRandomUpgrade(blacklistedUpgrades);
            blacklistedUpgrades.Add(randomUpgrade);
            upgradeObject.GetComponent<UpgradeChoiceHandler>().Initialize(randomUpgrade);
        }
        // Make time slow down and show upgrades
        while (currTime < timeToWait)
        {
            currTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1, 0, currTime / timeToWait);
            upgradeCanvasGroup.alpha = Mathf.Lerp(0, 1, currTime / timeToWait);
            yield return null;
        }
        Time.timeScale = 0;
    }

    public void HideUpgrades()
    {
        StartCoroutine(HideUpgradesCoroutine());
    }

    private IEnumerator HideUpgradesCoroutine()
    {
        float currTime = 0;
        float timeToWait = 0.8f;
        OverlayManager.Instance.HideScreenOverlay(timeToWait - 0.2f);
        while (currTime < timeToWait)
        {
            currTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0, 1, currTime / timeToWait);
            upgradeCanvasGroup.alpha = Mathf.Lerp(1, 0, currTime / timeToWait);
            yield return null;
        }
        Time.timeScale = 1;
        upgradeCanvasGroup.gameObject.SetActive(false);

        // However, if the player has more upgrades to render, do that!
        PlayerXPHandler.Instance.upgradesToRender--;
        if (PlayerXPHandler.Instance.upgradesToRender > 0)
        {
            OverlayManager.Instance.ShowScreenOverlayWithLowSortingOrder(0.6f, 0.6f, () =>
            {
                UIController.Instance.AllowPlayerToChooseUpgrade();
            });
        }
    }

}
