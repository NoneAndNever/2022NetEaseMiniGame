using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 移动控制器
/// </summary>
public class MovementCtrl: BaseManager<MovementCtrl>
{
    public int RoundNum = 0;//回合数

    public enum RoundState
    {
        RoundBegin,
        InTheRound,
        RoundEnd
    }

    public RoundState nowRoundState = RoundState.RoundBegin;

    private readonly EventCenter EventCenter = EventCenter.GetInstance();//广播事件管理器

    public IEnumerator NextRoundState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nowRoundState = (RoundState)(((int)nowRoundState + 1) % 3);
        switch (nowRoundState)
        {
            case RoundState.RoundBegin:
                EventCenter.BroadcastEvent(EventType.RoundBegin);
                break;
            case RoundState.InTheRound:
                EventCenter.BroadcastEvent(EventType.DoingMove);
                break;
            case RoundState.RoundEnd:
                RoundNum++;
                EventCenter.BroadcastEvent(EventType.RoundEnd);
                break;
        }
    }
}
