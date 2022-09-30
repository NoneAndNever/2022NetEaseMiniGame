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
    Node _tamp;
    
    #endregion



    protected void Awake()
    {
        EventCenter.AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
        EventCenter.AddListener(EventType.DoingMove, Move);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GraphNodes[(int)position.x, (int)position.y];
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
}
