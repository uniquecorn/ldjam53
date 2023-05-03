using UnityEngine;

public class ChainsawGuy : Person
{
    public Sprite windUp, attack;
    public AttackState attackState;
    public enum AttackState
    {
        None,
        Attack,
        FollowThrough
    }
    public float attackRange;
    public override bool InterruptAI()
    {
        if (attackState != AttackState.None)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer < 0.5f)
            {
                body.sprite = windUp;
            }
            else if (alertTimer < 1f)
            {
                body.sprite = attack;
                if (attackState != AttackState.FollowThrough)
                {
                    FollowThrough();
                }
            }
            else
            {
                attackState = AttackState.None;
            }
            return true;
        }
        return base.InterruptAI();
    }
    public void FollowThrough()
    {
        var colls = Physics2D.OverlapCircleAll(Transform.position, 4.25f);
        foreach (var coll in colls)
        {
            if(coll == Collider) continue;
            if (coll.TryGetComponent(out EntityObject entityObject))
            {
                var dir = (coll.transform.position - transform.position).normalized;
                entityObject.Hit(this,dir * 5f);
            }
        }

        attackState = AttackState.FollowThrough;
    }
    public override void RunAlert()
    {
        base.RunAlert();
        var dist = (targetPosition - Transform.position).sqrMagnitude;
        if (dist < (attackRange * attackRange))
        {
            attackState = AttackState.Attack;
            alertTimer = 0;
        }
    }
}