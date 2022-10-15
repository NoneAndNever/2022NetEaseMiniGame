using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine.UIElements;

/// <summary>
/// 狙击手
/// </summary>
public class Sniper : Role
{

    //旋转方向
    enum RotateDirection
    {
        Positive = 1,//顺时针
        Negative = -1//逆时针
    }
    
    //当前狙击手的旋转方向
    private Vector3 rotate = new Vector3(0, 0, 0);
    [SerializeField] private RotateDirection direction = RotateDirection.Positive;
    
    private void Awake()
    {
        rotate.z = transform.rotation.eulerAngles.z;
        EventCenter.AddListener(EventType.DoingMove, Move);
        EventCenter.AddListener(EventType.RoundEnd, EndCheck);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener(EventType.DoingMove, Move);
        EventCenter.RemoveListener(EventType.RoundEnd, EndCheck);
    }

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        Vector2 lookAt = Vector2.zero;
        switch (rotate.z)
        {
            case 0:
                lookAt = Vector2.up;
                break;
            case 90:
                lookAt = Vector2.left;
                break;
            case 180:
                lookAt = Vector2.down;
                break;
            case 270:
                lookAt = Vector2.right;
                break;
        }
        RaycastHit2D hit = Physics2D.Raycast(NodePosition.position + lookAt * 0.5f, lookAt, 2f, 1 << 6);
        if (hit)
        {
            hit.collider.gameObject.SetActive(false);
            Time.timeScale = 0;
        }
    }
    
    /// <summary>
    /// 旋转
    /// </summary>
    public override void Move()
    {
        rotate.z = (rotate.z + 90 * (int)direction) % 360;
        transform.DORotate(rotate,moveTime);
    }
    
    /// <summary>
    /// 进入狙击手的射界
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("enter");
            Node playerNow = col.GetComponent<Player>().NodePosition;
            if (NodePosition.position + Vector2.down == playerNow.position && ((int)rotate.z+360)%360 == 0
                || NodePosition.position + Vector2.up == playerNow.position && ((int)rotate.z + 360) % 360 == 180
                || NodePosition.position + Vector2.left == playerNow.position && ((int)rotate.z + 360) % 360 == 270
                || NodePosition.position + Vector2.right == playerNow.position && ((int)rotate.z + 360) % 360 == 90)
            {
                //狙击手死亡
                Debug.Log("kill Sniper");
                gameObject.SetActive(false);
            }
            //广播玩家位置
            else
            {
                EventCenter.BroadcastEvent<Node, Vector2, float>(EventType.PlayerFoundPartly, playerNow, transform.position, 5f);
                Debug.Log("broadcast");
            }
        }
     }
    
}
