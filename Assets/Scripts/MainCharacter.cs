using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
    
{
    private float movementX = 0;
    private float movementY = 0;
    private float moveSpeed = 1.5f;
    private bool isColliding;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(movementX, movementY, 0f) * moveSpeed * Time.deltaTime;
        
    }

    void OnCollisionEnter2D(Collision2D collison)
    {
        isColliding = true;

        if (collison.transform.tag != "Player")
        {
            //Debug.Log("Collision");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
