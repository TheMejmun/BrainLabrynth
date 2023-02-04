using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovemet : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform character;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(character.position.x, character.position.y, -10);
    }
}
