using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图节点
/// </summary>
public class Node
{
    public enum Direction
    {
        Four,
        Eight
    }
    public readonly int x;
    public readonly int y;
    public readonly Vector2 position;
    public bool isBlocked;
    private readonly List<Node> fourNeighbors = new List<Node>(4);
    private readonly List<Node> eightNeighbors = new List<Node>(8);
    public Node Connection;
    public int number = 0;
    
    //起点到当前点所需的步数
    public int G;
    //当前点到终点的最短步数
    public int H;
    public int F => G + H;
    
    public Node(int x,int y)
    {
        this.x = x;
        this.y = y;
        this.position = new Vector3(x, y);
        CheckObstacle();
    }

    /// <summary>
    /// 设置四向邻居
    /// </summary>
    /// <param name="node">邻居节点</param>
    /// <param name="direction">节点联通方向</param>
    public void SetNeighbor(Node node, Direction direction)
    {
        if (isBlocked|| node.isBlocked ) return;
        if (direction == Direction.Four && !CheckObstacle(node))
        {
            fourNeighbors.Add(node);
            node.fourNeighbors.Add(this);
        }
        else
        {
            eightNeighbors.Add(node);
            node.eightNeighbors.Add(this);
        }
    }

    /// <summary>
    /// 检测该节点是否被阻挡
    /// </summary>
    private void CheckObstacle()
    {
        isBlocked = Physics2D.Raycast(position, Vector2.zero, 1f, 1<<8);
    }
    
    /// <summary>
    /// 检测该节点是否被阻挡
    /// </summary>
    private bool CheckObstacle(Node other)
    {
        return  Physics2D.Raycast(position, (other.position-position), 1f, 1<<8);
    }

    /// <summary>
    /// 计算与target目标节点的步程差（不计阻碍）【按网格行走】
    /// </summary>
    /// <param name="target">目标节点</param>
    /// <param name="direction">节点联通方向</param>
    /// <returns>与目标节点的最短距离</returns>
    public int GetDistance(Node target, Direction direction)
    {
        int xStep = Mathf.Abs(x - target.x);
        int yStep = Mathf.Abs(y - target.y);
        
        if (direction == Direction.Four) return xStep + yStep;
        
        return xStep <= yStep ? yStep : xStep;
    }

    /// <summary>
    /// 获取非阻挡物的邻居节点
    /// </summary>
    /// <param name="direction">节点联通方向</param>
    /// <returns>可通行的邻居节点列表</returns>
    public List<Node> GetValidNeighbors(Direction direction)
    {
        return direction==Direction.Four? fourNeighbors: eightNeighbors;
    }

    /// <summary>
    /// 获取直线距离（平方）
    /// </summary>
    /// <param name="foundPosition"></param>
    /// <returns></returns>
    public float GetStraightDistance(Vector2 foundPosition)
    {
        return (foundPosition - position).sqrMagnitude;
    }
    
}
