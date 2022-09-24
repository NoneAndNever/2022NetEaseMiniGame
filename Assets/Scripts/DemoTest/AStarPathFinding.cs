using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A*寻路（以左下角为(0,0)点正数展开）
/// </summary>
public class AStarPathFinding: BaseManager<AStarPathFinding>
{
    private int width;
    private int length;
    public Node[,] GraphNodes;
    private List<Node> obstacles;
    
    
    
    
    /// <summary>
    /// 初始化地图点
    /// </summary>
    public void InitGraph()
    {
        GraphNodes = new Node[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                Node node = new Node(i,j);
                if (j != 0) node.SetNeighbor(GraphNodes[i, j-1], Node.Direction.Four);
                if (i != 0) node.SetNeighbor(GraphNodes[i-1, j], Node.Direction.Four);
                GraphNodes[i, j] = node;
            }
        }
    }

    /// <summary>
    /// 获取两个节点之间的最短路径
    /// </summary>
    /// <param name="startNode">起点</param>
    /// <param name="targetNode">目标点</param>
    /// <returns></returns>
    public Stack<Node> FindPath(Node startNode, Node targetNode)
    {
        
        //搜索目标节点列表
        List<Node> toSearch = new List<Node>() { startNode };
        //已经搜索过的节点列表
        List<Node> processed = new List<Node>();

        while (true)
        {
            //设置f值最小的节点为当前current节点，并开始搜索
            Node current = toSearch[0];
            foreach (Node t in toSearch)
            {
                //current节点与目标节点构成的矩形的边长差
                int currentRoadDifference = Mathf.Abs(Mathf.Abs(current.x - targetNode.x) - Mathf.Abs(current.y - targetNode.y)); 
                //t节点与目标节点构成的矩形的边长差
                int tRoadDifference = Mathf.Abs(Mathf.Abs(t.x - targetNode.x) - Mathf.Abs(t.y - targetNode.y)); 
                //判断该走哪一个邻居节点
                if (t.F < current.F
                    //当期望最短距离相等时，判断两个节点与目标节点间的直线距离
                    || t.F == current.F && t.H < current.H
                    || t.F == current.F && t.H == current.H && currentRoadDifference >= tRoadDifference)
                    //判断矩形边长差（选择差值更小的那个节点，表示沿最长边走）
                {
                    
                    if (t.F == current.F && t.H == current.H && currentRoadDifference == tRoadDifference) ;
                }
                

            }
            processed.Add(current);
            toSearch.Remove(current);
            
            //到达目标节点后回溯获取整条路线
            if (current == targetNode) {
                Node currentPathTile = targetNode;
                Stack<Node> path = new Stack<Node>();
                while (currentPathTile != startNode) {
                    path.Push(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                }
                return path;
            }
      
            //没到目标节点就对邻居节点进行搜索
            foreach (Node neighbor in current.GetValidNeighbors(Node.Direction.Four)) {
                //检查邻居节点是否已经被搜索过
                bool inProcessed = processed.Contains(neighbor);
                //检查邻居节点是否在搜索列表
                bool inSearch = toSearch.Contains(neighbor);
                //求出 若以当前路径到达邻居节点的步数
                int costToNeighbor = current.G + current.GetDistance(neighbor, Node.Direction.Four);
      
                //对未被搜索过的节点或者得到更短路径的邻居节点进行设置
                if ((!inSearch || costToNeighbor < neighbor.G) && (!inProcessed)) { 
                    //更新邻居节点的G值
                    neighbor.G = costToNeighbor;
                    //将邻居节点与当前节点连接
                    neighbor.Connection = current;

                    if (!inSearch) {
                        //设置未被搜索过的节点的H值
                        neighbor.H = neighbor.GetDistance(targetNode, Node.Direction.Four);
                        //将未被搜索的节点添加到搜索目标列表中
                        toSearch.Add(neighbor);
                    }
                }
                Debug.Log(neighbor.position);
            }
       
        }
        return null;
    }
}
