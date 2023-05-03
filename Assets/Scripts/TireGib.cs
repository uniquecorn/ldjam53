using UnityEngine;

public class TireGib : ItemGib
{
    public Sprite[] spin;
    public Sprite normal;
    public float timer;
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
}