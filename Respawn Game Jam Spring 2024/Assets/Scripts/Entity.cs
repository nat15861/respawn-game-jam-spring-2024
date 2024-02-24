using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float health;

    public float damage;

    void Start()
    {

    }

    void Update()
    {

    }

    public abstract void Damage(float damage, float knockbackStrength, Vector2 knockbackDirection);
}
