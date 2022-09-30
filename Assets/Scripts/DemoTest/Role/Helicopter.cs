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
    private void Awake()
    {
        EventCenter.AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
        EventCenter.AddListener(EventType.DoingMove, Move);
        EventCenter.AddListener(EventType.RoundEnd, ScanScope);

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
        
    }

    public override void Move()
    {
        Vector2 position = transform.position;
        _targetPos = (PlayerNode.position - position).sqrMagnitude < 0.251f
            ? PlayerNode.position
            : ((Vector2)PlayerNode.position - position).normalized * 0.5f + position;
        transform.DOMove(_targetPos, moveTime);
    }
    
    public Node GetPlayerNode()
    {
        return PlayerNode;
    }

    /*private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !MovementCtrl.IsMoving && MovementCtrl.Rounds % 2 == 0)
        {
            Debug.Log("enter");
            Node playerNow = col.GetComponent<Player>().NodePosition;
            //广播玩家位置
            EventCenter.BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 3f);
            Debug.Log("broadcast");
        }
    }*/
    private void ScanScope()
    {
        var col = Physics2D.OverlapCircle(transform.position, scanRadius, 1 << 6);
        
        if (col && !MovementCtrl.IsMoving && MovementCtrl.RoundNum % 2 == 0)
        {
            Debug.Log("enter");
            Node playerNow = col.GetComponent<Player>().NodePosition;
            //广播玩家位置
            EventCenter.BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 3f);
            Debug.Log("broadcast");
        }
    }
}
