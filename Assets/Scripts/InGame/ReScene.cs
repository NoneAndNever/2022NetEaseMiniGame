using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReScene : SingletonMono<ReScene>
{
    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(instance);
    }

    public void Reload()
    {
        //EventCenter.GetInstance().Clear();
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public IEnumerator ReLoad()
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Additive);

        asyncOperation.allowSceneActivation = false;    // 这里限制了跳转


        // 这里就是循环输入进度
        while(asyncOperation.progress < 0.9f)
        {
            Debug.Log(" progress = " + asyncOperation.progress);
        }

        asyncOperation.allowSceneActivation = true;    // 这里打开限制
        yield return null;

        if(asyncOperation.isDone)
        {
            Debug.Log("完成加载");
        }
    }
}
