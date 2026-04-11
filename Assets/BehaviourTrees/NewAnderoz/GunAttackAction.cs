using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GunAttack", story: "Shoot [x] times", category: "Action", id: "89b4ab83e14531dfbd4ef95bfde18cc9")]
public partial class GunAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> X;
    
    [SerializeReference] public BlackboardVariable<AndarozScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Transform> BulletSpawn;
    [SerializeReference] public BlackboardVariable<Transform> Body;
    [SerializeReference] public BlackboardVariable<GameObject> MuzzleFlash;
    [SerializeReference] public BlackboardVariable<AudioSource> AudioSource;
    [SerializeReference] public BlackboardVariable<Animator> TorsoAnimator; 
    private float shootStartDelay;
    private float shootDelay;
    private int shots;
    
    protected override Status OnStart()
    {
        // GUN ATTACK
        // in this attack, Andaroz chases the player at walking speed.
        // he is always facing the player
        
        
        // animation
        TorsoAnimator.Value.Play(Andaroz.GUN_ATTACK);

        shootStartDelay = Params.Value.gun_shootStartDelay;
        shootDelay = 0;
        shots = 0;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((shootStartDelay -= Time.deltaTime) > 0 || (shootDelay -= Time.deltaTime) > 0) return Status.Running;

        if (shots++ > X.Value) return Status.Success;
        
        // calculate spread
        float spreadAngle = UnityEngine.Random.Range(-Params.Value.bulletSpread, Params.Value.bulletSpread);
        float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
        float x = Body.Value.up.x, y = Body.Value.up.y;

        Vector3 bulletDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

        Bullet b = GameObject.Instantiate(Params.Value.bullet, BulletSpawn.Value).GetComponent<Bullet>();
        b.transform.Rotate(0f, 0f, Vector2.SignedAngle(Body.Value.up, bulletDir));
        b.gameObject.layer = StaticHelpers.EnemyProjectileLayer;
        b.Fly(bulletDir, Params.Value.bulletSpeed, Params.Value.gun_damage);
        b.specialObjectDamageMultiplier = Params.Value.gun_pillarDamageMultiplier;

        //play muzzle effect
        MuzzleFlash.Value.SetActive(true);
        AudioSource.Value.Play();

        shootDelay = Params.Value.gun_delayBetweenShots;
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        
    }
}

