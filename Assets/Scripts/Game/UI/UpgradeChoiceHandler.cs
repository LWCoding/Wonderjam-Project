using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeChoiceHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public static bool areUpgradesInteractable = false;
    [Header("Object Assignments")]
    public TextMeshProUGUI upgradeTitleText;
    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI upgradeDescText;
    public Image previewImage;
    private Upgrade _upgradeInfo;
    private Vector2 _initialScale;
    private Vector2 _desiredScale;

    private void Awake()
    {
        _initialScale = transform.localScale;
        _desiredScale = _initialScale;
    }

    public void Initialize(Upgrade upgrade)
    {
        int playerUpgradeLevel = PlayerUpgradeHandler.Instance.GetUpgradeLevel(upgrade.type);
        _upgradeInfo = upgrade;
        previewImage.sprite = upgrade.previewSprite;
        upgradeTitleText.text = upgrade.upgradeName;
        upgradeLevelText.text = "LV. " + playerUpgradeLevel;
        upgradeDescText.text = upgrade.upgradeDescription.Replace("[OLD]", upgrade.GetUpgradeValue(playerUpgradeLevel).ToString()).Replace("[NEW]", upgrade.GetUpgradeValue(playerUpgradeLevel + 1).ToString());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!areUpgradesInteractable) { return; }
        _desiredScale = _initialScale * new Vector3(1.08f, 1.08f, 1);
        SoundManager.Instance.PlayOneShot(AudioType.UI_HOVER, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!areUpgradesInteractable) { return; }
        _desiredScale = _initialScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!areUpgradesInteractable) { return; }
        areUpgradesInteractable = false;
        SoundManager.Instance.PlayOneShot(AudioType.UI_CLICK, 0.5f);
        PlayerUpgradeHandler.Instance.AddUpgrade(_upgradeInfo.type);
        UIController.Instance.HideUpgrades();
    }

    public void Update()
    {
        float difference = Mathf.Abs(transform.localScale.x - _desiredScale.x);
        if (difference > 0.011f)
        {
            if (transform.localScale.x > _desiredScale.x)
            {
                if (difference < 0.04f)
                {
                    transform.localScale -= new Vector3(0.01f, 0.01f, 0);
                }
                else
                {
                    transform.localScale -= new Vector3(0.03f, 0.03f, 0);
                }
            }
            else
            {
                if (difference < 0.04f)
                {
                    transform.localScale += new Vector3(0.01f, 0.01f, 0);
                }
                else
                {
                    transform.localScale += new Vector3(0.03f, 0.03f, 0);
                }
            }
        }
    }

}
