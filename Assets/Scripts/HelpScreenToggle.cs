using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpScreenToggle : MonoBehaviour
{
    public void ToggleObjectActivity()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
