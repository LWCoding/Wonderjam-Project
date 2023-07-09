using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialTextHandler : MonoBehaviour
{

    private void Update()
    {
        if (GetDistanceToPlayer() < 6)
        {
            StartCoroutine(FadeAwayCoroutine());
        }
    }

    private float GetDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, PlayerMovementHandler.Instance.transform.position);
    }

    private IEnumerator FadeAwayCoroutine()
    {
        float currTime = 0;
        float timeToWait = 0.7f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(0, 0.4f, 0);
        TextMeshPro _tmp = GetComponent<TextMeshPro>();
        Color initialColor = _tmp.color;
        Color targetColor = new Color(1, 1, 1, 0);
        while (currTime <= timeToWait)
        {
            currTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, currTime / timeToWait);
            _tmp.color = Color.Lerp(initialColor, targetColor, currTime / timeToWait);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
