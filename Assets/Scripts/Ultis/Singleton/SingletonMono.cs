using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 需要自己手动挂载的Mono脚本,需要手动确认唯一性
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        return instance;
    }

    protected virtual void Awake()
    {
        instance = this as T;
    }
}
