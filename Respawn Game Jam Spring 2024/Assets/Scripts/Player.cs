using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    private Game game;


    private Rigidbody2D rb;

    private Transform spriteTransform;

    private Transform lineTransform;


    private int rotationDirection;

    public float rotationSpeed = 10;

    private bool swimming = false;

    public float swimSpeed = 5;

    private bool swinging = false;

    public float idleSwingSpeed = 50;

    public float swingSpeed = 100;

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();


        rb = GetComponent<Rigidbody2D>();

        spriteTransform = transform.Find("Sprite");

        lineTransform = transform.Find("Line");
    }

    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        // Rotating line

        swinging = Input.GetMouseButton(0);


        Vector2 mousePos = game.cam.ScreenToWorldPoint(Input.mousePosition);

        spriteTransform.up = (mousePos - (Vector2)transform.position).normalized;


        // Rotating 
        
        int rotateDirection = 0;

        // New key presses go first, with right proity if called on the same frame
        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            rotateDirection = -1;
        }
        else if (Input.GetKeyDown("left") || Input.GetKeyDown("a"))
        {
            rotateDirection = 1;
        }

        // If there are no new key presses but both are being held down, just use the one that we used previously
        else if ((Input.GetKey("right") || Input.GetKey("d")) && (Input.GetKey("left") || Input.GetKey("a")))
        {
            rotateDirection = rotationDirection;
        }

        // Otherwise just check for held key presses
        else if (Input.GetKey("right") || Input.GetKey("d"))
        {
            rotateDirection = -1;
        }

        else if (Input.GetKey("left") || Input.GetKey("a"))
        {
            rotateDirection = 1;
        }

        rotationDirection = rotateDirection;


        // Swimming forwards

        swimming = Input.GetKey("up") || Input.GetKey("w");
    }

    private void FixedUpdate()
    {
        //spriteTransform.Rotate(rotationSpeed * rotationDirection * Vector3.forward * Time.fixedDeltaTime);

        if (swimming)
        {
            rb.velocity = swimSpeed * spriteTransform.up;
        }

        if (swinging)
        {
            lineTransform.Rotate(-swingSpeed * Vector3.forward * Time.fixedDeltaTime);
        }
        else
        {
            lineTransform.Rotate(-idleSwingSpeed * Vector3.forward * Time.fixedDeltaTime);
        }
    }
}
