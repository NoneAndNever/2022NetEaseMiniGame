using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 直升机
/// </summary>
public class Helicopter : Role
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckPlayerNode(Node playerNode)
    {
        Vector2 playerPos = playerNode.position;
        float distance = (playerPos - (Vector2)transform.position).magnitude;
        if (distance <  2f)
        {
            //transform.DOMove();
        }
    }
}
