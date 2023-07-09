using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{

    public static OverlayManager Instance;
    [Header("Object Assignments")]
    public GameObject screenOverlayObject;
    private IEnumerator _screenOverlayCoroutine = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.screenOverlayObject = screenOverlayObject;
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void ShowScreenOverlay(float targetOpacity, float timeInSeconds = 0.7f, Action actionAfterOverlay = null)
    {
        screenOverlayObject.GetComponent<Canvas>().sortingOrder = 99;
        if (_screenOverlayCoroutine != null)
        {
            StopCoroutine(_screenOverlayCoroutine);
        }
        _screenOverlayCoroutine = ShowScreenOverlayCoroutine(targetOpacity, timeInSeconds, actionAfterOverlay);
        StartCoroutine(_screenOverlayCoroutine);
    }

    public void ShowScreenOverlayWithLowSortingOrder(float targetOpacity, float timeInSeconds = 0.7f, Action actionAfterOverlay = null)
    {
        screenOverlayObject.GetComponent<Canvas>().sortingOrder = 5;
        if (_screenOverlayCoroutine != null)
        {
            StopCoroutine(_screenOverlayCoroutine);
        }
        _screenOverlayCoroutine = ShowScreenOverlayCoroutine(targetOpacity, timeInSeconds, actionAfterOverlay);
        StartCoroutine(_screenOverlayCoroutine);
    }

    public void HideScreenOverlay(float timeInSeconds = 0.7f, Action actionAfterOverlay = null)
    {
        if (_screenOverlayCoroutine != null)
        {
            StopCoroutine(_screenOverlayCoroutine);
        }
        _screenOverlayCoroutine = HideScreenOverlayCoroutine(timeInSeconds, actionAfterOverlay);
        StartCoroutine(_screenOverlayCoroutine);
    }

    private IEnumerator ShowScreenOverlayCoroutine(float targetOpacity, float timeInSeconds, Action actionAfterOverlay)
    {
        screenOverlayObject.SetActive(true);
        float frames = 0;
        float maxFrames = 60 * timeInSeconds;
        Color initialColor = new Color(0, 0, 0, 0);
        Color targetColor = new Color(0, 0, 0, targetOpacity);
        Image screenOverlayImage = screenOverlayObject.GetComponent<Image>();
        while (frames <= maxFrames)
        {
            frames++;
            screenOverlayImage.color = Color.Lerp(initialColor, targetColor, frames / maxFrames);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        if (actionAfterOverlay != null) { actionAfterOverlay(); }
    }

    private IEnumerator HideScreenOverlayCoroutine(float timeInSeconds, Action actionAfterOverlay)
    {
        screenOverlayObject.SetActive(true);
        float frames = 0;
        float maxFrames = 60 * timeInSeconds;
        Image screenOverlayImage = screenOverlayObject.GetComponent<Image>();
        Color initialColor = screenOverlayImage.color;
        Color targetColor = new Color(0, 0, 0, 0);
        while (frames <= maxFrames)
        {
            frames++;
            screenOverlayImage.color = Color.Lerp(initialColor, targetColor, frames / maxFrames);
            yield return null;
        }
        screenOverlayObject.SetActive(false);
        if (actionAfterOverlay != null) { actionAfterOverlay(); }
    }

}
