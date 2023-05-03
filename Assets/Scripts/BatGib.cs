using UnityEngine;

public class BatGib : Gib
{
    public Sprite[] spin;
    public Sprite normal;
    public float timer;
    public BatGuy owner;
    protected override void Bounce()
    {
        base.Bounce();
        owner = null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (inAir || rb.velocity.sqrMagnitude > 5f)
        {
            timer += Time.fixedDeltaTime*5f;
            sr.sprite = spin[Mathf.FloorToInt(timer % spin.Length)];
        }
        else
        {
            sr.sprite = normal;
        }
    }

    public override bool Hittable(PhysicsObject otherObject) => otherObject != owner && base.Hittable(otherObject);
}