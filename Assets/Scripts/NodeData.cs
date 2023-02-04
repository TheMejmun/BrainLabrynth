using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NodeData
{
    public NodeData(string title)
    {
        Title = title;
        LinksTo = new List<string>();
        ChildrenLoaded = false;
    }

    public string Title { get; }
    public List<string> LinksTo { get; }
    public bool ChildrenLoaded;

    public override string ToString() => $"{Title}";
}