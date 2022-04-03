using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioToggle : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public void ToggleAudio()
    {
        audioSource.mute = !audioSource.mute;
    }
}
