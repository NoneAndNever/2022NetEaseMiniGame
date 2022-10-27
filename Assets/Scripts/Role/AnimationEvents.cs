using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject role;
    [SerializeField] private DataPersistenceManager data;
    private static int deadCount = 0;


    public void RoleDie()
    {
        role.SetActive(false);
    }

    public void ReBorn()
    {
        deadCount++;
        if (deadCount != 2)
        {
            
            data.LoadGame();
        }
        else
        {
            deadCount = 0;
            ReScene.GetInstance().Reload();
        }
        
        //data.LoadGame();
        //StartCoroutine(ReScene.GetInstance().ReLoad());
        //ReScene.GetInstance().Reload();
    }
}