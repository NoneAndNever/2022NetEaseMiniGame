using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

/// <summary>
/// 巡逻兵
/// </summary>
public class Scout : Role, IDataPersistence
{
    [SerializeField] private string id;
    private float offsetRadius=0.5f;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid() 
    {
        id = Guid.NewGuid().ToString();
    }

    #region 属性

    private Stack<Node> _path = null;
    private Node _nextNode = null;
    private static float scanRadius = 1.514f;
    private Node _tamp;
    private int number;
    
    #endregion

    #region 重合时的位置

    private Vector2 SetCoincidencePos()
    {
        if(_nextNode.number == 1) return Vector2.zero;
        float rad = Mathf.Deg2Rad * (number * 360 / _nextNode.number);
        Vector2 degVec = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad))*offsetRadius;
        Debug.Log(degVec);
        return degVec;

    }

    #endregion

    #region 行动状态

    public enum States
    {
        IsIdle,//站立
        IsMove,//移动
        Detect,//扫描
        //Die//死亡
    }
    
    private States nowState = States.IsIdle;
        
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    private static readonly int Detect = Animator.StringToHash("Detect");

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
            case States.Detect:
                _animator.SetTrigger(Detect);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    #endregion

    #region 生命周期

    private void Awake()
    {
        EventCenter.GetInstance()
            .AddListener<Node>(EventType.PlayerFound, SetPlayerNode)
            .AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundBegin, BeginCheck)
            .AddListener(EventType.RoundEnd, EndCheck);
        id = Guid.NewGuid().ToString();
    }

    private void OnDisable()
    {
        EventCenter.GetInstance()
            .RemoveListener<Node>(EventType.PlayerFound, SetPlayerNode)
            .RemoveListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .RemoveListener(EventType.DoingMove, Move)
            .RemoveListener(EventType.RoundEnd, EndCheck);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)position.x, (int)position.y);
        number = ++NodePosition.number;
        //_tamp = PlayerNode;
        ChangeState(States.IsIdle);

        PlayerNode = NodePosition;
    }

    void Update()
    {
        if (_tamp != PlayerNode)
        {
            _path = AStarPathFinding.GetInstance().FindPath(NodePosition, PlayerNode, number);
            _tamp = PlayerNode;
        }
    }

    #endregion
    
    public override void Move()
    {
        if (_nextNode == null) return;
        
        ChangeState(States.IsMove);
        //开始移动
        NodePosition.number = 0;
        
        transform.DOMove(_nextNode.position + SetCoincidencePos(), moveTime).OnComplete(delegate { ChangeState(States.IsIdle); });
        NodePosition = _nextNode; 
        _path = AStarPathFinding.GetInstance().FindPath(NodePosition, PlayerNode, number);
                   
        
    }

    #region 回合检测与碰撞体检测


    private void BeginCheck()
    {
        if (_path == null) _nextNode = null;
        else
        {
            //获取移动方向
            _path?.TryPop(out _nextNode);
            _nextNode = _nextNode ?? NodePosition;
            number = ++_nextNode.number;
        }
    }
    
    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        if (MovementCtrl.GetInstance().RoundNum % 2 == 0)
        {
               ChangeState(States.Detect);
               var col = Physics2D.OverlapCircle(transform.position, scanRadius, 1 << 6);
               if (!col) return;
               PlayerNode = col.GetComponent<Player>().NodePosition;
               _path = AStarPathFinding.GetInstance().FindPath(NodePosition, PlayerNode, number);
        }

    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player>().ChangeState(Player.States.Die);
            Debug.Log("kill player");
        }
    }
    
    #endregion


    public void LoadData(GameData data)
    {
        //侦察兵的节点
        data.scoutNodePosition.TryGetValue(id, out var pos);
        transform.position = pos;
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)pos.x, (int)pos.y);
        //目标节点
        data.scoutTargetPosition.TryGetValue(id, out pos);
        PlayerNode = AStarPathFinding.GetInstance().GetGraphNode((int)pos.x, (int)pos.y);

        data.scoutNumber.TryGetValue(id, out number);
        NodePosition.number = NodePosition.number < number ? number : NodePosition.number;
        _path = AStarPathFinding.GetInstance().FindPath(NodePosition, PlayerNode, NodePosition.number--);
    }

    public void SaveData(GameData data)
    {
        if (data.scoutNodePosition.ContainsKey(id))
        {
            data.scoutNodePosition.Remove(id);
        }
        data.scoutNodePosition.Add(id, NodePosition.position);
        
        if (data.scoutTargetPosition.ContainsKey(id))
        {
            data.scoutTargetPosition.Remove(id);
        }
        data.scoutTargetPosition.Add(id, PlayerNode.position);

        if (data.scoutNumber.ContainsKey(id))
        {
            data.scoutNumber.Remove(id);
        }
        data.scoutNumber.Add(id, number);
    }
    
    
}
