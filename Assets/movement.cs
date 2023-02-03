using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    int velocity = 1;
       int speed = 1;
        int walkAcceleration = 1;
        int groundDeceleration = 1;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        float moveInput = Input.GetAxisRaw("Horizontal");
       


    }
}
