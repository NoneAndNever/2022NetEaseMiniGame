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
    //玩家
    [SerializeField] private Transform player;

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
        CheckPlayerPos(player);
    }
    
    /// <summary>
    /// 判断玩家是否进入检测范围
    /// </summary>
    /// <param name="playerTrans">玩家</param>
    private void CheckPlayerPos(Transform playerTrans)
    {
        
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
            if (NodePosition.position + 2 * Vector2.down == playerNow.position
                || NodePosition.position + 2 * Vector2.up == playerNow.position
                || NodePosition.position + 2 * Vector2.left == playerNow.position
                || NodePosition.position + 2 * Vector2.right == playerNow.position)
            {
                //TODO 玩家死亡
                Debug.Log("kill");
            }
            //广播玩家位置
            else
            {
                EventCenter.BroadcastEvent(EventType.PlayerFound, playerNow);
                Debug.Log("broadcast");
            }
        }
    }
}
