using Sirenix.OdinInspector;
using UnityEngine;

public class AICar : Car
{
    public enum State
    {
        Idle,
        Alert
    }

    public State state;
// [ShowInInspector]
//     public float AngleToPlayer
//     {
//         get
//         {
//             var dir = (Vector2)(GameManager.instance.player.Transform.position - Transform.position).normalized;
//             return Vector2.SignedAngle(body.transform.up, dir);
//         }
//     }
    protected override void Update()
    {
        switch (state)
        {
            case State.Idle :
                motion = Vector2.zero;
                break;
            case State.Alert:
                var dir = (Vector2)(GameManager.instance.player.Transform.position - Transform.position).normalized;
                var rot = Vector2.SignedAngle(body.transform.up, dir);
                if (rot < -5)
                {
                    motion = new Vector2(-1, 1);
                }
                else if(rot > 5)
                {
                    motion = Vector2.one;
                }
                else
                {
                    motion = new Vector2(rot/5f,1);
                }
                break;
        }
        base.Update();
    }
}