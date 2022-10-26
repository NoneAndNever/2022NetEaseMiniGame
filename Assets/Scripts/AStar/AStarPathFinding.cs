using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// A*寻路（以左下角为(0,0)点正数展开）
/// </summary>
public class AStarPathFinding : BaseManager<AStarPathFinding>
{
    private Node[,] GraphNodes;
    private List<Node> _obstacles;
    private Vector2 _begin;

    private Node transverseNode;//横向的节点
    private Node verticalNode;//纵向的节点

    /// <summary>
    /// 初始化地图点
    /// </summary>
    public void InitGraph(Vector2 begin, int width, int length)
    {
        _begin = begin;
        GraphNodes = new Node[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                Node node = new Node((int)_begin.x + i, (int)_begin.y + j);
                //Node node = new Node(i, j);
                if (j != 0) node.SetNeighbor(GraphNodes[i, j - 1], Node.Direction.Four);
                if (i != 0) node.SetNeighbor(GraphNodes[i - 1, j], Node.Direction.Four);
                GraphNodes[i, j] = node;
            }
        }
    }

    /// <summary>
    /// 通过坐标获取地图点
    /// </summary>
    /// <param name="nodeX"></param>
    /// <param name="nodeY"></param>
    /// <returns></returns>
    public Node GetGraphNode(int nodeX, int nodeY)
    {
        return GraphNodes[nodeX - (int)_begin.x, nodeY - (int)_begin.y];
    }

    /// <summary>
    /// 获取两个节点之间的最短路径
    /// </summary>
    /// <param name="startNode">起点</param>
    /// <param name="targetNode">目标点</param>
    /// <returns></returns>
    public Stack<Node> FindPath([CanBeNull] Node startNode, [CanBeNull] Node targetNode, int number)
    {
        if (startNode == targetNode) return null;

        //搜索目标节点列表
        List<Node> toSearch = new List<Node>() { startNode };
        //已经搜索过的节点列表
        List<Node> processed = new List<Node>();
        

        while (targetNode != null && toSearch.Any())
        {
            //设置f值最小的节点为当前current节点，并开始搜索
            Node current = toSearch[0];
            foreach (Node t in toSearch)
            {
                if (t == current || t.F >= current.F && (t.F != current.F || t.H > current.H)) continue;

                if (t.F < current.F || t.H < current.H) current = t;
                else
                {
                    
                    transverseNode = Mathf.Abs(targetNode.y - current.y) < Mathf.Abs(targetNode.y - t.y) ? t : current;
                    verticalNode = transverseNode == t ? current : t;
                    // int transverseDifference = Mathf.Abs(Mathf.Abs(transverseNode.x - targetNode.x) -
                    //                                      Mathf.Abs(transverseNode.y - targetNode.y));
                    // int verticalDifference = (Mathf.Abs(verticalNode.x - targetNode.x) -
                    //                           Mathf.Abs(verticalNode.y - targetNode.y));
                    int transverseDifference = Mathf.Abs(transverseNode.x - targetNode.x);
                    int verticalDifference = Mathf.Abs(verticalNode.y - targetNode.y);
                    
                    if (transverseDifference != verticalDifference)
                        current = transverseDifference < verticalDifference ? transverseNode : verticalNode;
                    else if (transverseDifference == verticalDifference)
                        current = transverseNode.y > targetNode.y ? verticalNode : transverseNode;

                    if (startNode != null && processed.Count == 1 && number % 2 == 0)
                        current = current == transverseNode ? verticalNode : transverseNode;
                }

            }

            processed.Add(current);
            toSearch.Remove(current);

            //到达目标节点后回溯获取整条路线
            if (current == targetNode)
            {
                Node currentPathTile = targetNode;
                Stack<Node> path = new Stack<Node>();
                while (currentPathTile != startNode)
                {
                    path.Push(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                }
                
                return path;
            }

            //没到目标节点就对邻居节点进行搜索
            foreach (Node neighbor in current.GetValidNeighbors(Node.Direction.Four))
            {
                //检查邻居节点是否已经被搜索过
                bool inProcessed = processed.Contains(neighbor);
                //检查邻居节点是否在搜索列表
                bool inSearch = toSearch.Contains(neighbor);
                //求出 若以当前路径到达邻居节点的步数
                int costToNeighbor = current.G + current.GetDistance(neighbor, Node.Direction.Four);

                //对未被搜索过的节点或者得到更短路径的邻居节点进行设置
                if ((!inSearch || costToNeighbor < neighbor.G) && (!inProcessed))
                {
                    //更新邻居节点的G值
                    neighbor.G = costToNeighbor;
                    //将邻居节点与当前节点连接
                    neighbor.Connection = current;

                    if (!inSearch)
                    {
                        //设置未被搜索过的节点的H值
                        neighbor.H = neighbor.GetDistance(targetNode, Node.Direction.Four);
                        //将未被搜索的节点添加到搜索目标列表中
                        toSearch.Add(neighbor);
                    }
                }
            }

        }

        return null;
    }
}