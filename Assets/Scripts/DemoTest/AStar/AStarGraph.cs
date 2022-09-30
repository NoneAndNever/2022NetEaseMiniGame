using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AStarGraph : MonoBehaviour
{
    private static AStarPathFinding pathFinding = AStarPathFinding.GetInstance();
    
    // Start is called before the first frame update
    void Start()
    {
        pathFinding.InitGraph();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
