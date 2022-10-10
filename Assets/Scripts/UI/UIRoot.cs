
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主要的canvas(Overlay)，附带基本的UI池
/// </summary>
public class UIRoot: SingletonAutoMono<UIRoot>
{
    /*public Transform Transform;//画布*/
    private Dictionary<PanelType, GameObject> uiPool;
    

    
}
