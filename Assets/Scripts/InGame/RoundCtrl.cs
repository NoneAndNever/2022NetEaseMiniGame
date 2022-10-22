using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 移动控制器
/// </summary>
public class RoundCtrl: SingletonMono<RoundCtrl>, IDataPersistence
{
    public int RoundNum = 0;//回合数

    private WaitForSeconds roundEnd = new WaitForSeconds(0.3f);
    private WaitForSeconds roundBegin = new WaitForSeconds(0.1f);
    private WaitForSeconds inRound = new WaitForSeconds(0.4f);
    private readonly WaitForSeconds inSave = new WaitForSeconds(0.1f);

    public enum RoundState
    {
        RoundBegin,
        InTheRound,
        RoundEnd,
        Extra
    }

    private RoundState nowRoundState = RoundState.RoundBegin;

    //private readonly EventCenter EventCenter = EventCenter.GetInstance();//广播事件管理器

    public IEnumerator NextRoundState([CanBeNull] Node playerNode)
    {
        nowRoundState = (RoundState)(((int)nowRoundState + 1) % 4);
        switch (nowRoundState)
        {
            case RoundState.RoundBegin:
                yield return inSave;
                EventCenter.GetInstance().BroadcastEvent(EventType.RoundBegin);
                break;
            case RoundState.InTheRound:
                yield return roundBegin;
                EventCenter.GetInstance().BroadcastEvent(EventType.DoingMove);
                break;
            case RoundState.RoundEnd:
                yield return inRound;
                RoundNum++;
                EventCenter.GetInstance().BroadcastEvent(EventType.RoundEnd);
                break;
            case RoundState.Extra:
                yield return roundEnd;
                EventCenter.GetInstance().BroadcastEvent<Vector2>(EventType.Extra, playerNode.position);
                break;
        }
    }

    

    public void LoadData(GameData data)
    {
        RoundNum = data.roundCount;
        nowRoundState = RoundState.RoundBegin;
    }

    public void SaveData(GameData data)
    {
        data.roundCount = RoundNum;
    }
}
