using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUIController : MonoBehaviour
{

    [Header("Object Assignments")]
    public TextMeshProUGUI pointsText;

    private void Start()
    {
        pointsText.text = "HIGH SCORE: " + PlayerPrefs.GetInt("highscore");
    }

}
