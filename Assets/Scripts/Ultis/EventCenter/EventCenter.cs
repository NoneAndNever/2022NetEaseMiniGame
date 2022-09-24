using System;
using System.Collections.Generic;


public delegate void CallBack();
public delegate void CallBack<in T>(T t);

public class EventCenter: BaseManager<EventCenter>
{
    private Dictionary<EventType, Delegate> eventTable = new Dictionary<EventType, Delegate>();
    /// <summary>
    /// 添加一个0参数函数监听一个类型的事件，当该事件触发时，会自动调用添加的函数
    /// </summary>
    /// <param name="eventType">希望监听的事件类型</param>
    /// <param name="callBack">用于监听的函数</param>
    public EventCenter AddListener(EventType eventType, CallBack callBack)
    {
        if (!eventTable.ContainsKey(eventType))
            //如果广播事件字典中不含该事件的键值对，则添加
        {
            eventTable.Add(eventType, null);
        }
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
            //如果存在且委托不为空 但是委托类型不对，则抛出错误
        {
            throw new Exception($"尝试事件为{eventType}添加不同类型委托，当前委托类型为{d.GetType()}，要添加的委托类型为{callBack.GetType()}");
        }
        //否则进行正常增加委托
        eventTable[eventType] = (CallBack)eventTable[eventType] + callBack;
        return this;
    }
    
    public EventCenter AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        if (!eventTable.ContainsKey(eventType))
            //如果广播事件字典中不含该事件的键值对，则添加
        {
            eventTable.Add(eventType, null);
        }
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType())
            //如果存在且委托不为空 但是委托类型不对，则抛出错误
        {
            throw new Exception($"尝试事件为{eventType}添加不同类型委托，当前委托类型为{d.GetType()}，要添加的委托类型为{callBack.GetType()}");
        } 
        //否则进行正常增加委托
        eventTable[eventType] = (CallBack<T>)eventTable[eventType] + callBack;
        return this;
    }

    /// <summary>
    /// 移除一个正在监听一个类型事件的0参数函数
    /// </summary>
    /// <param name="eventEnum">移除函数正在监听的事件类型</param>
    /// <param name="callBack">希望移除的函数</param>
    public void RemoveListener(EventType eventEnum, CallBack callBack)
    {
        if (eventTable.ContainsKey(eventEnum))
        {
            Delegate d = eventTable[eventEnum];
            if (d == null)
            {
                throw new Exception($"移除事件错误：事件{eventEnum}没有对应的委托类型");
            }
            if (d.GetType() != callBack.GetType())
            {
                throw new Exception($"移除事件错误：事件{eventEnum}没有对应的委托,当前委托为{d.GetType()}，要移除的事件为{callBack.GetType()}");
            }
        }
        else
        {
            throw new Exception($"移除事件错误：没有事件码{eventEnum}");
        }
        eventTable[eventEnum] = (CallBack)eventTable[eventEnum] - callBack;
        if (eventTable[eventEnum] == null)
            //如果委托为空，直接移除键值对
        {
            eventTable.Remove(eventEnum);
        }
    }
    
    public void RemoveListener<X>(EventType eventEnum, CallBack<X> callBack)
    {
        if (eventTable.ContainsKey(eventEnum))
        {
            Delegate d = eventTable[eventEnum];
            if (d == null)
            {
                throw new Exception($"移除事件错误：事件{eventEnum}没有对应的委托类型");
            }
            if (d.GetType() != callBack.GetType())
            {
                throw new Exception($"移除事件错误：事件{eventEnum}没有对应的委托,当前委托为{d.GetType()}，要移除的事件为{callBack.GetType()}");
            }
        }
        else
        {
            throw new Exception($"移除事件错误：没有事件码{eventEnum}");
        }
        eventTable[eventEnum] = (CallBack<X>)eventTable[eventEnum] - callBack;
        if (eventTable[eventEnum] == null)
        {
            eventTable.Remove(eventEnum);
        }
    }
    public EventCenter BroadcastEvent(EventType eventEnum)
    {
        //如果对应的事件不存在监听委托，则返回
        if (!eventTable.TryGetValue(eventEnum, out var d)) return this;
        //如果委托事件符合委托类型，则调用委托
        if (d is CallBack callBack)
        {
            callBack();
        }
        else
        {
            throw new Exception($"广播事件错误：事件{eventEnum}有不同类型的委托");
        }
        return this;
    }
    public void BroadcastEvent<T>(EventType eventEnum,T arg)
    {
        //如果对应的事件不存在监听委托，则返回
        if (!eventTable.TryGetValue(eventEnum, out var d)) return;
        //如果委托事件符合委托类型，则调用委托
        if (d is CallBack<T> callBack)
        {
            callBack(arg);
        }
        else
        {
            throw new Exception($"广播事件错误：事件{eventEnum}有不同类型的委托");
        }
    }
    

}
