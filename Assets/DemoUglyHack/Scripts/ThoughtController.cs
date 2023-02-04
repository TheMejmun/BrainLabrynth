using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtController : MonoBehaviour
{

    Rigidbody2D rb;
    float walkSpeed = 4f;
    float speedLimit = 0.7f;
    float verticalInput;
    float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if(verticalInput != 0 || horizontalInput != 0)
        {
            if (verticalInput != 0 && horizontalInput != 0)
            {
                horizontalInput *= speedLimit;
                verticalInput *= speedLimit;
            }
            transform.Rotate(0f, 0f, Random.Range(0f, 360f));
            rb.velocity = new Vector2(horizontalInput * walkSpeed, verticalInput * walkSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0f, 0f);
        }
    }
}
