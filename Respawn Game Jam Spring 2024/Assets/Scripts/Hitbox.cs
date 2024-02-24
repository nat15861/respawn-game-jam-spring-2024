using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Hitbox : MonoBehaviour
{
    private Transform owner;

    public LayerMask target;


    private float width;

    private float height;

    // Angle of the hitbox (box shape only)
    private float angle;


    private Vector2 knockbackDirection;

    private float damage;

    private float knockback;

    // Duration that the hitbox will last for before it is destroyed
    private float duration;

    // The hitbox has no duration and will persist until the component is destroyed by something else
    private bool persistent;

    public bool disabled;

    // The transform that will be used to calculate knockback on incoming colliders
    // If left null, the provided knockbackDirection will be used
    private Transform calculateAngleTransform;

    // The hitbox will check for collisions every frame instead of being called manually
    private bool automatic;

    // The hitbox will repeatedly call the damage function on an inbound entity even if the collider hasn't exited the bounding box first
    private bool continuous;

    // The hitbox will alert its owner if there is a collision
    private bool alertOnHit;

    // The shape of the hitbox will be a circle instead of a box
    private bool circleHitbox;

    // The hitbox will call the damage function on entities that it hits, and will not ignore them
    private bool effectTarget;


    [SerializeField]
    private bool hit;


    public List<Collider2D> inBoundColliders = new List<Collider2D>();


    // Start is called before the first frame update
    public void Init(Transform owner, LayerMask target, Vector2 relativePos, Vector2 dimensions, float angle, float damage, Vector2 knockbackDirection, float knockback, float duration, bool automatic = true, bool continuous = false, bool alertOnHit = false, Transform calculateAngleTransform = null, bool circleHitbox = false, bool effectTarget = true)
    {
        this.owner = owner;

        this.target = target;

        this.angle = angle;

        this.knockbackDirection = knockbackDirection;

        this.damage = damage;

        this.knockback = knockback;

        this.duration = duration;

        this.automatic = automatic;

        this.continuous = continuous;

        this.alertOnHit = alertOnHit;

        this.calculateAngleTransform = calculateAngleTransform;

        this.circleHitbox = circleHitbox;

        this.effectTarget = effectTarget;

        if (duration == -1)
        {
            persistent = true;
        }


        transform.position = owner.position + (Vector3)relativePos + new Vector3(0, 0, -1);

        UpdateSize(dimensions);        
    }

    // Update is called once per frame
    void Update()
    {
        if (automatic)
        {
            Refresh();
        }
    }

    public List<string> Refresh()
    {
        if (!persistent)
        {
            duration -= Time.deltaTime;

            if (duration <= 0)
            {
                Destroy(gameObject);

                return new List<string>();
            }
        }


        if (!disabled)
        {
            return CheckCollisions();
        }

        return new List<string>();
    }


    private List<string> CheckCollisions()
    {
        //List of colliders that are currently in the bounding area
        Collider2D[] colliders;

        if (circleHitbox)
        {
            colliders = Physics2D.OverlapCircleAll(transform.position, width, target);
        }
        else
        {
            colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(width, height), angle, target);
        }


        List<int> stillInboundIndexes = new List<int>();

        List<string> newColliderLayers = new List<string>();

        // If the collider is already in inBoundColliders, we don't need to collide with it again. Instead, add its index in inBoundColliders to the stillInboundIndexes list

        // Otherwise, if the collider just appeared in the bounding box, add it to the in bounds list, grab its new index in inBoundColliders, and collide with it
        // (Doing things in this order to avoid issues that might arise if the collider is destroyed or becomes null when the Damage() function is called
        for (int i = 0; i < colliders.Length; i++)
        {
            if (inBoundColliders.Contains(colliders[i]))
            {
                stillInboundIndexes.Add(inBoundColliders.IndexOf(colliders[i]));

                if (continuous)
                {
                    Collide(colliders[i]);
                }

                continue;
            }


            inBoundColliders.Add(colliders[i]);

            stillInboundIndexes.Add(inBoundColliders.Count - 1);

            newColliderLayers.Add(LayerMask.LayerToName(colliders[i].gameObject.layer));

            Collide(colliders[i]);
        }

        // After we know which colliders are still in the bounding box, remove all of the ones that aren't
        // Also remove a collider if it has become null since calling the Collide() function
        for (int i = inBoundColliders.Count - 1; i >= 0; i--)
        {
            if (!stillInboundIndexes.Contains(i) || inBoundColliders[i] == null)
            {
                inBoundColliders.RemoveAt(i);
            }
        }

        return newColliderLayers;
    }

    private void Collide(Collider2D collider)
    {
        hit = true;


        // Realized that with our setup, the entity component won't always be on the same gameobject as the collider
        // Temporary solution for now, we can think of a better one later
        Entity entity = null;

        if (collider.GetComponent<Entity>() != null)
        {
            entity = collider.GetComponent<Entity>();
        }
        else if(collider.transform.parent.GetComponent<Entity>() != null)
        {
            entity = collider.transform.parent.GetComponent<Entity>();
        }


        if (entity != null)
        {
            if (calculateAngleTransform != null)
            {
                knockbackDirection = (collider.transform.position - calculateAngleTransform.position).normalized;
            }

            if (effectTarget)
            {
                entity.Damage(damage, knockback, knockbackDirection);
            }
        }


        if (alertOnHit)
        {
            //owner.GetComponent<IHitboxAlertable>().RegisterHit(this, collider.transform);
        }
    }


    public bool CheckHit()
    {
        return hit;
    }

    public void UpdateAngle(float newAngle)
    {
        angle = newAngle;

        transform.up = new Vector2(Mathf.Cos((angle + 90) * Mathf.Deg2Rad), Mathf.Sin((angle + 90) * Mathf.Deg2Rad));
    }

    public void UpdateSize(Vector2 dimensions)
    {
        width = dimensions.x;

        height = dimensions.y;

        // We need the dimensions of the hitbox to match exactly with the ones passed in the parameters,
        // but if we just set the local scale directly, the hitboxes on enemies would be messed up because those hitboxes are parented to the enemy,
        // so setting the local scale to the exact dimensions of the enemy would multiply the parent scale (which might not be 1x1),
        // by the dimensions. The result is that the hitbox dimensions would be multiplied together and be a different size from the one we want

        // To fix this, temporarily un-parent the hitbox so that it is in world space, and then set the scale to the correct size
        // If we then re-parent the hitbox, the local scale will automatically adjust so that the actual scale stays the same
        // For enemies with scaled transforms, this will result in a local position of 1x1 and a real scale that is exactly equal to the enemy scale

        //*** CHECK THAT THIS WORKS WHEN PARENT SCALE IS 0 AND BOX COLLIDER IS NOT 1X1

        // Check to make sure the hitbox was orginally parented in the first place before we put it back
        // If it wasn't, we still need to set the scale, but we just won't parent it when we're done
        bool hasParent = transform.parent != null;

        //NOTE: This assumes that the owner and the transform parent are the same
        transform.parent = null;

        transform.localScale = new Vector3(width, height, 1);

        transform.up = new Vector2(Mathf.Cos((angle + 90) * Mathf.Deg2Rad), Mathf.Sin((angle + 90) * Mathf.Deg2Rad));


        if (hasParent)
        {
            transform.parent = owner;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(width, height));
    }
}
