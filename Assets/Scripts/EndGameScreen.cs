using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text scoreText;
    public void ShowGameOverScreen(string score)
    {
        scoreText.text = score;
        gameObject.SetActive(true);
    }

    public void HideGameOverScreen()
    {
        gameObject.SetActive(false);
    }
}
