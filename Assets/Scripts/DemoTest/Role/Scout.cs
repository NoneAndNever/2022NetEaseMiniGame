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



    protected void Awake()
    {
        EventCenter.AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
        EventCenter.AddListener(EventType.DoingMove, Move);
        EventCenter.AddListener(EventType.RoundEnd, EndCheck);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.RemoveListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
        EventCenter.RemoveListener(EventType.DoingMove, Move);
        EventCenter.RemoveListener(EventType.RoundEnd, EndCheck);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
        _tamp = PlayerNode;
    }

    void Update()
    {
        if (_tamp != PlayerNode)
        {
            _path = PathFinding.FindPath(NodePosition, PlayerNode, false);
            _tamp = PlayerNode;
        }
    }

    public Node GetPlayerNode()
    {
        return PlayerNode;
    }

    public override void Move()
    {
        //获取移动方向
        _path?.TryPop(out _nextNode);
        //开始移动
        _nextNode = _nextNode ?? NodePosition;
        NodePosition = _nextNode;
        transform.DOMove(_nextNode.position, moveTime);
            
        _path = PathFinding.FindPath(_nextNode, PlayerNode, false);
    }

    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        if (MovementCtrl.RoundNum % 2 == 0)
        {
               var col = Physics2D.OverlapCircle(transform.position, scanRadius, 1 << 6);
               if (col)
               {
                   Debug.Log("enter");
                   PlayerNode = col.GetComponent<Player>().NodePosition;
               }     
        }

    }

    /// <summary>
    /// 检测侦察兵
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        //TODO 玩家死亡
        if (col.CompareTag("Player"))
        {
            Debug.Log("kill player");
            col.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
    }
}
