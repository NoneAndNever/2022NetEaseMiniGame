using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 移动控制器
/// </summary>
public class MovementCtrl: BaseManager<MovementCtrl>
{

    public bool IsMoving;//移动状态
    public int RoundNum = 0;//回合数

    private readonly EventCenter EventCenter = EventCenter.GetInstance();//广播事件管理器


    
    /// <summary>
    /// 角色与敌人的移动
    /// </summary>
    public void Moving()
    {
        if (!IsMoving)
        {
            //所有单位进行移动
            EventCenter.BroadcastEvent(EventType.DoingMove);
            //锁定移动状态
            IsMoving = true;
        }
    }

    /// <summary>
    /// 重置移动条件
    /// </summary>
    private void Reset()
    {
        IsMoving = false;
    }
}
