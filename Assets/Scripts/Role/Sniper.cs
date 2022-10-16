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

    private Vector3 leftCircular = new Vector3(-1, 1, 1);
    private Vector3 rightCircular = new Vector3(1, 1, 1);
    
    #region 初始设置

    //旋转方向
    enum RotateDirection
    {
        Positive = 1,//逆时针
        Negative = -1//顺时针
    }
    
    //初始角度
    enum InitialAngel
    {
        UP = 0,//上
        Left = 90,//左
        Down = 180,//下
        Right = 270//右
    }
    
    //当前狙击手的旋转方向
    private Vector3 rotate = new Vector3(0, 0, 0);
    [SerializeField] private RotateDirection direction = RotateDirection.Positive;    
    [SerializeField] private InitialAngel initialAngel = InitialAngel.Right;    

    #endregion

    #region 行动状态

    public enum States
    {
        IsIdleUp,//站立_向上
        IsIdleDown,//站立_向下
        IsIdleHorizon,//站立_水平
        
        Shoot,
        Die//死亡
    }
    
    private States nowState;
        
    private static readonly int RotateDir = Animator.StringToHash("RotateDir");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Die = Animator.StringToHash("Die");

    /// <summary>
    /// 改变行动状态，同时播放动画
    /// </summary>
    /// <param name="state"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ChangeState(States state)
    {
        
        switch (state)
        {
            case States.IsIdleUp:
            case States.IsIdleDown:
            case States.IsIdleHorizon:
                _animator.SetInteger(RotateDir, (int)rotate.z);
                break;
            case States.Shoot:
                _animator.SetTrigger(Shoot);
                break;
            case States.Die:
                _animator.SetTrigger(Die);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    #endregion

    #region 生命周期

    private void Awake()
    {
        rotate.z = transform.rotation.eulerAngles.z;
        EventCenter
            .AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundEnd, EndCheck);
    }


    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GetGraphNode((int)position.x, (int)position.y);
        rotate.z = (int)initialAngel;
        nowState = rotate.z switch
        {
            0 => States.IsIdleUp,
            90 => States.IsIdleHorizon,
            270 => States.IsIdleHorizon,
            180 => States.IsIdleDown,
            _ => States.Die
        };
        
        transform.localScale = rotate.z > 180
                               || (direction == RotateDirection.Positive && rotate.z == 0)
                               || (direction == RotateDirection.Negative && rotate.z == 180)
            ? rightCircular : leftCircular;
        ChangeState(nowState);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        EventCenter
            .RemoveListener(EventType.DoingMove, Move)
            .RemoveListener(EventType.RoundEnd, EndCheck);
    }

    #endregion

    
    /// <summary>
    /// 旋转
    /// </summary>
    public override void Move()
    {
        rotate.z = (rotate.z + 90 * (int)direction + 360) % 360;
        transform.localScale = rotate.z > 180
                               || (direction == RotateDirection.Positive && rotate.z == 0)
                               || (direction == RotateDirection.Negative && rotate.z == 180)
            ? rightCircular : leftCircular;
        nowState = rotate.z switch
        {
            0 => States.IsIdleUp,
            90 => States.IsIdleHorizon,
            270 => States.IsIdleHorizon,
            180 => States.IsIdleDown,
            _ => States.Die
        };
        ChangeState(nowState);
    }


    #region 回合检测与碰撞体检测

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
            ChangeState(States.Shoot);
            hit.collider.gameObject.GetComponent<Player>().ChangeState(Player.States.Die);
        }
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
            Player player = col.GetComponent<Player>();
            Node playerNow = player.NodePosition;
            if (NodePosition.position + Vector2.down == playerNow.position && (int)rotate.z == 0
                || NodePosition.position + Vector2.up == playerNow.position && (int)rotate.z == 180
                || NodePosition.position + Vector2.left == playerNow.position && (int)rotate.z == 270
                || NodePosition.position + Vector2.right == playerNow.position && (int)rotate.z == 90)
            {
                //狙击手死亡
                Debug.Log("kill Sniper");
                player.ChangeState(Player.States.Attack);
                ChangeState(States.Die);
            }
            //广播玩家位置
            else
            {
                EventCenter.BroadcastEvent(EventType.PlayerFound, playerNow);
                Debug.Log("broadcast");
            }
        }
    }
    
    #endregion
}
