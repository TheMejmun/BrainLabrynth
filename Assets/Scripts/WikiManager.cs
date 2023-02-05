using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// https://gamedevbeginner.com/singletons-in-unity-the-right-way/#unity_singleton

public class WikiManager : MonoBehaviour
{
    private static string BASE_API_URL = "https://en.wikipedia.org/w/api.php";

    private static WikiManager _instance;

    public static WikiManager Instance { get { return _instance; } }

    private Dictionary<String, NodeData> NodeDict = new Dictionary<String, NodeData>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            // TODO Testing
            print(GetRandomNode());
        }
    }

    public NodeData GetNode(string title)
    {
        if (!NodeDict.ContainsKey(title))
        {
            string plcontinue = ParseLinksJSON(GetLinksJSON(title, null));
            while (plcontinue != null)
            {
                plcontinue = ParseLinksJSON(GetLinksJSON(title, plcontinue));
            }
        }
        return NodeDict[title];
    }

    public NodeData GetRandomNode()
    {
        return GetNode(ParseRandomJSON(GetRandomJSON()));
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

    // https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp
    private string ParseLinksJSON(string json)
    {
        // print(json);
        string plcontinue = null;

        var root = JObject.Parse(json); // parse as array  
        foreach (KeyValuePair<String, JToken> item in root)
        {
            // GET CONTINUE STRING
            if (item.Key == "continue")
            {
                plcontinue = (String)item.Value["plcontinue"];
            }
            // PARSE PAGES
            else if (item.Key == "query")
            {
                foreach (JToken page in item.Value["pages"])
                {
                    foreach (JToken pageData in page.AsJEnumerable())
                    {
                        string title = (String) pageData.Value<JToken>("title");
                        // print(String.Format("title: {0}", title));

                        // Create entry if null
                        if (!NodeDict.ContainsKey(title))
                        {
                            NodeDict.Add(title, new NodeData(title));
                        }
                        NodeData nodeData = NodeDict[title];

                        // Get links
                        JToken links = pageData.Value<JToken>("links");
                        foreach (JToken linkData in links.AsJEnumerable())
                        {
                            string linkTitle = (String) linkData.Value<JToken>("title");
                            // print(String.Format("linkTitle: {0}", linkTitle));

                            // Save link titles to dict
                            nodeData.LinksTo.Add(linkTitle);
                        }
                    }
                }
            }
        }

        // print(String.Format("plcontinue: {0}", plcontinue));

        return plcontinue;
    }

    private string GetRandomJSON()
    {
        string requestString = BASE_API_URL + "?action=query&list=random&rnnamespace=0&rnlimit=1&format=json";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        return jsonResponse;
    }

    // https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp
    private string ParseRandomJSON(string json)
    {
        var root = JObject.Parse(json); // parse as array  
        foreach (KeyValuePair<String, JToken> item in root)
        {
            // PARSE PAGES
            if (item.Key == "query")
            {
                foreach (JToken page in item.Value["random"])
                {
                    foreach (JProperty pageData in page.AsJEnumerable())
                    {
                        // print(pageData);
                        if (pageData.Name == "title") { 
                            return (String)pageData.Value;
                        }
                    }
                }
            }
        }

        return null;
    }
}
