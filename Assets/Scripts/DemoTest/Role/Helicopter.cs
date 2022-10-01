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
<<<<<<< HEAD
    private Vector2 _direction;
    
=======
    private Vector2 _targetPos;
    private static float scanRadius = 2f;
>>>>>>> origin/PatrickStar
    private void Awake()
    {
        EventCenter.AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
        EventCenter.AddListener(EventType.DoingMove, Move);
<<<<<<< HEAD
=======
        EventCenter.AddListener(EventType.RoundEnd, ScanScope);

>>>>>>> origin/PatrickStar
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
<<<<<<< HEAD
        NodePosition = PathFinding.GraphNodes[(int)position.x, (int)position.y];
=======
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
>>>>>>> origin/PatrickStar
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Move()
    {
<<<<<<< HEAD
        _direction = (Vector2)(PlayerNode?.position)?.normalized;
=======
        Vector2 position = transform.position;
        _targetPos = (PlayerNode.position - position).sqrMagnitude < 0.251f
            ? PlayerNode.position
            : ((Vector2)PlayerNode.position - position).normalized * 0.5f + position;
        transform.DOMove(_targetPos, moveTime);
>>>>>>> origin/PatrickStar
    }
    
    public Node GetPlayerNode()
    {
        return PlayerNode;
    }

<<<<<<< HEAD
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
=======
    /*private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !MovementCtrl.IsMoving && MovementCtrl.Rounds % 2 == 0)
>>>>>>> origin/PatrickStar
        {
            Debug.Log("enter");
            Node playerNow = col.GetComponent<Player>().NodePosition;
            //广播玩家位置
            EventCenter.BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 3f);
            Debug.Log("broadcast");
<<<<<<< HEAD
=======
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
>>>>>>> origin/PatrickStar
        }
    }
}
