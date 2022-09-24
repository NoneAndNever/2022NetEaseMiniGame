using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : MonoBehaviour
{
    #region 变量定义

    [SerializeField] public Node PlayerPosition { get; set; }
    //移动控制器
    private readonly MovementCtrl movementCtrl = MovementCtrl.GetInstance();
    private readonly AStarPathFinding pathFinding = AStarPathFinding.GetInstance();

    #endregion

    private void Awake()
    {
        movementCtrl.PlayerTrans = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //当角色不在移时，进行位移动画插值
        if (!movementCtrl.IsMoving)
        {
            if (Input.GetKeyDown(KeyCode.W)) { PlayerPosition = pathFinding.GraphNodes[PlayerPosition.x, PlayerPosition.y+1];movementCtrl.Moving(PlayerPosition); }
            else if (Input.GetKeyDown(KeyCode.S)) { PlayerPosition = pathFinding.GraphNodes[PlayerPosition.x, PlayerPosition.y-1];movementCtrl.Moving(PlayerPosition); }
            else if (Input.GetKeyDown(KeyCode.D)) { PlayerPosition = pathFinding.GraphNodes[PlayerPosition.x+1, PlayerPosition.y];movementCtrl.Moving(PlayerPosition);}
            else if (Input.GetKeyDown(KeyCode.A)) { PlayerPosition = pathFinding.GraphNodes[PlayerPosition.x-1, PlayerPosition.y];movementCtrl.Moving(PlayerPosition);}

            
        }

    }

}
