using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
    
{
    private float movementX = 0;
    private float moveSpeed = 1.5f;
    public Rigidbody2D myRigidbody;
    private bool isColliding;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * moveSpeed * Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space) && isColliding)
        {
            myRigidbody.velocity = Vector2.up * 5;
        }
    }

    void OnCollisionEnter2D(Collision2D collison)
    {
        isColliding = true;

        if (collison.transform.tag != "Player")
        {
            Debug.Log("Collision");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }
}
