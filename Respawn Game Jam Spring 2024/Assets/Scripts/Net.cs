using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    public Hitbox hitboxPref;

    public Player player;


    private CircleCollider2D cc;

    private Hitbox hitbox;


    public List<Item> capturedItems;

    public float itemScaleIncrease = .1f;

    void Start()
    {
        cc = GetComponent<CircleCollider2D>();

        
        hitbox = Instantiate(hitboxPref, transform);

        hitbox.Init(transform, LayerMask.GetMask("Fish"), Vector2.zero, cc.radius * Vector2.one, 0, player.damage, Vector2.zero, player.knockback, -1, calculateAngleTransform: player.transform, circleHitbox: true);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            collision.transform.position = transform.position;

            collision.transform.parent = transform;

            Item item = collision.GetComponent<Item>();

            item.Capture();

            capturedItems.Add(item);


            transform.localScale += itemScaleIncrease * new Vector3(1, 1, 0);

            hitbox.UpdateSize(cc.radius * Vector2.one);
        }
    }
}
