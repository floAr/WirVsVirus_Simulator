using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebBridge : MonoBehaviour
{
    // see https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html




    [DllImport("__Internal")]
    private static extern void PushJsonData(string str);


    void Start()
    {
  
    }
}

