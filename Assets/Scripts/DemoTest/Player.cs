using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : MonoBehaviour
{
    #region 变量定义

    [SerializeField] public Node PlayerPosition { get; set; }
    //移动控制器
    private readonly MovementCtrl movementCtrl = MovementCtrl.GetInstance();
    private readonly AStarPathFinding pathFinding = AStarPathFinding.GetInstance();

    #endregion

    private void Awake()
    {
        movementCtrl.PlayerTrans = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //当角色不在移时，进行位移动画插值
        if (!movementCtrl.IsMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveCheck(pathFinding.GraphNodes[PlayerPosition.x, PlayerPosition.y + 1]);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                MoveCheck(pathFinding.GraphNodes[PlayerPosition.x, PlayerPosition.y - 1]);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                MoveCheck(pathFinding.GraphNodes[PlayerPosition.x + 1, PlayerPosition.y]);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                MoveCheck(pathFinding.GraphNodes[PlayerPosition.x - 1, PlayerPosition.y]);
            }

        }

    }

    /// <summary>
    /// 移动检测，不可到达障碍物点
    /// </summary>
    /// <param name="nextPos">移动目标位置</param>
    private void MoveCheck(Node nextPos)
    {
        if (!nextPos.isBlocked)
        {
            PlayerPosition = nextPos;
            movementCtrl.Moving(PlayerPosition);
        }
    }
}
