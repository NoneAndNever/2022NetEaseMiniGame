using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 巡逻兵
/// </summary>
public class Scout : Role
{

    #region 属性

    private Stack<Node> _path = null;
    private Node _nextNode = null;
    private static float scanRadius = 1.414f;
    private Node _tamp;
    
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
    //private static readonly int Die = Animator.StringToHash("Die");

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
            /*case States.Die:
                _animator.SetTrigger(Die);
                break;*/
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    #endregion

    #region 生命周期

    protected void Awake()
    {
        EventCenter
            .AddListener<Node>(EventType.PlayerFound, SetPlayerNode)
            .AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundEnd, EndCheck);
    }

    private void OnDisable()
    {
        EventCenter
            .RemoveListener<Node>(EventType.PlayerFound, SetPlayerNode)
            .RemoveListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .RemoveListener(EventType.DoingMove, Move)
            .RemoveListener(EventType.RoundEnd, EndCheck);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
        _tamp = PlayerNode;
        ChangeState(States.IsIdle);
    }

    void Update()
    {
        if (_tamp != PlayerNode)
        {
            _path = PathFinding.FindPath(NodePosition, PlayerNode, false);
            _tamp = PlayerNode;
        }
    }

    #endregion


    public override void Move()
    {
        if (_path == null) return;
        
        ChangeState(States.IsMove);
        //获取移动方向
        _path?.TryPop(out _nextNode);
        //开始移动
        _nextNode = _nextNode ?? NodePosition;
        NodePosition = _nextNode;
        transform.DOMove(NodePosition.position, moveTime).OnComplete(delegate {ChangeState(States.IsIdle); });
            
        _path = PathFinding.FindPath(NodePosition, PlayerNode, false);
    }


    
    #region 回合检测与碰撞体检测
    
    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        if (MovementCtrl.RoundNum % 2 == 0)
        {
               ChangeState(States.Detect);
               var col = Physics2D.OverlapCircle(transform.position, scanRadius, 1 << 6);
               if (col)
               {
                   Debug.Log("enter");
                   PlayerNode = col.GetComponent<Player>().NodePosition;
               }     
        }

    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        //TODO 玩家死亡
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player>().ChangeState(Player.States.Die);
            Debug.Log("kill player");
            col.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
    }
    
    #endregion
    

}
