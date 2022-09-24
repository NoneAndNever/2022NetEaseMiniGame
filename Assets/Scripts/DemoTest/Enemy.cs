using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;


public class Enemy : MonoBehaviour
{
    #region 变量定义
    //玩家
    [SerializeField] private Transform player;
    //当前所在节点
    public Node EnemyPosition { get; set; }
    //移动控制器
    private readonly MovementCtrl movementCtrl = MovementCtrl.GetInstance();
    #endregion

    #region 属性
    #endregion



    private void Awake()
    {
        movementCtrl.EnemiesTrans.Add(transform);
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }

    protected virtual void Attack()
    {
        
    }

}
