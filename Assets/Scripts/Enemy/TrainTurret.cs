using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTurret : TrainWeapon
{
    public Transform bulletSpawn;
    public GameObject muzzleFlash;
    
    internal override void Hit()
    {
        anim.Play($"TurretHit{hitIndex}", 5+hitIndex);
    }
    
    public void Fire(GameObject bullet, float damage, float spread, float speed, float coverDmgMult = 0)
    {
        if (!enabled) return;

        // calculate spread
        float spreadAngle = UnityEngine.Random.Range(-spread, spread);
        float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
        float x = -transform.right.x, y = -transform.right.y;

        Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

        if (bullet && bulletSpawn)
        {
            Bullet b = Instantiate(bullet, bulletSpawn).GetComponent<Bullet>();
            b.transform.Rotate(0f, 0f, Vector2.SignedAngle(-transform.right, bulletDir));
            b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
            b.Fly(bulletDir, speed, damage);
            b.specialObjectDamageMultiplier = coverDmgMult;

            //play muzzle effect
            if (muzzleFlash) muzzleFlash.SetActive(true);
        }
    }

}
