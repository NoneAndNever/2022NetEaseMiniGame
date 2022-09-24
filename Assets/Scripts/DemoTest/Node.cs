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
    
    public Vector3 position { get; }
    private bool isBlocked { get; set; }
    public List<Node> FourNeighbors = new List<Node>(4);
    public List<Node> EightNeighbors = new List<Node>(8);

    public Node Connection { get; set; }
    //起点到当前点所需的步数
    public int G { get; set; }
    //当前点到终点的最短步数
    public int H { get; set; }
    public int F => G + H;
    
    public Node(Vector3 vector3)
    {
        position = vector3;
        CheckObstacle();
    }

    /// <summary>
    /// 设置四向邻居
    /// </summary>
    /// <param name="node">邻居节点</param>
    /// <param name="direction">节点联通方向</param>
    public void SetNeighber(Node node, Direction direction)
    {
        if (direction == Direction.Four)
        {
            FourNeighbors.Add(node);
            node.FourNeighbors.Add(this);
        }
        else
        {
            EightNeighbors.Add(node);
            node.EightNeighbors.Add(this);
        }
    }

    /// <summary>
    /// 检测该节点是否被阻挡
    /// </summary>
    public void CheckObstacle()
    {
        Vector3 orign = position - Vector3.back;
        if (Physics2D.Raycast(position, Vector3.forward, 1f, 1<<8)) isBlocked = true;
        else isBlocked = false;
    }

    /// <summary>
    /// 计算与target目标节点的步程差（不计阻碍）【按网格行走】
    /// </summary>
    /// <param name="target">目标节点</param>
    /// <param name="direction">节点联通方向</param>
    /// <returns>与目标节点的最短距离</returns>
    public int GetDistance(Node target, Direction direction)
    {
        int xStep = (int)Mathf.Abs(position.x - target.position.x);
        int yStep = (int)Mathf.Abs(position.y - target.position.y);
        if (direction == Direction.Four) return xStep + yStep;
        else return xStep <= yStep ? yStep : xStep;
    }

    /// <summary>
    /// 获取非阻挡物的邻居节点
    /// </summary>
    /// <param name="direction">节点联通方向</param>
    /// <returns>可通行的邻居节点列表</returns>
    public List<Node> GetValidNeighbors(Direction direction)
    {
        List<Node> result = new List<Node>();
        List<Node> neighbors = direction == Direction.Four ? FourNeighbors : EightNeighbors;
        foreach (var neighbor in neighbors)
        {
            if (!neighbor.isBlocked)
            {
                result.Add(neighbor);
            }
        }
        return result;
    }
}
