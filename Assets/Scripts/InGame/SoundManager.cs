using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private AudioClip tempBGM;
    private int sceneNum;
    private int lastSceneNum;
    
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            //EventCenter.GetInstance().AddListener<int>(EventType.ChangeMusic, ChangeMusic);
        }
        else Destroy(instance);
    }

    private void Start()
    {
        AudioListener.volume = volume;
    }

    private void Update()
    {
           int sceneNum = SceneManager.GetActiveScene().buildIndex;
        if (lastSceneNum != sceneNum)
        {
            lastSceneNum = sceneNum;
            ChangeMusic(lastSceneNum);
        }
    }

    private void ChangeMusic(int sceneNumber)
    {
        BGM.loop = true;
        if (sceneNumber > 1 && sceneNumber < 8)
        {
            if (sceneNumber == 2)
            {
                BGM.clip = BGM1; 
                BGM.Play();
            }
            else if (sceneNumber == 6)
            {
                BGM.clip = BGM2; 
                BGM.Play();
            }
            else if (sceneNumber == 7)
            {
                BGM.clip = BGM3; 
                BGM.Play();
            }
            
        }
        else
        {
            BGM.clip = null;
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
