using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlateMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform character;
    public Camera myCamera;
    void Start()
    {
        Vector2 groundCoordinate = myCamera.ScreenToWorldPoint(new Vector2(0, 0));
        transform.position = new Vector3(character.position.x, groundCoordinate.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float updateVal = character.position.x - transform.position.x;
        transform.position += new Vector3(updateVal, 0f, 0f);
    }
}
