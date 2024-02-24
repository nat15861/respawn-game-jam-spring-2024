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
        


        //  interpolate rotation (we are in water)
        spriteTransform.up = Vector3.Lerp(spriteTransform.up, (mousePos - (Vector2)transform.position).normalized, Time.deltaTime * rotationSpeed);



		// Swimming forwards

		rb.velocity -= rb.velocity * 0.9f * Time.deltaTime;
		
        if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
        {
            rb.velocity = Vector2.Lerp(rb.velocity, swimSpeed * spriteTransform.up, Time.deltaTime * swimSpeed);
            //rb.velocity = swimSpeed * spriteTransform.up;
            DebugExtension.DebugArrow(transform.position, Vector2.Lerp(rb.velocity, swimSpeed * spriteTransform.up, Time.deltaTime * swimSpeed));
        }
	}

    //  Moving movement code out of fixedupdate (mitigates stutters)
	private void FixedUpdate()
    {
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
