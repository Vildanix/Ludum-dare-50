using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TMP_Text statusText;
    [SerializeField] TMPro.TMP_Text maxStatusText;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetValue(int value)
    {
        slider.value = value;
        statusText.text = value.ToString();
    }

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        maxStatusText.text = value.ToString();
    }
}
