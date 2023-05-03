using System;
using System.Collections;
using System.Collections.Generic;
using Castle;
using UnityEngine;
using Random = UnityEngine.Random;

public class Person : EntityObject
{
    public AnimationCurve stepCurve;
    public Sprite idle;
    public Sprite hitL,hitT,hitB,angry;
    public Sprite[] forward, back;

    public SpriteRenderer body;
    public SpriteRenderer shadow;
    public State state;
    public Vector3 targetPosition;
    public float moveTimer,wanderTimer;
    public float speed;
    public float wanderDistance;
    public Gib[] gibs;
    public float angerTimer;
    public float hitTimer;
    public float alertTimer;
    public float alertRadius;
    public bool knockedUp;
    public enum State
    {
        Idle,
        Alert
    }
    public void Update()
    {
        if (knockedUp)
        {
            hitTimer += Time.deltaTime;
            if (rb.velocity.sqrMagnitude < 5f && !inAir)
            {
                rb.velocity = Vector2.zero;
                knockedUp = false;
            }
            else
            {
                if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
                {
                    body.sprite = hitL;
                    body.flipX = rb.velocity.x > 0;
                }
                else if (rb.velocity.y > 0)
                {
                    body.sprite = hitT;
                }
                else
                {
                    body.sprite = hitB;
                }
            }
        }
        else if (angerTimer > 0)
        {
            angerTimer -= Time.deltaTime;
            body.sprite = angry;
            return;
        }
        else if (!InterruptAI())
        {
            switch (state)
            {
                case State.Idle:
                    RunIdle();
                    break;
                case State.Alert:
                    RunAlert();
                    break;
            }
        }
    }

    public virtual bool InterruptAI() => false;

    public virtual void RunIdle()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer > 3)
        {
            wanderTimer -= Random.Range(3f,5f);
            Wander();
        }
        Vector2 dist = (GameManager.instance.player.Transform.position - Transform.position);
        if (dist.sqrMagnitude < alertRadius* alertRadius)
        {
            state = State.Alert;
            targetPosition = GameManager.instance.player.Transform.position;
        }
    }

    public virtual void RunAlert()
    {
        alertTimer += Time.deltaTime;
        targetPosition = GameManager.instance.player.Transform.position;
    }
    public override void SetHeight(float height)
    {
        base.SetHeight(height);
        body.transform.localPosition = Vector2.up * height;
        Collider.enabled = height < 1f;
    }

    protected override void Bounce()
    {
        base.Bounce();
        if (inAir)
        {
            ItemData.Instance.SpawnBlood(transform.position);
        }
    }

    protected virtual bool LockMovement()
    {
        return knockedUp || angerTimer > 0;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (LockMovement()) return;
        if ((targetPosition - transform.position).sqrMagnitude > 3f)
        {
            Move();
        }
        else
        {
            moveTimer = 0;
            body.transform.localPosition = Vector2.up * height;
            if (state == State.Idle)
            {
                body.sprite = idle;
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Transform.position,alertRadius);
    }

    public void SetAngry()
    {
        if (LockMovement()) return;
        angerTimer = 1f;
        body.sprite = angry;
        targetPosition = transform.position;
    }
    public override void Hit(PhysicsObject otherObject, Vector2 velocity)
    {
        knockedUp = true;
        height = 0;
        hitTimer = 0;
        base.Hit(otherObject,velocity);
        if (currentHealth > 0)
        {
            ItemData.Instance.SpawnBigBlood(transform.position);
        }
        Fly(velocity.magnitude/50);
        
    }
    public void Move()
    {
        var dist = (targetPosition - transform.position);
        moveTimer += Time.fixedDeltaTime*2f;
        var currentStep = Mathf.PingPong((moveTimer % 1)*2f,1);
        height = currentStep * 0.25f;
        SetHeight(height);
        var dir = dist.normalized;
        body.flipX = dir.x < 0;
        SetMoveSprite(dir.y > 0, Mathf.FloorToInt(moveTimer % 2));
        var pos = dir * Mathf.Min(Time.fixedDeltaTime * speed * currentStep, dist.magnitude);
        rb.MovePosition(rb.position + (Vector2)pos);
    }
    public virtual void SetMoveSprite(bool backwards, int index) => body.sprite = backwards ? back[index] : forward[index];
    public void Wander() => targetPosition = transform.position + (Vector3)Random.insideUnitCircle*wanderDistance;
    public override bool Invulnerable => knockedUp && hitTimer < 0.5f;
    protected override void Die(Vector2 velocity)
    {
        gibs.Shuffle();
        foreach (var gib in gibs)
        {
            var g = Instantiate(gib, transform.position + (Vector3)Random.insideUnitCircle,
                Quaternion.Euler(0, 0, Random.Range(-5f, 5f)));
            g.Spawn(transform.position,velocity);
            if (Random.value > 0.1f) break;
        }

        Instantiate(ItemData.Instance.bloodExplosion, transform.position,Quaternion.identity);
        var colls = Physics2D.OverlapCircleAll(transform.position, 8f);
        foreach (var coll in colls)
        {
            if (coll.TryGetComponent<Person>(out var person))
            {
                if (person == this) continue;
                person.SetAngry();
            }
        }
        base.Die(velocity);
    }
    public override bool Hittable(PhysicsObject otherObject) => knockedUp && otherObject is not Car && !otherObject.Invulnerable;
}