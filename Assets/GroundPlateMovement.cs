using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlateMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform character;
    public Camera camera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(character.position.x, -10, 0);
    }
}
