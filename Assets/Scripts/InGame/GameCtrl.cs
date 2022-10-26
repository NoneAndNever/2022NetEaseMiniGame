using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameCtrl : MonoBehaviour
{
    [SerializeField] private List<Vector2> keyList;
    [SerializeField] private List<EventInfo> eventList;


    private void Awake()
    {
        if (keyList.Count != eventList.Count)
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                           + keyList.Count + ") does not match the number of values (" + eventList.Count 
                           + ") which indicates that something went wrong");
        }
        EventCenter.GetInstance().AddListener<Vector2>(EventType.Extra,CheckEvent);
    }

    private void CheckEvent(Vector2 vec2)
    {
        var index = SearchExist(vec2);
        if (index==-1)
        {
            StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
            return;
        }

        switch (eventList[index].eventBehaviour)
        {
            case EventBehaviour.Dial:
                TriggerDial(eventList[index].argument);
                break;
            case EventBehaviour.InteractableObj:
                break;
            case EventBehaviour.NextLevel:
                int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(sceneNum);
                EventCenter.GetInstance().BroadcastEvent<int>(EventType.ChangeMusic, sceneNum);
                break;
        }
        keyList.RemoveAt(index);
        eventList.RemoveAt(index);
    }

    private int SearchExist(Vector2 vec2)
    {
        for (int i = 0; i < keyList.Count; i++)
        {
            if ((int)keyList[i].x==(int)vec2.x &&
                (int)keyList[i].y==(int)vec2.y)
            {
                return i;
            }
        }

        return -1;
    }

    private void TriggerDial(int lineNum)
    {
        DialogueManager.GetInstance().SetLine(lineNum).ShowText();
    }
}

[Serializable]
public enum EventBehaviour
{
    Dial,
    InteractableObj,
    NextLevel
}

[System.Serializable]
public class EventInfo
{
    public EventBehaviour eventBehaviour;
    public int argument;
}
