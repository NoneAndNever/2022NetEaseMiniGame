using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 玩家
/// </summary>
public class Player : Role
{


    private void Awake()
    {
        EventCenter.AddListener(EventType.DoingMove, Move);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
    }

    // Update is called once per frame
    void Update()
    {
        //当角色不在移时，进行位移动画插值
        if (!MovementCtrl.IsMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveCheck(PathFinding.GetGraphNode(NodePosition.x, NodePosition.y + 1));
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveCheck(PathFinding.GetGraphNode(NodePosition.x, NodePosition.y - 1));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveCheck(PathFinding.GetGraphNode(NodePosition.x + 1, NodePosition.y));
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveCheck(PathFinding.GetGraphNode(NodePosition.x - 1, NodePosition.y));
            }

        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
        transform.DOMove(NodePosition.position, moveTime).OnComplete((delegate
        {
            //锁定移动状态
            MovementCtrl.IsMoving = false;
            //回合数+1
            MovementCtrl.RoundNum++;
            if (MovementCtrl.RoundNum % 2 == 0) 
                EventCenter.BroadcastEvent(EventType.RoundEnd);
        }));
    }
    
    /// <summary>
    /// 移动检测，不可到达障碍物点
    /// </summary>
    /// <param name="nextNode">移动目标位置</param>
    private void MoveCheck(Node nextNode)
    {
        if (!nextNode.isBlocked)
        {
            NodePosition = nextNode;
            MovementCtrl.Moving();
        }
    }


}
