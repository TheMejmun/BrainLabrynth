using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCharacter : MonoBehaviour
    
{
    public Logic spawner;
    private float movementX = 0;
    private float movementY = 0;
    private float moveSpeed = 2.5f;

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");
        transform.position += new Vector3(movementX, movementY, 0f) * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "neuron")
        {
            spawner.onNeuronTriggered(collision.gameObject);
        }
    }
}
 