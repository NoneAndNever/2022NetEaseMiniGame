public enum EventType
{
    PlayerFound,//广播玩家位置
    PlayerFoundPartly,//局部广播玩家位置
    
    SceneBegin,//场景开始
    DoingMove,//回合中移动
    RoundEnd,//回合结束检测
    RoundBegin,//回合开始检测
    Extra,//保存和cg回合
    ChangeMusic//切换场景时播放cg
}
