using UnityEngine;

public class Shell : Bullet
{
    public float decay;
    public override void Move(float distance)
    {
        base.Move(distance);
        speed -= Time.deltaTime * decay;
        if (speed < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}