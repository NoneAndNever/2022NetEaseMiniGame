using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 巡逻兵
/// </summary>
public class Enemy : Role
{

    #region 属性
    #endregion



    private void Awake()
    {
        MovementCtrl.EnemiesTrans.Add(transform);
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }


}
