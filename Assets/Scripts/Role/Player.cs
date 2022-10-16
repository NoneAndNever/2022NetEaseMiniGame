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
    [SerializeField] private Transform fatherTrans;//父物体
    private readonly Vector3 turnLeft = new Vector3(0, 180, 0);
    private readonly Vector3 turnRight = Vector3.zero;
    
    #region 节点

    public Node nextNode;

    private Node UpNode => PathFinding.GetGraphNode(NodePosition.x, NodePosition.y + 1);
    private Node DownNode => PathFinding.GetGraphNode(NodePosition.x, NodePosition.y - 1);
    private Node RightNode => PathFinding.GetGraphNode(NodePosition.x + 1, NodePosition.y);
    private Node LeftNode =>PathFinding.GetGraphNode(NodePosition.x - 1, NodePosition.y);

    private Node tempNode;

    #endregion

    #region 方向指示
    
    private enum Direction
    {
        Begin,//起始状态
        Up,//向上
        Down,//向下
        Left,//向左
        Right,//向右
        Center//中心
    }

    private Direction selectedDirection = Direction.Begin;
    
    [SerializeField] private GameObject upInstruction;//指示向上
    [SerializeField] private GameObject downInstruction;//指示向下
    [SerializeField] private GameObject rightInstruction;//指示向右
    [SerializeField] private GameObject leftInstruction;//指示向左

    [SerializeField] private Sprite normalInstruction;//默认指示
    [SerializeField] private Sprite confirmInstruction;//确认指示

    private Dictionary<Direction, SpriteRenderer> instructionSprites = new Dictionary<Direction, SpriteRenderer>();//每个指示的精灵

    /// <summary>
    /// 检测是否有被阻挡的节点，有则对应方向的指示取消激活
    /// </summary>
    private void CheckInstructions()
    {
        upInstruction.SetActive(!UpNode.isBlocked);
        downInstruction.SetActive(!DownNode.isBlocked);
        leftInstruction.SetActive(!LeftNode.isBlocked);
        rightInstruction.SetActive(!RightNode.isBlocked);
    }

    /// <summary>
    /// 移动时所有指示取消激活
    /// </summary>
    private void CancelInstructions()
    {
        upInstruction.SetActive(false);
        downInstruction.SetActive(false);
        leftInstruction.SetActive(false);
        rightInstruction.SetActive(false);
    }

    /// <summary>
    /// 将确认的方向由默认指示变为确认指示
    /// </summary>
    /// <param name="confirmDirction"></param>
    private void ConfirmInstruction(Direction confirmDirction)
    {
        foreach (Direction direction in instructionSprites.Keys)
            instructionSprites[direction].sprite = direction == confirmDirction ? confirmInstruction : normalInstruction;
    }

    #endregion
    
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
                nowState = nowState;
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
        var position = fatherTrans.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
        
        instructionSprites.Add(Direction.Up, upInstruction.GetComponent<SpriteRenderer>());
        instructionSprites.Add(Direction.Down, downInstruction.GetComponent<SpriteRenderer>());
        instructionSprites.Add(Direction.Left, leftInstruction.GetComponent<SpriteRenderer>());
        instructionSprites.Add(Direction.Right, rightInstruction.GetComponent<SpriteRenderer>());
        
        CheckInstructions();
        
        ChangeState(nowState);
    }

    // Update is called once per frame
    void Update()
    {
        //当角色不在移时，进行位移动画插值
        if (!MovementCtrl.IsMoving)
        {

            if (Input.GetKeyDown(KeyCode.W)) MoveCheck(Direction.Up);

            else if (Input.GetKeyDown(KeyCode.S)) MoveCheck(Direction.Down);

            else if (Input.GetKeyDown(KeyCode.D)) MoveCheck(Direction.Right);

            else if (Input.GetKeyDown(KeyCode.A)) MoveCheck(Direction.Left);

            else if (Input.GetKeyDown(KeyCode.Space)) MoveCheck(Direction.Center);
        }
    }

    #endregion


    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
        if (selectedDirection == Direction.Left) transform.eulerAngles = turnLeft;
        else if (selectedDirection == Direction.Right) transform.eulerAngles = turnRight;
        
        nowState = States.IsMove;
        ChangeState(nowState);
        CancelInstructions();
        
        fatherTrans.DOMove(nextNode.position, moveTime).OnComplete(delegate
        {
            //锁定移动状态
            MovementCtrl.IsMoving = false;
            //更新玩家的地图点
            NodePosition = nextNode;
            //回合数+1
            MovementCtrl.RoundNum++;
            
            CheckInstructions();
            
            EventCenter.BroadcastEvent(EventType.RoundEnd);
            nowState = States.IsIdle;
            ChangeState(nowState);
            EventCenter.BroadcastEvent(EventType.RoundBegin);
        });
    }

    /// <summary>
    /// 移动检测，不可到达障碍物点
    /// </summary>
    /// <param name="direction">移动方向</param>
    private void MoveCheck(Direction direction)
    {
        tempNode = direction switch
        {
            Direction.Up => UpNode,
            Direction.Down => DownNode,
            Direction.Left => LeftNode,
            Direction.Right => RightNode,
            Direction.Center => NodePosition
        };

        if (!tempNode.isBlocked)
        {
            if (direction == selectedDirection)
            {
                nextNode = tempNode;
                MovementCtrl.Moving();
            }
            else
            {
                selectedDirection = direction;
                ConfirmInstruction(selectedDirection);
            }
        }
    }

}
