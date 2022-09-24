using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class AStarGraph : MonoBehaviour
{
    private AStarPathFinding _pathFinding = AStarPathFinding.GetInstance();
    [SerializeField] private GameObject _player;
    private GameObject[] _enemies;
    private MoveControl _moveControl = MoveControl.GetInstance();

    // Start is called before the first frame update
    void Start()
    {
        _pathFinding.InitGraph();
        Vector3 pposition = _player.transform.position;
        _player.GetComponent<Player>().PlayerPosition = _pathFinding.GraphNodes[(int)pposition.x, (int)pposition.y];
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in _enemies)
        {
            Vector3 eposition = enemy.transform.position;
            enemy.GetComponent<Enemy>().EnemyPosition = _pathFinding.GraphNodes[(int)eposition.x, (int)eposition.y];
            Stack<Node> _path = _pathFinding.FindPath(enemy.GetComponent<Enemy>().EnemyPosition, _player.GetComponent<Player>().PlayerPosition);
            _moveControl.paths.Add(enemy, _path);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pair in _moveControl.paths)
        {
            var path = pair.Value.ToArray();
            for (int i = 0; path.Length - i - 1 > 0;i++)
            {
                Node current = path[i];
                Debug.DrawLine(current.position, current.Connection.position, Color.green);
            }
        }
    }
}
