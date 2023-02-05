using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartEndDisplayManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject textObject;
    private TMPro.TextMeshProUGUI displayText;
    public string from;
    public string to;
    void Start()
    {
        displayText = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        displayText.text = "Get from " + from + " to " + to;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
