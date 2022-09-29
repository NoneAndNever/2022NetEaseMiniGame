using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Role : MonoBehaviour
{
    
    #region 变量定义
    protected Node PlayerNode;//玩家节点
    public Node NodePosition { get; set; }//自身节点
    
    protected readonly MovementCtrl MovementCtrl = MovementCtrl.GetInstance(); //移动控制器
    protected readonly AStarPathFinding PathFinding = AStarPathFinding.GetInstance();//A*地图


    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Move()
    {
        
    }
    
    protected void SetPlayerNode(Node playerNode)
    {
        PlayerNode = playerNode;
    }    
    protected void SetPlayerNode(Node playerNode, Vector2 foundPosition, float radius)
    {
        if (NodePosition.GetStraightDistance(foundPosition) < (radius * radius + 0.01f))
        {
            SetPlayerNode(playerNode);
        }
    }
}
