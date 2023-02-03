using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

// https://gamedevbeginner.com/singletons-in-unity-the-right-way/#unity_singleton

public class WikiManager : MonoBehaviour
{
    private static string BASE_API_URL = "https://en.wikipedia.org/w/api.php";

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
            // TODO just testing
            print(GetLinksJSON("Brad Pitt", null));
        }
    }

    private string GetLinksJSON(string titles, string plcontinue)
    {
        string requestString = String.Format(BASE_API_URL + "?action=query&prop=links&format=json&plnamespace=0&titles={0}&pllimit=max", titles);
        if (plcontinue != null) {
            requestString = String.Format(requestString + "&plcontinue={0}", plcontinue);
        }
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestString);
        HttpWebResponse response = (HttpWebResponse) request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        return jsonResponse;
    }
}
