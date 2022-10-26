using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    public enum _soundType
    {
        EMPTY,
        BGM1,
        BGM2,
        BGM3,
        BGM4
    }

    [SerializeField] private AudioSource BGM;
    private _soundType selectedBGM = _soundType.EMPTY;

    private float volume = 0.25f;

    [SerializeField] private AudioClip BGM1, BGM2, BGM3;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(instance);

    }

    private void Start()
    {
        AudioListener.volume = volume;
    }

    public void ChangeMusic(_soundType soundType)
    {
        if (soundType != selectedBGM)
        {
            BGM.clip = soundType switch
            {
                _soundType.BGM1 => this.BGM1,
                _soundType.BGM2 => BGM2,
                _soundType.BGM3 => BGM3
            };

            selectedBGM = soundType;
            BGM.loop = true;
            BGM.Play();
        }
    }

    public void ChangeVolume(float value)
    {
        volume = value;
        AudioListener.volume = volume;
    }

    public float GetVolume()
    {
        return AudioListener.volume;
    }
}
