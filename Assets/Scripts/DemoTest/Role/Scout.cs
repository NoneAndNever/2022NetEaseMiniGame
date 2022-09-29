using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 巡逻兵
/// </summary>
public class Scout : Role
{

    #region 属性
    #endregion



    private void Awake()
    {
        
        MovementCtrl.Scouts.Add(this);
        MovementCtrl.paths.Add(this, null);
        EventCenter.GetInstance().AddListener<Node>(EventType.PlayerFound, SetPlayerNode);
        EventCenter.GetInstance().AddListener<Node, Vector2, float>(EventType.PlayerFoundPartly, SetPlayerNode);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        NodePosition = PathFinding.GraphNodes[(int)position.x, (int)position.y];
    }

    void Update()
    {

    }

    public Node GetPlayerNode()
    {
        return PlayerNode;
    }

    public override void Move()
    {
        base.Move();
    }
}
