using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector,SerializeField]
    private new Transform transform;
    public Transform Transform => transform ? transform : transform = base.transform;
    public float initialSpeed;
    protected float speed;

    public Collider2D ignored;
    public bool firing;
    public void Fire(Collider2D ignored,bool playerProjectile)
    {
        this.ignored = ignored;
        if (playerProjectile)
        {
            this.speed = initialSpeed * GameManager.instance.player.ProjectileSpeed;
        }
        else
        {
            this.speed = initialSpeed;
        }
        firing = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (firing)
        {
            var distance = speed * Time.deltaTime;
            Move(distance);
        }
    }

    public virtual void Move(float distance)
    {
        var hits = Physics2D.RaycastAll(Transform.position, Transform.up, distance);
        for (var i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider == ignored) continue;
            Transform.position += Transform.up * hits[i].distance;
            if (hits[i].collider.TryGetComponent(out EntityObject entity))
            {
                entity.Hit(null, Transform.up * distance/10);
                entity.Fly(0.25f);
            }
            
            firing = false;
            Destroy(gameObject);
            return;
        }
        Transform.position += Transform.up * distance;
    }
}