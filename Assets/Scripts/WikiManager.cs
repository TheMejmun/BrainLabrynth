using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://gamedevbeginner.com/singletons-in-unity-the-right-way/#unity_singleton

public class WikiManager : MonoBehaviour
{

    private static WikiManager _instance;

    public static WikiManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
