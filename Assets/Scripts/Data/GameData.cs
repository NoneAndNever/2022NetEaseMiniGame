using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int roundCount;//回合数

    public Vector2 PlayerNode;
    
    public SerializableDictionary<string, bool> sniperAlive;//狙击手的存活状态
    public SerializableDictionary<string, Vector3> sniperRotate;//狙击手的旋转角度

    public SerializableDictionary<string, Vector2> scoutNodePosition;//侦察兵的位置
    public SerializableDictionary<string, Vector2> scoutTargetPosition;//侦察兵的目标位置

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        roundCount = 0;
        PlayerNode = Vector2.zero;
        sniperAlive = new SerializableDictionary<string, bool>();
        sniperRotate = new SerializableDictionary<string, Vector3>();
        scoutNodePosition = new SerializableDictionary<string, Vector2>();
        scoutTargetPosition = new SerializableDictionary<string, Vector2>();
    }
}
