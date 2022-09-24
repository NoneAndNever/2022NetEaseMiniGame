using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Script;

public class Player : MonoBehaviour
{
    #region 变量定义
    //入口点
    [SerializeField] private Transform Entry;
    //出口
    [SerializeField] private Transform Export;
    //出口
    [SerializeField] public Node PlayerPosition { get; set; }
    //移动控制器
    private MoveControl _moveControl = MoveControl.GetInstance();
    private AStarPathFinding _pathFinding = AStarPathFinding.GetInstance();

    #endregion

    private void Awake()
    {
        _moveControl.playerTrans = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Entry.position;
    }

    // Update is called once per frame
    void Update()
    {
        //当角色不在移时，进行位移动画插值
        if (!_moveControl._isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W)) { PlayerPosition = _pathFinding.GraphNodes[(int)PlayerPosition.position.x, (int)PlayerPosition.position.y+1]; _moveControl.Moving(PlayerPosition);}
            else if (Input.GetKeyDown(KeyCode.S)) { PlayerPosition = _pathFinding.GraphNodes[(int)PlayerPosition.position.x, (int)PlayerPosition.position.y-1]; _moveControl.Moving(PlayerPosition); }
            else if (Input.GetKeyDown(KeyCode.D)) { PlayerPosition = _pathFinding.GraphNodes[(int)PlayerPosition.position.x+1, (int)PlayerPosition.position.y]; _moveControl.Moving(PlayerPosition);}
            else if (Input.GetKeyDown(KeyCode.A)) { PlayerPosition = _pathFinding.GraphNodes[(int)PlayerPosition.position.x-1, (int)PlayerPosition.position.y]; _moveControl.Moving(PlayerPosition);}

            
        }

    }

}
