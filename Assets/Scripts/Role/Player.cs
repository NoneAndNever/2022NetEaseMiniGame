using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 玩家
/// </summary>
public class Player : Role, IDataPersistence
{
    [SerializeField] private Transform fatherTrans;//父物体
    private readonly Vector3 turnLeft = new Vector3(45, 180, 0);
    private readonly Vector3 turnRight = new Vector3(-45, 0, 0);
    private Node rebornNode;

    private bool RoundStart = true;
    
    #region 节点

    public Node nextNode;

    private Node UpNode => AStarPathFinding.GetInstance().GetGraphNode(NodePosition.x, NodePosition.y + 1);
    private Node DownNode => AStarPathFinding.GetInstance().GetGraphNode(NodePosition.x, NodePosition.y - 1);
    private Node RightNode => AStarPathFinding.GetInstance().GetGraphNode(NodePosition.x + 1, NodePosition.y);
    private Node LeftNode =>AStarPathFinding.GetInstance().GetGraphNode(NodePosition.x - 1, NodePosition.y);

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

    [SerializeField] private GameObject upShadow;//上圈
    [SerializeField] private GameObject downShadow;//下圈
    [SerializeField] private GameObject rightShadow;//右圈
    [SerializeField] private GameObject leftShadow;//左圈

    [SerializeField] private Sprite normalInstruction;//默认指示
    [SerializeField] private Sprite confirmInstruction;//确认指示

    private Dictionary<Direction, SpriteRenderer> instructionSprites = new Dictionary<Direction, SpriteRenderer>();//每个指示的精灵

    /// <summary>
    /// 检测是否有被阻挡的节点，有则对应方向的指示取消激活
    /// </summary>
    private void CheckInstructions()
    {
        upShadow.SetActive(NodePosition.GetValidNeighbors(Node.Direction.Four).Contains(UpNode));
        downShadow.SetActive(NodePosition.GetValidNeighbors(Node.Direction.Four).Contains(DownNode));
        leftShadow.SetActive(NodePosition.GetValidNeighbors(Node.Direction.Four).Contains(LeftNode));
        rightShadow.SetActive(NodePosition.GetValidNeighbors(Node.Direction.Four).Contains(RightNode));
    }

    /// <summary>
    /// 移动时所有指示取消激活
    /// </summary>
    private void CancelInstructions()
    {
        upShadow.SetActive(false);
        downShadow.SetActive(false);
        leftShadow.SetActive(false);
        rightShadow.SetActive(false);
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
    public bool IsDead => nowState == States.Die ? true : false;
    
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
                _animator.SetBool(IsIdle, false);
                _animator.SetBool(IsMove, false);
                _animator.SetTrigger(Die);
                nowState = States.Die;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    #endregion

    #region 生命周期

    private void Awake()
    {
        EventCenter.GetInstance().AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundBegin, BeginCheck)
            .AddListener(EventType.RoundEnd, EndCheck);
    }

   
    // Start is called before the first frame update
    void Start()
    {
        var position = fatherTrans.position;
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)position.x, (int)position.y);
        
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
        if (RoundStart)
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

        if (NodePosition.GetValidNeighbors(Node.Direction.Four).Contains(tempNode) || tempNode == NodePosition)
        {
            if (direction == selectedDirection)
            {
                nextNode = tempNode;
                RoundStart = false;
                StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
            }
            else
            {
                selectedDirection = direction;
                ConfirmInstruction(selectedDirection);
            }
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
        if (selectedDirection == Direction.Left) transform.localEulerAngles = turnLeft;
        else if (selectedDirection == Direction.Right) transform.localEulerAngles = turnRight;
        
        nowState = States.IsMove;
        ChangeState(nowState);
        CancelInstructions();

        fatherTrans.DOMove(nextNode.position, moveTime);
        StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
        
    }

    /// <summary>
    /// 回合末检测
    /// </summary>
    private void EndCheck()
    {
        //更新玩家的地图点
        NodePosition = nextNode;

        if (nowState != States.Die)
        {
            nowState = States.IsIdle;
            ChangeState(nowState);
        }
    
        StartCoroutine(RoundCtrl.GetInstance().NextRoundState(NodePosition));
    }

    /// <summary>
    /// 回合初检测
    /// </summary>
    private void BeginCheck()
    {
        if (nowState != States.Die)
        {
            CheckInstructions();
            RoundStart = true;
        }
    }

    public void LoadData(GameData data)
    {
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)data.PlayerNode.x, (int)data.PlayerNode.y);
        fatherTrans.position = NodePosition.position;
        nowState = States.IsIdle;
        ChangeState(States.IsIdle);
        BeginCheck();
    }

    public void SaveData(GameData data)
    {
        data.PlayerNode = new Vector2(NodePosition.x, NodePosition.y);
    }
}
