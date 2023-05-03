using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle;
using Sirenix.OdinInspector;
using UnityEngine;

public class Car : EntityObject
{
    public Transform bodyTransform;
    public SpriteRenderer body,wheels;
    [System.Serializable]
    public class CarView
    {
        public Sprite body;
        public Sprite[] bodyHurt;
        public Sprite wheelsIdle;
        public Sprite[] wheelsRoll;
        public bool flipSprites;
    }
    public CarView[] views;
    public float speed;
    public ParticleSystem[] smoke;
    public float maxTurningDegree;
    public float currentAngle;
    [ShowInInspector]
    private float angularAcceleration;
    public Vector2 motion;
    public float angularAccelerationMultiplier;
    public float acceleration = 3f;
    public float bumpMultiplier = 2;
    public virtual float Handling => 1f;
    public float AngularMultiplier => angularAccelerationMultiplier * Handling;
    public float MaxTurn => maxTurningDegree + (10 * Handling);
    public virtual float Speed => speed;
    public virtual float Acceleration => acceleration;
    public float distanceTracker;
    // Update is called once per frame
    protected virtual void Update()
    {
        if (Mathf.Abs(motion.x) > 0.01f)
        {
            angularAcceleration += Time.deltaTime*AngularMultiplier * motion.x;
            if (Mathf.Abs(angularAcceleration) >= MaxTurn)
            {
                if (angularAcceleration < 0)
                {
                    angularAcceleration = -MaxTurn;
                    if (!smoke[0].isPlaying)
                    {
                        smoke[0].Play();
                    }
                    //smoke[0].Emit(Mathf.FloorToInt(rb.velocity.magnitude));
                }
                else
                {
                    angularAcceleration = MaxTurn;
                    if (!smoke[1].isPlaying)
                    {
                        smoke[1].Play();
                    }
                }
            }
            else
            {
                smoke[0].Stop();
                smoke[1].Stop();
            }
        }
        else
        {
            angularAcceleration = Mathf.Lerp(angularAcceleration, 0, Time.deltaTime*5f);
            smoke[0].Stop();
            smoke[1].Stop();
        }

        var currentSpeed = rb.velocity.magnitude;
        distanceTracker += currentSpeed / 10f;
        if (currentSpeed > 0.1f)
        {
            var turningFactor = 1 / currentSpeed;
            currentAngle += angularAcceleration *Time.deltaTime * Mathf.Clamp(turningFactor,2,10)/2;
        }
        if (currentAngle > 180)
        {
            currentAngle = -180 + (currentAngle - 180);
        }
        else if (currentAngle < -180)
        {
            currentAngle = (360 + currentAngle);
        }
        //currentAngle = (currentAngle % 360) - 180;
        var dir = Quaternion.Euler(0, 0, currentAngle) * Vector2.up * motion.y;
        if (Mathf.Abs(motion.y) > 0.1f)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, dir * Speed,Time.deltaTime * Acceleration);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, dir * Speed,Time.deltaTime * 2f);
        }

        
        // if (rb.velocity.sqrMagnitude > 5)
        // {
        //     foreach (var s in smoke)
        //     {
        //         s.Emit(Mathf.FloorToInt(rb.velocity.magnitude));
        //     }
        // }
        
        // angle = Vector2.SignedAngle(Vector2.up, force);
        // var targetAngle = Mathf.Lerp(currentAngle, angle, Time.deltaTime);
        // currentAngle = targetAngle;
        bodyTransform.localRotation = Quaternion.Euler(0,0,currentAngle);
        
        var spriteIndex = 0;
        
        if (currentAngle < 22.5f && currentAngle > -22.5f)
        {
            spriteIndex = 0;
        }
        else if (currentAngle < 67.5f && currentAngle > -67.5f)
        {
            spriteIndex = 1;
        }
        else if (currentAngle < 112.5f && currentAngle > -112.5f)
        {
            spriteIndex = 2;
        }
        else if (currentAngle < 157.5f && currentAngle > -157.5f)
        {
            spriteIndex = 3;
        }
        else
        {
            spriteIndex = 4;
        }
        SetCarView(views[spriteIndex],currentSpeed >= 1f);
    }

    public void SetCarView(CarView view, bool rolling)
    {
        var flip = view.flipSprites && currentAngle > 0;
        if (view.bodyHurt.IsSafe())
        {
            var damageState = Mathf.CeilToInt((float) currentHealth / ((float) maxHealth / (view.bodyHurt.Length + 1)));
            damageState = Mathf.Clamp(damageState, 1, view.bodyHurt.Length + 1)-1;
            if (damageState >= view.bodyHurt.Length)
            {
                body.sprite = view.body;
            }
            else
            {
                body.sprite = view.bodyHurt[damageState];
            }
        }
        else
        {
            body.sprite = view.body;
        }
        wheels.sprite = rolling ? view.wheelsRoll[Mathf.FloorToInt(distanceTracker % 2)] : view.wheelsIdle;
        body.flipX = wheels.flipX = flip;
    }
    public override bool Invulnerable => true;
    public override void HandleContact(ContactPoint2D contact)
    {
        base.HandleContact(contact);
        if (contact.otherRigidbody.isKinematic)
        {
            rb.AddForce(contact.normal * contact.normalImpulse*bumpMultiplier,ForceMode2D.Impulse);
        }
        else if (contact.otherCollider.TryGetComponent(out PhysicsObject otherObject))
        {
            if (Mathf.Abs(otherObject.rb.mass - rb.mass) < 1f)
            {
                rb.AddForce(contact.normal * contact.normalImpulse*bumpMultiplier,ForceMode2D.Impulse);
            }
        }
    }
}