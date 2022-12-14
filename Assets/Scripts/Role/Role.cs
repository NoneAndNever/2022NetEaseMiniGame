using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Role : MonoBehaviour
{
    
    #region 变量定义
    protected Node PlayerNode { get; set; }//玩家节点
    public Node NodePosition { get; set; }//自身节点
    
    protected float moveTime = 0.5f;

    [SerializeField] protected Animator _animator;



    #endregion


    public virtual void Move()
    {
        
    }
    
    /// <summary>
    /// 设置所有角色的玩家节点
    /// </summary>
    /// <param name="playerNode"></param>
    protected void SetPlayerNode(Node playerNode)
    {
        PlayerNode = playerNode;
    }    
    
    /// <summary>
    /// 设置在某foundPosition单位的radius范围内的其他单位的玩家节点
    /// </summary>
    /// <param name="playerNode"></param>
    /// <param name="foundPosition"></param>
    /// <param name="radius"></param>
    protected void SetPlayerNode(Node playerNode, Vector2 foundPosition, float radius)
    {
        if (GetStraightDistance(foundPosition) < (radius * radius + 0.01f))
        {
            SetPlayerNode(playerNode);
        }
    }
    
    /// <summary>
    /// 获取直线距离（平方）
    /// </summary>
    /// <param name="foundPosition"></param>
    /// <returns></returns>
    private float GetStraightDistance(Vector2 foundPosition)
    {
        return (foundPosition - (Vector2)transform.position).sqrMagnitude;
    }
}
