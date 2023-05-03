using UnityEngine;

public class Gun : Item
{
    public float fireRate;
    public float FireRate => fireRate * GameManager.instance.player.FireRate;
    public Bullet bulletPrefab;
    public float inaccuracy = 0.1f;
    public const float ProjectileSpawnDistance = 1f;

    public override void UpdateSlot(Inventory.SlotItem slot)
    {
        base.UpdateSlot(slot);
        if (slot.timer >= fireRate)
        {
            slot.timer -= fireRate;
            TryToFire();
        }
    }

    public void TryToFire()
    {
        var currentPosition = GameManager.instance.player.Transform.position;
        var colls = Physics2D.OverlapCircleAll(currentPosition, 12f);
        EntityObject closest = null;
        var closestDist = 144f;
        for (var i = 0; i < colls.Length; i++)
        {
            if(colls[i] == GameManager.instance.player.Collider)continue;
            if (!colls[i].TryGetComponent(out EntityObject entityObject))continue;
            var dist = (entityObject.Transform.position - currentPosition).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = entityObject;
            }
        }

        if (closest != null)
        {
            var inaccuracyMod = inaccuracy / GameManager.instance.player.Accuracy;
            var dir = ((closest.Transform.position + (Vector3)Random.insideUnitCircle * inaccuracyMod) - currentPosition).normalized;
            CreateBullet(currentPosition,dir);
        }
    }

    public virtual void CreateBullet(Vector2 currentPosition,Vector2 direction)
    {
        var bullet = Object.Instantiate(bulletPrefab, currentPosition+direction*ProjectileSpawnDistance, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, direction)));
        bullet.Fire(GameManager.instance.player.Collider,true);
    }
}

public class Shotgun : Gun
{
    public int shells;
    public float spread;
    public float shellRandomness;
    public override void CreateBullet(Vector2 currentPosition, Vector2 direction)
    {
        if (shells > 1)
        {
            var it = spread / (shells - 1);
            var startDir = Quaternion.Euler(0, 0, -spread/2) * direction;
            for (var i = 0; i < shells; i++)
            {
                base.CreateBullet(currentPosition, Quaternion.Euler(0,0, (it * i) + Random.Range(-shellRandomness,shellRandomness)) * startDir);
            }
        }
        else
        {
            base.CreateBullet(currentPosition, direction);
        }
    }
}