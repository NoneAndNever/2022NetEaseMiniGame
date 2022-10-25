using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject other;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.Raycast(transform.position, (other.transform.position-transform.position), 1f, 1<<8))
        {
            Debug.Log("test"+true);
        }
    }
}
