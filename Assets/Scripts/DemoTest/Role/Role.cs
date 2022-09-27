using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Role : MonoBehaviour
{
    
    #region 变量定义

    [SerializeField] public Node NodePosition { get; set; }
    //移动控制器
    protected readonly MovementCtrl MovementCtrl = MovementCtrl.GetInstance();
    protected readonly AStarPathFinding PathFinding = AStarPathFinding.GetInstance();

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
