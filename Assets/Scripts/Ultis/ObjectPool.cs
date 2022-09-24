using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ObjectPool : BaseManager<ObjectPool>
{
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;

    public ObjectPool()
    {
        pool = new GameObject("ObjectPool");             //实例化总池对所有对象池进行统一存储
    }
    
    public GameObject GetObject(GameObject prefab)
    {
        GameObject gameObject;
        string name = prefab.name + "Pool";
        if (!objectPool.ContainsKey(name))
            //如果不存在对应子池,，则实例化后推入池中
        {
            //不存在则创建，并设置为总池的子物体
            GameObject childPool = new GameObject(name);
            childPool.transform.SetParent(pool.transform);
            gameObject = GameObject.Instantiate(prefab,childPool.transform);//实例化prefab对象，并设置为对应子池的子物体
            PushObject(gameObject);                                         //推入队列等待应用
        }
        else if (objectPool[name].Count == 0)
            //如果实例可用数量为0，则实例化后推入池中
        {
            GameObject childPool = pool.transform.Find(name).gameObject;    //寻找是否存在对应prefab的子池
            gameObject = GameObject.Instantiate(prefab,childPool.transform);//实例化prefab对象，并设置为对应子池的子物体
            PushObject(gameObject);                                         //推入队列等待应用
        }

        gameObject = objectPool[name].Dequeue();    //从队列中取出实例
        gameObject.SetActive(true);                 //激活实例
        
        return gameObject;
    }

    public void PushObject(GameObject gameObject)
    {
        string name = gameObject.name.Replace("(Clone)", "Pool");
        if(!objectPool.ContainsKey(name))                       //查找是否存在对应prefab的队列，不存在则创建
            objectPool.Add(name,new Queue<GameObject>());
        objectPool[name].Enqueue(gameObject);                   //将实例入队待用
        gameObject.SetActive(false);                            //禁用物体
    }
}
