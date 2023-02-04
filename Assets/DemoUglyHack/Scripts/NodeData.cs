using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct WeirdNodeData
{

    public WeirdNodeData(string title)
    {
        Title = title;
        LinksTo = new List<string>();
    }

    public string Title { get; }
    public List<string> LinksTo { get; }

    public override string ToString() => $"{Title}: [{string.Join(", ", LinksTo)}]";
}