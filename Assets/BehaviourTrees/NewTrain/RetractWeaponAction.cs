using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RetractWeapon", story: "Retract [Weapons]", category: "Action", id: "3e199400dbf4f15266c70788c3144b91")]
public partial class RetractWeaponAction : Action
{
    [SerializeReference] public BlackboardVariable<TrainBossScriptableObject> Params;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Weapons;
    [SerializeReference] public BlackboardVariable<GameObject> Body;
    [SerializeReference] public BlackboardVariable<Animator> Animator;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // retract
        bool retract =false;

        foreach (var item in Weapons.Value)
        {
            Vector3 targetDir = -Body.Value.transform.up;
            float angle = Vector3.SignedAngle(targetDir, -item.transform.right, Vector3.back);
            if (Mathf.Abs(angle) > 0.01f)
            {
                item.transform.Rotate(0, 0, angle * Time.deltaTime * Params.Value.rotationSpeed);
            }
            else
            {
                item.transform.rotation = Quaternion.Euler(0, 0, 90f);
            }

            if (Mathf.Abs(angle) < 1f && !retract)
            {
                retract = true;
                Animator.Value.Play(TrainBoss.TURRETS_RETRACT, TrainBoss.LAYER_TURRETS);

                // hide hpbars
                foreach (var t in Weapons.Value)
                {
                    t.GetComponent<TrainWeapon>().Hide();
                }
            }
            
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

