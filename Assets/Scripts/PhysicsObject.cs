using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PhysicsObject : MonoBehaviour
{
    public const float minimumHitThreshold = 25f;
    [HideInInspector,SerializeField]
    private new Transform transform;
    public Transform Transform => transform ? transform : transform = base.transform;
    [HideInInspector,SerializeField]
    private new Collider2D collider;
    [ShowInInspector,InfoBox("No Collider on Object",InfoMessageType.Error,VisibleIf = "NoColliderAttached")]
    public Collider2D Collider
    {
        get
        {
            if (collider != null) return collider;
            TryGetComponent(out collider);
            return collider;
        }
    }
    public Rigidbody2D rb;
    public float hitVelocityMultiplier;
    public virtual float HitVelocityMultiplier => hitVelocityMultiplier;
    public bool inAir;
    public float heightVelocity;
    public const float HeightVelocityDamper = 2;
    public float height;
    public virtual bool Hittable(PhysicsObject otherObject) => !otherObject.Invulnerable;
    public virtual void Hit(PhysicsObject otherObject, Vector2 velocity)
    {
        rb.AddForce(velocity,ForceMode2D.Impulse);
    }

    public virtual void SetHeight(float height) { }
    public virtual bool Invulnerable => false;
    public virtual bool Immovable => true;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (rb.velocity.sqrMagnitude < minimumHitThreshold)
        {
            return;
        }

        for (var i = 0; i < other.contactCount; i++)
        {
            HandleContact(other.GetContact(i));
        }
        if (other.gameObject.TryGetComponent<PhysicsObject>(out var otherObject) && Hittable(otherObject))
        {
            if (Hittable(otherObject))
            {
                otherObject.Hit(this,rb.velocity* HitVelocityMultiplier);
                HitFeedback(otherObject);
            }
        }
    }

    public virtual void HitFeedback(PhysicsObject victim)
    {
        
    }

    public virtual void HandleContact(ContactPoint2D contact)
    {
     
    }
    protected virtual void Bounce()
    {
        rb.velocity /= 2;
        if (heightVelocity < -0.1f)
        {
            heightVelocity = -heightVelocity / 2;
        }
        else
        {
            heightVelocity = 0;
            inAir = false;
        }
    }

    public void Fly(float velocity)
    {
        inAir = true;
        heightVelocity = velocity;
    }
    protected virtual void FixedUpdate()
    {
        if (inAir)
        {
            heightVelocity -= Time.fixedDeltaTime * HeightVelocityDamper;
            height += heightVelocity;
            if (height <= 0)
            {
                height = 0;
                Bounce();
            }
            SetHeight(height);
        }
    }
#if UNITY_EDITOR
    protected bool NoColliderAttached => collider == null;
    protected bool NoTransformAttached => transform == null;
    protected virtual void Reset()
    {
        if (NoColliderAttached) TryGetComponent(out collider);
        if (NoTransformAttached) transform = base.transform;
    }
#endif
}

public abstract class EntityObject : PhysicsObject
{
    public int maxHealth;
    [ShowInInspector]
    protected int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public override void Hit(PhysicsObject otherObject, Vector2 velocity)
    {
        
        base.Hit(otherObject,velocity);
        var damage = Mathf.FloorToInt(velocity.magnitude / 10) + 1;
        currentHealth-= damage;
        if (currentHealth <= 0)
        {
            Die(velocity);
        }
    }

    protected virtual void Die(Vector2 velocity)
    {
        gameObject.SetActive(false);
    }
}