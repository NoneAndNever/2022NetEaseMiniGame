using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AStarGraph : MonoBehaviour
{
    public int width;
    public int length;
    public Transform AStar;
    
    private static AStarPathFinding pathFinding = AStarPathFinding.GetInstance();
    
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD
        pathFinding.InitGraph();
=======
        pathFinding.InitGraph(AStar.position, width, length);
>>>>>>> origin/PatrickStar
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
