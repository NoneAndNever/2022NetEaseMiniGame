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
        MovementCtrl.PlayerTrans = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GraphNodes[(int)position.x, (int)position.y];
    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }

    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
        //当角色不在移时，进行位移动画插值
        if (!MovementCtrl.IsMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveCheck(PathFinding.GraphNodes[NodePosition.x, NodePosition.y + 1]);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveCheck(PathFinding.GraphNodes[NodePosition.x, NodePosition.y - 1]);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveCheck(PathFinding.GraphNodes[NodePosition.x + 1, NodePosition.y]);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveCheck(PathFinding.GraphNodes[NodePosition.x - 1, NodePosition.y]);
            }

        }
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
            MovementCtrl.Moving(NodePosition);
        }
    }


}
