using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MissileAttack", story: "Launch [x] missiles at [Target]", category: "Action", id: "e0616fbb59a52f4686efc051fa39288d")]
public partial class MissileAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> X;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<AndarozScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<Animator> TorsoAnimator; 
    [SerializeReference] public BlackboardVariable<bool> LockRotation; 
    [SerializeReference] public BlackboardVariable<Transform> Body;
    [SerializeReference] public BlackboardVariable<List<GameObject>> MissileSpawns;

    private float shootStartDelay;
    private float shootDelay;
    private int shots;
    private int prev;
    
    protected override Status OnStart()
    {
        // play lock on animation
        if (Params.Value.crosshairController)
        {
            Crosshair crosshair = GameObject.Instantiate(Params.Value.crosshairController, Body.Value).GetComponent<Crosshair>();
            if (crosshair)
            {
                crosshair.AimAt(Target);
                Params.Value.missile_shootStartDelay = crosshair.animationTime;
            }
        }

        TorsoAnimator.Value.Play(Andaroz.MISSILE_ATTACK);
        
        //freeze rotation
        LockRotation.Value = true;
        prev = 0;
        shootStartDelay = Params.Value.missile_shootStartDelay;
        shootDelay = 0;
        shots = 0;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if ((shootStartDelay -= Time.deltaTime) > 0 || (shootDelay -= Time.deltaTime) > 0) return Status.Running;

        if (shots++ > X.Value) return Status.Success;

        // calculate spread
        float spreadAngle = UnityEngine.Random.Range(-Params.Value.missileSpread, Params.Value.missileSpread);
        float rads = Mathf.Deg2Rad * ((spreadAngle > 0) ? spreadAngle : (360f + spreadAngle));
        float x = Body.Value.up.x, y = Body.Value.up.y;

        Vector3 missileDir = new Vector2((Mathf.Cos(rads) * x) - (Mathf.Sin(rads) * y), (Mathf.Sin(rads) * x) + (Mathf.Cos(rads) * y));

        // shoot
        HomingMissile b = GameObject.Instantiate(Params.Value.missile, MissileSpawns.Value[prev].transform).GetComponent<HomingMissile>();

        //play shoot sound
        MissileSpawns.Value[prev].GetComponent<AudioSource>().Play();

        prev = (prev + 1) % MissileSpawns.Value.Count;

        b.transform.Rotate(0f, 0f, Vector2.SignedAngle(Body.Value.up, missileDir));
        b.gameObject.layer = StaticHelpers.EnemyMissileLayer;
        b.Fly(Target.Value, missileDir, Params.Value.missileRotForce, Params.Value.missileMaxSpeed, Params.Value.missile_damage);
        b.specialObjectDamageMultiplier = Params.Value.missile_pillarDamageMultiplier;
        b.explosive.SetWhatTakesDamage(Params.Value.whatTakesDamageFromMissiles);
        shootDelay = Params.Value.missile_delayBetweenShots;
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        LockRotation.Value = false;
    }
}

