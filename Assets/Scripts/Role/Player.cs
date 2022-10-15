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

    public Node nextNode;

    #region 行动状态

    public enum States
    {
        IsIdle,//站立
        IsMove,//移动
        Vigilant,//警惕
        Attack,//攻击
        Die//死亡
    }

    private States nowState = States.IsIdle;
    
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int Vigilant = Animator.StringToHash("Vigilant");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Die = Animator.StringToHash("Die");
    
    /// <summary>
    /// 改变行动状态，同时播放动画
    /// </summary>
    /// <param name="state"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ChangeState(States state)
    {
        switch (state)
        {
            case States.IsIdle:
                _animator.SetBool(IsIdle, true);
                _animator.SetBool(IsMove, false);
                nowState = States.IsIdle;
                break;
            case States.IsMove:
                _animator.SetBool(IsMove, true);
                _animator.SetBool(IsIdle, false);
                nowState = States.IsMove;
                break;
            case States.Vigilant:
                _animator.SetTrigger(Vigilant);
                break;
            case States.Attack:
                _animator.SetTrigger(Attack);
                break;
            case States.Die:
                _animator.SetTrigger(Die);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    #endregion

    #region 生命周期

    private void Awake()
    {
        EventCenter.AddListener(EventType.DoingMove, Move);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
        ChangeState(States.IsIdle);
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
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                MoveCheck(PathFinding.GetGraphNode(NodePosition.x, NodePosition.y));
            }
        }
    }    

    #endregion


    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
        ChangeState(States.IsMove);
        transform.DOMove(nextNode.position, moveTime).OnComplete(delegate
        {
            //锁定移动状态
            MovementCtrl.IsMoving = false;
            //更新玩家的地图点
            NodePosition = nextNode;
            //回合数+1
            MovementCtrl.RoundNum++;
            EventCenter.BroadcastEvent(EventType.RoundEnd);
            ChangeState(States.IsIdle);
            EventCenter.BroadcastEvent(EventType.RoundBegin);
        });
    }
    
    /// <summary>
    /// 移动检测，不可到达障碍物点
    /// </summary>
    /// <param name="nextNode">移动目标位置</param>
    private void MoveCheck(Node nextNode)
    {
        if (!nextNode.isBlocked)
        {
            if (this.nextNode != nextNode) this.nextNode = nextNode;
            else
            {
                MovementCtrl.Moving();
            }
        }
    }

}
