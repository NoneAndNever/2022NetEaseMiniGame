using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 移动控制器
/// </summary>
public class MovementCtrl: BaseManager<MovementCtrl>
{

    public Transform PlayerTrans;//玩家
    public List<Transform> EnemiesTrans = new List<Transform>();//所有敌人
    
    public bool IsMoving { get; private set; }//移动状态
    private readonly float moveTime = 0.2f;//移动时间

    public Dictionary<GameObject ,Stack<Node>> paths = new Dictionary<GameObject ,Stack<Node>>();
    
    private readonly AStarPathFinding pathFinding = AStarPathFinding.GetInstance();
    

    
    /// <summary>
    /// 角色与敌人的移动
    /// </summary>
    /// <param name="playerNode">玩家移动方向</param>
    public void Moving(Node playerNode)
    {
        if (!IsMoving)
        {
            //敌人移动
            Node nextPos;
            foreach (Transform enemy in EnemiesTrans)
            {
                //获取移动方向
                nextPos = paths[enemy.gameObject].Pop();
                //开始移动
                enemy.DOMove(nextPos.position, moveTime);
                //更新敌人的A*路径
                paths[enemy.gameObject] = pathFinding.FindPath(nextPos, playerNode);
            }
            //玩家移动
            PlayerTrans.DOMove(playerNode.position, moveTime).OnComplete(Reset);
            //锁定移动状态
            IsMoving = true;
        }
    }

    /// <summary>
    /// 重置移动条件
    /// </summary>
    private void Reset()
    {
        IsMoving = false;
    }
}
