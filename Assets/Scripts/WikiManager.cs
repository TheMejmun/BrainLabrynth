using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

// https://gamedevbeginner.com/singletons-in-unity-the-right-way/#unity_singleton
public class WikiManager : MonoBehaviour
{
    private static string BASE_API_URL = "https://en.wikipedia.org/w/api.php";

    private static WikiManager _instance;

    public static WikiManager Instance { get { return _instance; } }

    private Dictionary<String, NodeData> NodeDict = new Dictionary<String, NodeData>();

    private GetNodeJob job;

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
            //RequestRandomNode((node) => { print(node); });
        }
    }

    private void Update()
    {
        if(job != null)
        {
            if (job.Update())
            {
                job = null;
            }
        }
    }

    public void Abort()
    {
        if (job != null)
        {
            job.Abort();
            job = null;
            print("Aborted job");
        }
    }

    public void RequestNode(string title, Action<NodeData> doAfter)
    {
        Abort();
        job = new GetNodeJob();
        job.title = title;
        job.doAfter = doAfter;
        job.Start();
    }

    public void RequestRandomNode(Action<NodeData> doAfter)
    {
        Abort();
        job = new GetNodeJob();
        job.doAfter = doAfter;
        job.Start();
    }

    private NodeData GetNode(string title)
    {
        Debug.Log("Fetching Node " + title);

        if (!NodeDict.ContainsKey(title))
        {
            string plcontinue = ParseLinksJSON(GetLinksJSON(title, null));
            while (plcontinue != null)
            {
                plcontinue = ParseLinksJSON(GetLinksJSON(title, plcontinue));
            }
            ReduceLinks(title);
        }
        return NodeDict[title];
    }

    private NodeData GetRandomNode()
    {
        return GetNode(ParseRandomJSON(GetRandomJSON()));
    }

    // TODO discuss maxCount
    private void ReduceLinks(string title, int maxCount = 5)
    {
        // This looks like infinite recursion, but as the Node has now been created already, it should not be a problem
        NodeData node = GetNode(title);

        if (node.LinksTo.Count <= maxCount)
            return;

        List<string> newLinksTo = new List<string>();

        // TODO decide on how to remove
        System.Random r = new System.Random();
        for(int i = 0; i < maxCount; ++i) { 
            int index  = r.Next(node.LinksTo.Count);

            string link = node.LinksTo[index];
            if (newLinksTo.Contains(link)) {
                // Go again if duplicate
                --i;
            }
            else
            {
                newLinksTo.Add(link);
            }
        }

        node.LinksTo.Clear();
        node.LinksTo.AddRange(newLinksTo);
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

    private class GetNodeJob : ThreadedJob
    {
        public string title = null;  // in
        public Action<NodeData> doAfter; // in
        public NodeData node; // out

        protected override void ThreadFunction()
        {
            // Debug.Log("Starting ThreadFunction");

            if (title == null)
            {
                node = WikiManager.Instance.GetRandomNode();
            }
            else
            {
                node = WikiManager.Instance.GetNode(title);
            }

            // Debug.Log("Finished ThreadFunction");
        }
        protected override void OnFinished()
        {
            doAfter(node);
        }
    }
}
