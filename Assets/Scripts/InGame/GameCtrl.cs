using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameCtrl : SingletonMono<GameCtrl>
{
    [SerializeField] private List<Vector2> keyList;
    [SerializeField] private List<EventInfo> eventList;

    private bool isInUI = false;
    private GameObject uiCam;
    public Vector2 lastVec;
    private bool isGetSignal;


    private void Start()
    {
        CheckEvent(GameObject.FindWithTag("Player").transform.position);
    }

    private void Update()
    {
        if(!isInUI) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isInUI = false;
            uiCam.SetActive(false);
            var index = SearchExist(lastVec);
            if(index==-1)
                StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
            else
                DoSomething(index);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (keyList.Count != eventList.Count)
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                           + keyList.Count + ") does not match the number of values (" + eventList.Count 
                           + ") which indicates that something went wrong");
        }
        EventCenter.GetInstance().AddListener<Vector2>(EventType.Extra,CheckEvent);
    }

    private void OnDisable()
    {
        EventCenter.GetInstance().RemoveListener<Vector2>(EventType.Extra,CheckEvent);
    }

    public void CheckEvent(Vector2 vec2)
    {
        var index = SearchExist(vec2);
        lastVec = vec2;
        if (index==-1)
        {
            StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
            
            return;
        }
        DoSomething(index);
        
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

    private bool CheckCollections()
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            if (eventList[i].eventBehaviour == EventBehaviour.GetCollection)
                return false;
        }

        return true;
    }

    private void TriggerDial(int lineNum)
    {
        DialogueManager.GetInstance().SetLine(lineNum).ShowText();
    }

    public void DoSomething(int index)
    {
        switch (eventList[index].eventBehaviour)
        {
            case EventBehaviour.Dial:
                TriggerDial(eventList[index].argument);
                break;
            case EventBehaviour.InteractableObj:
                isInUI=true;
                uiCam = eventList[index].obj;
                uiCam.SetActive(true);
                break;
            case EventBehaviour.NextLevel:
                if (CheckCollections())
                {
                    int sceneNum = SceneManager.GetActiveScene().buildIndex + 1;
                    SceneManager.LoadScene(sceneNum);
                    EventCenter.GetInstance().BroadcastEvent<int>(EventType.ChangeMusic, sceneNum);
                }
                else 
                    StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
                break;
            case EventBehaviour.SaveGame:
                DataPersistenceManager.GetInstance().SaveGame();
                StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
                break;
            case EventBehaviour.GetCollection:
                eventList[index].obj.SetActive(false);
                StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
                break;
            case EventBehaviour.GetSignal:
                isGetSignal = true;
                break; 
        }

        if (eventList[index].eventBehaviour != EventBehaviour.NextLevel)
        {
            keyList.RemoveAt(index);
            eventList.RemoveAt(index);
        }
    }
}

[Serializable]
public enum EventBehaviour
{
    Dial,
    InteractableObj,
    NextLevel,
    SaveGame,
    GetCollection,
    GetSignal
}

[System.Serializable]
public class EventInfo
{
    public EventBehaviour eventBehaviour;
    public int argument;
    [AllowNull] public GameObject obj;
}

