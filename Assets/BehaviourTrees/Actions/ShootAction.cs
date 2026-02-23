using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "Shoot", category: "Action", id: "b331401305c9a292e8af0f9645ab5010")]
public partial class ShootAction : Action
{

    [SerializeReference] public BlackboardVariable<Transform> Body;
    
    [SerializeReference] public BlackboardVariable<RangedScriptableObject> Eso;
    
    [SerializeReference] public BlackboardVariable<Transform> BulletSpawn;
    [SerializeReference] public BlackboardVariable<GameObject> MuzzleFlash;
    [SerializeReference] public BlackboardVariable<AudioSource> AudioSource;
    
    protected override Status OnStart()
    {
        // calculate spread
        float spreadAngle = Random.Range(-Eso.Value.bulletSpread, Eso.Value.bulletSpread);
        float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
        float x = Body.Value.up.x, y = Body.Value.up.y;

        Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

        // muzzleFlash
        if (MuzzleFlash.Value) MuzzleFlash.Value.SetActive(true);
        if (AudioSource.Value) AudioSource.Value.Play();


        // spawn bullet
        if (Eso.Value.bullet && BulletSpawn.Value)
        {
            Bullet b = GameObject.Instantiate(Eso.Value.bullet, BulletSpawn.Value).GetComponent<Bullet>();
            b.transform.Rotate(0f,0f, Vector2.SignedAngle(Body.Value.up, bulletDir));
            b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
            b.Fly(bulletDir, Eso.Value.bulletSpeed, Eso.Value.damage);
        }
        
        LevelManager.instance.AlertAllEnemiesInCurrentScene(Body.Value.position);
        
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

