using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AStarGraph : MonoBehaviour
{
    private static AStarPathFinding pathFinding = AStarPathFinding.GetInstance();
    [SerializeField] private GameObject player;
    private GameObject[] enemies;
    private MovementCtrl movementCtrl = MovementCtrl.GetInstance();

    // Start is called before the first frame update
    void Start()
    {
        pathFinding.InitGraph();
        Vector3 pposition = player.transform.position;
        player.GetComponent<Player>().NodePosition = pathFinding.GraphNodes[(int)pposition.x, (int)pposition.y];
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Vector3 eposition = enemy.transform.position;
            enemy.GetComponent<Enemy>().NodePosition = pathFinding.GraphNodes[(int)eposition.x, (int)eposition.y];
            Stack<Node> _path = pathFinding.FindPath(enemy.GetComponent<Enemy>().NodePosition, player.GetComponent<Player>().NodePosition, false);
            movementCtrl.paths.Add(enemy, _path);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pair in movementCtrl.paths)
        {
            var path = pair.Value.ToArray();
            for (int i = 0; i < path.Length - 1;i++)
            {
                Node current = path[i];
                Debug.DrawLine(current.position, current.Connection.position, Color.green);
            }
        }
    }
}
