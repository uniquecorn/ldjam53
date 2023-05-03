using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gib : PhysicsObject
{
    public SpriteRenderer sr;
    public bool bleed;
    public override void SetHeight(float height)
    {
        base.SetHeight(height);
        sr.transform.localPosition = Vector2.up * height;
        Collider.enabled = height < 1f;
    }
    protected override void Bounce()
    {
        base.Bounce();
        if (inAir && bleed)
        {
            ItemData.Instance.SpawnBlood(transform.position);
        }
    }
    public virtual void Spawn(Vector2 origin,Vector2 continuedVelocity)
    {
        Fly(0.2f);
        //rb.AddForceAtPosition(Random.insideUnitCircle*10f,origin,ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-1f,1f),ForceMode2D.Impulse);
        rb.AddForce(continuedVelocity/5f,ForceMode2D.Impulse);
    }

    public override void Hit(PhysicsObject otherObject, Vector2 velocity)
    {
        base.Hit(otherObject, velocity);
        Debug.Log(velocity.magnitude);
        Fly(velocity.magnitude/50);
    }
}