using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Entity
{
    public Hitbox hitboxPref;

    public Timer timerPref;


    private Player player;

    private Rigidbody2D rb;

    private BoxCollider2D bc;


    private Hitbox hitbox;

    private Timer knockbackTimer;


    public float swimSpeed = 1;

    public State state = State.Swimming;

    public enum State
    {
        Swimming,
        Recoiling
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        rb = GetComponent<Rigidbody2D>();

        bc = GetComponent<BoxCollider2D>();


        hitbox = Instantiate(hitboxPref, transform);

        hitbox.Init(transform, LayerMask.GetMask("Player"), Vector2.zero, (Vector2)transform.lossyScale, GetAngle(), damage, Vector2.zero, 5, -1);


        knockbackTimer = Instantiate(timerPref, transform);

        knockbackTimer.Init("Knockback Timer", .5f);
    }

    void Update()
    {
        if(state == State.Recoiling)
        {
            if (knockbackTimer.IsFinished(false))
            {
                state = State.Swimming;
            }

            return;
        }

        //hitbox.UpdateAngle(GetAngle());
    }

    private void FixedUpdate()
    {
        if (state == State.Recoiling) return;


        Vector2 playerDirection = ((Vector2)(player.transform.position - transform.position)).normalized;

        transform.right = playerDirection;

        rb.velocity = swimSpeed * transform.right;
    }

    public override void Damage(float damage, float knockbackStrength, Vector2 knockbackDirection)
    {
        health -= damage;

        rb.velocity = knockbackStrength * knockbackDirection;

        state = State.Recoiling;

        knockbackTimer.Begin();
    }

    private float GetAngle()
    {
        return Mathf.Rad2Deg * Mathf.Atan2(transform.up.y, transform.up.x);
    }
}
