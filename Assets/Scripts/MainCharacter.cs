using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
    
{
    float movementX = 0;
    float moveSpeed = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementX = Input.GetAxisRaw("Horizontal");

        transform.position += new Vector3(movementX, 0f, 0f) * moveSpeed * Time.deltaTime;

    }
}
