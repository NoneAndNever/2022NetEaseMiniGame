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
        Node[,] graph = pathFinding.GraphNodes;
        if (!IsMoving)
        {
        
            

            //敌人移动
            Node nextPos;
            foreach (Transform enemy in EnemiesTrans)
            {
                //移动方向判定
                /*if (enemy.position.y < final.y)
                    dir = (int)enemy.position.x != (int)final.x
                        ? (enemy.position.x < final.x ? Vector3.right : Vector3.left)
                        : Vector3.up;
                else if (enemy.position.y > final.y) dir = Vector3.down;
                else dir = enemy.position.x < final.x ? Vector3.right : Vector3.left;*/
                //开始移动
                nextPos = paths[enemy.gameObject].Pop();
                enemy.DOMove(nextPos.position, moveTime);
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
