using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public bool alertsEnemies = true;
    public float bulletSpread;
    public float bulletSpeed;
    public float cameraShakeIntensity;
    public float cameraShakeTime;
    public GameObject muzzleFlash;
    public GameObject bullet;
    public Transform bulletSpawn;

    // for alerting enemies
    private float alertInterval = 1f;
    private float time;

    internal override void Awake()
    {
        base.Awake();

        time = Time.time - alertInterval;
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void ResetUse()
    {
        base.ResetUse();
    }

    public override void Use()
    {
        if (!readyToUse) return;
        readyToUse = false;

        Fire();
    }

    internal virtual void Fire()
    {
        if (muzzleFlash)
        {
            muzzleFlash.SetActive(true);
        }

        CameraShake.instance.ShakeCamera(cameraShakeIntensity, cameraShakeTime);

        // Alert enemies
        if (Time.time - time >= alertInterval)
        {
            time = Time.time;
            LevelManager.instance.AlertAllEnemiesInCurrentScene(transform.position);
        }


        // calculate spread
        float spreadAngle = Random.Range(-bulletSpread, bulletSpread);
        float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
        float x = transform.up.x, y = transform.up.y;

        Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

        if (bullet && bulletSpawn)
        {
            Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
            b.transform.Rotate(0f, 0f, Vector2.SignedAngle(transform.up, bulletDir));
            b.gameObject.layer = StaticHelpers.PlayerProjectileLayer;
            b.Fly(bulletDir, bulletSpeed, damage);
            b.specialObjectDamageMultiplier = 0f;
        }

        if (cooldown > 0) Invoke(nameof(ResetUse), cooldown);
        else ResetUse();
    }    


    public override void Drop(Vector2 forwards)
    {
        base.Drop(forwards);
    }

    public override void Pickup(Transform parentTo, Weapon weapon)
    {
        base.Pickup(parentTo, weapon);
    }
}
