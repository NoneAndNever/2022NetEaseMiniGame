using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.Timeline;

/// <summary>
/// 狙击手
/// </summary>
public class Sniper : Role
{
    //玩家
    private Transform player;

    //旋转方向
    enum RotateDirection
    {
        Positive = 1,//顺时针
        Negative = -1//逆时针
    }
    
    //当前狙击手的旋转方向
    private Vector3 rotate = new Vector3(0, 0, 0);
    [SerializeField] private RotateDirection direction = RotateDirection.Positive;
    
    private void Awake()
    {
        MovementCtrl.SnipersTrans.Add(transform);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerPos(player);
    }
    
    /// <summary>
    /// 判断玩家是否进入检测范围
    /// </summary>
    /// <param name="playerTrans">玩家</param>
    private void CheckPlayerPos(Transform playerTrans)
    {
        Vector2 toForward = transform.forward;
        Vector2 toPlayer = playerTrans.position - transform.position;
        float distance = toPlayer.magnitude;
        float angel = Mathf.Acos(Vector2.Dot(toForward, toPlayer));
        if (distance < 2f && angel <= Mathf.PI / 6)
        {
            if (MovementCtrl.IsMoving == true || angel == 0)
            {
                //玩家死亡
                Destroy(playerTrans.GameObject());
            }
            else
            {
                //广播玩家位置Node
                
            }
        }
    }

    /// <summary>
    /// 旋转
    /// </summary>
    public void Move()
    {
        rotate.z = (rotate.z + 90 * (int)direction) % 360;
        transform.DORotate(rotate,0.5f);
    }
}
