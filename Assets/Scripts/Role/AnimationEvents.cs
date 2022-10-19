using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject role;
    [SerializeField] private DataPersistenceManager data;
    
    public void RoleDie()
    {
        role.SetActive(false);
    }

    public void ReBorn()
    {
        data.LoadGame();
    }
}