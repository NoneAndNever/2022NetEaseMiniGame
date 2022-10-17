using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject role;
    
    public void RoleDie()
    {
        role.SetActive(false);
    }
}