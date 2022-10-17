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

    private WaitForSeconds roundEnd = new WaitForSeconds(0.7f);
    private WaitForSeconds roundBegin = new WaitForSeconds(0.1f);
    private WaitForSeconds inRound = new WaitForSeconds(0.5f);

    public enum RoundState
    {
        RoundBegin,
        InTheRound,
        RoundEnd
    }

    public RoundState nowRoundState = RoundState.RoundBegin;

    private readonly EventCenter EventCenter = EventCenter.GetInstance();//广播事件管理器

    public IEnumerator NextRoundState()
    {
        nowRoundState = (RoundState)(((int)nowRoundState + 1) % 3);
        switch (nowRoundState)
        {
            case RoundState.RoundBegin:
                yield return roundEnd;
                EventCenter.BroadcastEvent(EventType.RoundBegin);
                break;
            case RoundState.InTheRound:
                yield return roundBegin;
                EventCenter.BroadcastEvent(EventType.DoingMove);
                break;
            case RoundState.RoundEnd:
                yield return inRound;
                RoundNum++;
                EventCenter.BroadcastEvent(EventType.RoundEnd);
                break;
        }
    }
}
