using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCar : Car
{
    public int frameStopIntensity = 1;
    public float projectileSpawnDistance;
    public float inaccuracy = 0.1f;

    public override float HitVelocityMultiplier
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IHitMultiplier item)
                {
                    value += item.Value;
                }
            }
            return value * hitVelocityMultiplier;
        }
    }

    public float Accuracy
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IAccuracy item)
                {
                    value += item.Value;
                }
            }
            return value;
        }
    }
    public override float Speed
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is ISpeedItem item)
                {
                    value += item.Value;
                }
            }
            return value*speed;
        }
    }

    public override float Acceleration
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IAcceleration item)
                {
                    value += item.Value;
                }
            }
            return value*acceleration;
        }
    }

    public override float Handling
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IHandling item)
                {
                    value += item.Value;
                }
            }
            return value;
        }
    }

    public float FireRate
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IFireRate item)
                {
                    value += item.Value;
                }
            }
            return value;
        }
    }
    public float ProjectileSpeed
    {
        get
        {
            var value = 1f;
            foreach (var slot in GameManager.instance.inventory.items)
            {
                if (slot.GetItem is IProjectileSpeed item)
                {
                    value += item.Value;
                }
            }
            return value;
        }
    }
    void HandleInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            motion = new Vector2(0, 1f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            motion = new Vector2(0, -1f);
        }
        else
        {
            motion = Vector2.zero;
        }

        if (Input.GetKey(KeyCode.A))
        {
            motion = new Vector2(1, motion.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            motion = new Vector2(-1, motion.y);
        }
        else
        {
            motion = new Vector2(0, motion.y);
        }
    }
    protected override void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            HandleInput();
        }
        base.Update();
        foreach (var slot in GameManager.instance.inventory.items)
        {
            slot.timer += Time.deltaTime;
            slot.GetItem.UpdateSlot(slot);
        }
    }
    
    public override void HitFeedback(PhysicsObject victim)
    {
        base.HitFeedback(victim);
        if (victim is Person)
        {
            Framestop(frameStopIntensity).Forget();
        }
    }

    public async UniTaskVoid Framestop(int framesToPause = 1)
    {
        var prevTimeScale = Time.timeScale;
        Time.timeScale = 0.001f;
        CameraManager.CameraShake = 5f;
        for (var i = 0; i < framesToPause; i++)
        {
            await UniTask.Yield();
        }
        Time.timeScale = prevTimeScale;
    }
}