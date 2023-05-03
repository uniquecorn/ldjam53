using UnityEngine;

public class BatGuy : Person
{
    public Sprite[] forwardE, backE;
    public Sprite windUp,attackSuccess,attackMiss,attackThrow;
    public bool emptyHanded;
    public AttackState attackState;
    private int throwAttempt;
    public BatGib batGib;
    public enum AttackState
    {
        None,
        Attack,
        Throwing,
        FollowThrough
    }
    public float attackRange;
    public override void SetMoveSprite(bool backwards, int index)
    {
        if (emptyHanded)
        {
            body.sprite = backwards ? backE[index] : forwardE[index];
        }
        else
        {
            base.SetMoveSprite(backwards, index);
        }
    }

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
        var dir = (targetPosition - transform.position);
        var dist = dir.sqrMagnitude;
        if (attackState == AttackState.Attack)
        {
            if (dist < attackRange * attackRange)
            {
                body.sprite = attackSuccess;
                GameManager.instance.player.Hit(this,dir*5f);
            }
            else
            {
                body.sprite = attackMiss;
            }
        }
        else if(attackState == AttackState.Throwing)
        {
            emptyHanded = true;
            var g = Instantiate(batGib, transform.position,
                Quaternion.Euler(0, 0, Random.Range(-5f, 5f)));
            g.owner = this;
            g.Spawn(Transform.position,dir*20);
            g.Fly(0.4f);
            body.sprite = attackThrow;
        }
        attackState = AttackState.FollowThrough;
    }
    public override void RunAlert()
    {
        base.RunAlert();
        if (!emptyHanded)
        {
            var dist = (targetPosition - Transform.position).sqrMagnitude;
            if (dist < (attackRange * attackRange))
            {
                attackState = AttackState.Attack;
                alertTimer = 0;
            }
            else
            {

                var minDist = (attackRange * 3) * (attackRange * 3);
                var maxDist = (attackRange * 5) * (attackRange * 5);
                if (dist < maxDist && dist > minDist)
                {
                    if (Mathf.CeilToInt(alertTimer) != throwAttempt)
                    {
                        throwAttempt = Mathf.CeilToInt(alertTimer);
                        if (Random.value < 0.1f)
                        {
                            attackState = AttackState.Throwing;
                            alertTimer = 0;
                        }
                    }

                }
            }
        }
    }
}