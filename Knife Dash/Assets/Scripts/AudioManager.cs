using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public float SoundEffect_Volume { get; set; }
    public float BGMusic_Volume { get; set; }

    public List<AudioSource> SoundEffectSources = new List<AudioSource>();
    public AudioSource BGMusic;

    private void Start()
    {
        UpdateSoundEffectSettings();
    }

    public void UpdateSoundEffectSettings()
    {
        SoundEffectSources = new List<AudioSource>();
        var currentSources = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < currentSources.Length; i++)
        {
            if (currentSources[i] == BGMusic) continue;
            currentSources[i].volume = SoundEffect_Volume;
            SoundEffectSources.Add(currentSources[i]);
        }
    }

    public void ToggleSoundEffects(bool state)
    {
        var currentSources = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < currentSources.Length; i++)
        {
            if (currentSources[i] == BGMusic) continue;
            currentSources[i].enabled = state;
        }
    }
}
