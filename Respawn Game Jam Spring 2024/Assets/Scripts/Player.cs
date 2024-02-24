using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Entity
{
    public Timer timerPref;

    
    private Game game;


    private Rigidbody2D rb;

    private Transform spriteTransform;

    private Transform lineTransform;

    private Timer knockbackTimer;


    private int rotationDirection;

    public float rotationSpeed = 10;

    private bool swimming = false;

    public float swimSpeed = 5;

    private bool swinging = false;

    public float idleSwingSpeed = 50;

    public float swingSpeed = 100;

    // For clarity, this is the force at which we knock other things back
    public float knockback = 7;

    // This is the amount of time we will recoil when we are hit by other things
    public float knockbackTime = .3f;


    public State state = State.Swimming;

    public enum State
    {
        Swimming,
        Recoiling
    }

    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();


        rb = GetComponent<Rigidbody2D>();

        spriteTransform = transform.Find("Sprite");

        lineTransform = transform.Find("Line");


        knockbackTimer = Instantiate(timerPref, transform);

        knockbackTimer.Init("Knockback Timer", knockbackTime);
    }

    void Update()
    {
        if (state == State.Recoiling)
        {
            if (knockbackTimer.IsFinished(false))
            {
                state = State.Swimming;
            }

            return;
        }

        GetInput();
    }

    private void GetInput()
    {
        // Rotating line
        swinging = Input.GetMouseButton(0);

        // Swimming forwards
        swimming = Input.GetKey("up") || Input.GetKey("w");
    }

    private void FixedUpdate()
    {
        if (state == State.Recoiling) return;


        // Rotating the player
        Vector2 mousePos = game.cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 targetDirection = (mousePos - (Vector2)transform.position).normalized;

        Quaternion targetQuanternion = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x) -90);

        // Higher speed the further away the two direction vectors are. Minimum of .2
        float rotationMultiplier = 1 - ((Vector2.Dot(targetDirection, spriteTransform.up) / 2) + .5f) +.2f;

        spriteTransform.rotation = Quaternion.RotateTowards(spriteTransform.rotation, targetQuanternion, rotationSpeed * rotationMultiplier * Time.fixedDeltaTime);
        

        // Swimming
        if (swimming)
        {
            rb.velocity = swimSpeed * spriteTransform.up;
        }

        //Swinging the net
        if (swinging)
        {
            lineTransform.Rotate(-swingSpeed * Vector3.forward * Time.fixedDeltaTime);
        }
        else
        {
            lineTransform.Rotate(-idleSwingSpeed * Vector3.forward * Time.fixedDeltaTime);
        }
    }

    public override void Damage(float damage, float knockbackStrength, Vector2 knockbackDirection)
    {
        health -= damage;

        rb.velocity = knockbackStrength * knockbackDirection;

        state = State.Recoiling;

        knockbackTimer.Begin();
    }
}
