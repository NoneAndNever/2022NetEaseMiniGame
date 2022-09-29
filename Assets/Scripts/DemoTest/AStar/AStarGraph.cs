using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AStarGraph : MonoBehaviour
{
    private static AStarPathFinding pathFinding = AStarPathFinding.GetInstance();
    private MovementCtrl movementCtrl = MovementCtrl.GetInstance();

    // Start is called before the first frame update
    void Start()
    {
        pathFinding.InitGraph();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pair in movementCtrl.paths)
        {
            var path = pair.Value?.ToArray();
            for (int i = 0; i < path?.Length - 1;i++)
            {
                Node current = path[i];
                Debug.DrawLine(current.position, current.Connection.position, Color.green);
            }
        }
    }
    
}
