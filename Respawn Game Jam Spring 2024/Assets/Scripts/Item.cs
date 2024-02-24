using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody2D rb;

    private CircleCollider2D cc;
    
    private SpriteRenderer sr;

    public float sinkSpeed = 1;

    public State state = State.Sinking;

    public enum State
    {
        Sinking,
        Ensnared
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        cc = GetComponent<CircleCollider2D>();
        
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (state == State.Ensnared) return;

        rb.velocity = sinkSpeed * Vector2.down;
    }

    public void Capture()
    {
        cc.enabled = false;

        //sr.enabled = false;

        rb.velocity = Vector2.zero;

        rb.isKinematic = true;

        state = State.Ensnared;
    }
}
