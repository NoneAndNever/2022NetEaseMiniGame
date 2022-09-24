using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 自动Mono单例,无需手动挂载
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if (!instance)//如果单例不存在,则自动生成一个空物体,并自动将脚本挂上,并返回
        {
            GameObject gameObject = new GameObject(typeof(T).ToString()) ;
            instance = gameObject.AddComponent<T>();
        }

        return instance;
    }

}
