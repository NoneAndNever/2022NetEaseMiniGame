using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 直升机
/// </summary>
public class Helicopter : Role
{
    private void Awake()
    {
        EventCenter.GetInstance().AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.GetInstance().AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
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
        base.Move();
    }
    
    public Node GetPlayerNode()
    {
        return PlayerNode;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("enter");
            Node playerNow = col.GetComponent<Player>().NodePosition;
            //广播玩家位置
            EventCenter.GetInstance().BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 3f);
            Debug.Log("broadcast");
        }
    }
}
