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
public class Sniper : Role, IDataPersistence
{
    
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid() 
    {
        id = System.Guid.NewGuid().ToString();
    }

    private Vector3 leftCircular = new Vector3(-1, 1, 1);
    private Vector3 rightCircular = new Vector3(1, 1, 1);

    [SerializeField] private Transform rangeInstruction;
    
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
    private Vector3 rotate;
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
                nowState = States.Die;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    #endregion

    #region 生命周期

    private void Awake()
    {
        EventCenter.GetInstance()
            .AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode)
            .AddListener(EventType.DoingMove, Move)
            .AddListener(EventType.RoundEnd, EndCheck)
            .AddListener(EventType.RoundBegin, BeginCheck);
        id = Guid.NewGuid().ToString();
    }


    // Start is called before the first frame update
    void Start()
    {
        rotate = rangeInstruction.eulerAngles;
        rotate.z = (int)initialAngel;
        rangeInstruction.eulerAngles = rotate;

        var position = transform.position;
        NodePosition = AStarPathFinding.GetInstance().GetGraphNode((int)position.x, (int)position.y);
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

    private void OnDisable()
    {
        EventCenter.GetInstance()
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
        rangeInstruction.DORotate(rotate, moveTime);
    }


    #region 回合检测与碰撞体检测

    private void BeginCheck()
    {
        //if (MovementCtrl.RoundNum > 0)
    }

    /// <summary>
    /// 回合末检查
    /// </summary>
    private void EndCheck()
    {
        if (nowState != States.Die)
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
            RaycastHit2D hit = Physics2D.Raycast(NodePosition.position, lookAt, 2f, 1 << 6);
            if (hit)
            {
                ChangeState(States.Shoot);
                hit.collider.gameObject.GetComponent<Player>().ChangeState(Player.States.Die);
            }
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
            Debug.Log("PlayerEnter");
            Player player = col.GetComponent<Player>();
            Node playerNow = player.NodePosition;
            Node playerNext = player.nextNode;
            if (NodePosition.position + Vector2.down == playerNow.position && (int)rotate.z == 0
                || NodePosition.position + Vector2.up == playerNow.position && (int)rotate.z == 180
                || NodePosition.position + Vector2.left == playerNow.position && (int)rotate.z == 270
                || NodePosition.position + Vector2.right == playerNow.position && (int)rotate.z == 90)
            {
                //狙击手死亡
                player.ChangeState(Player.States.Attack);
                ChangeState(States.Die);
            }
            //广播玩家位置
            else
            {
                EventCenter.GetInstance().BroadcastEvent<Node,Vector2,float>(EventType.PlayerFoundPartly, playerNow, transform.position, 5f);
                Debug.Log("broadcast" + playerNext.position);
            }
        }
    }
    
    #endregion
    
    public void LoadData(GameData data)
    {
        data.sniperAlive.TryGetValue(id, out var isAlive);
        switch (isAlive)
        {
            case false:
                gameObject.SetActive(false);
                break;
            case true:
                gameObject.SetActive(true);
                break;
        }

        data.sniperRotate.TryGetValue(id, out var rot);
        rotate = rot;
        
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
        
        rangeInstruction.eulerAngles = rotate;

    }

    public void SaveData(GameData data)
    {
        if (data.sniperAlive.ContainsKey(id))
        {
            
            data.sniperAlive.Remove(id);
        }
        data.sniperAlive.Add(id, nowState != States.Die);

        if (data.sniperRotate.ContainsKey(id))
        {
            data.sniperRotate.Remove(id);
        }
        data.sniperRotate.Add(id, rotate);
    }
}
