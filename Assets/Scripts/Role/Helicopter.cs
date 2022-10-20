using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 直升机
/// </summary>
public class Helicopter : Role
{
    private Vector2 _targetPos;
    private static float scanRadius = 2f;
    
    //画线
    private LineRenderer line;//路线
    private float density = 1f;
    private float lineLength;
    private Vector2 tiling;
    private Vector2 offest;
    private int id = Shader.PropertyToID("_MainTex");
    private Material _material;
    
    
    private void Awake()
    {
        EventCenter.GetInstance().AddListener<Node>(EventType.PlayerFound, SetPlayerNode)
            .AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundEnd, EndCheck)
            .AddListener(EventType.RoundBegin, BeginCheck);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)position.x, (int)position.y);
        _targetPos = NodePosition.position;

        tiling = new Vector2(20, 0);
        offest = Vector2.zero;
        line = GetComponent<LineRenderer>();
        _material = line.material;
        _material.SetTextureScale(id, tiling);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 移动
    /// </summary>
    public override void Move()
    {
       transform.DOMove(_targetPos, moveTime);
    }
    
    
        
    /// <summary>
    /// 回合开始前检查
    /// </summary>
    private void BeginCheck()
    {
        if (PlayerNode != NodePosition && PlayerNode != null)
        {
            Vector2 position = transform.position;
            line.SetPosition(0, position);
            _targetPos = (PlayerNode.position - position).sqrMagnitude < 0.251f
                ? PlayerNode.position
                : ((Vector2)PlayerNode.position - position).normalized * 0.5f + position;

            lineLength = ((Vector2)PlayerNode.position - position).magnitude;
            tiling = new Vector2(lineLength * 50, 0);
            _material.SetTextureScale(id, tiling);
            line.SetPosition(1, _targetPos);
        }
    }
    
    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        /*if (MovementCtrl.GetInstance().RoundNum % 2 == 0)
        {
                var col = Physics2D.OverlapCircle(transform.position, scanRadius, 1 << 6);
                if (col)
                {
                    Debug.Log("enter");
                    Node playerNow = col.GetComponent<Player>().NodePosition;
                    //广播玩家位置
                    EventCenter.GetInstance().BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 3f);
                    Debug.Log("broadcast");
                }
        }*/

    }
    
}
