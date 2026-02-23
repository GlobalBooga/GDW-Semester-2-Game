using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Destination", story: "Make [agent] travel along [Waypoints]", category: "Action/Navigation", id: "c56b291e4760fe1c361e1e65ec5a2b99")]
public partial class SetDestinationAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Body;
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    private Queue<GameObject> waypoints = new Queue<GameObject>();
    
    
    protected override Status OnStart()
    {
        foreach (var t in Waypoints.Value)
        {
            waypoints.Enqueue(t);
        }
        

        if (waypoints.Count == 0) return Status.Failure;

        
        Agent.Value.speed = 2;
            
            
        if (Agent.Value.remainingDistance <= Agent.Value.stoppingDistance)
        {
            waypoints.Enqueue(waypoints.Dequeue());
        }

        
        Agent.Value.SetDestination(waypoints.Peek().transform.position);
        
        Quaternion newQuat = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(Agent.Value.destination - Body.Value.position, Vector3.up, Vector3.back));
        Body.Value.rotation = Quaternion.Lerp(Body.Value.rotation, newQuat, 0.05f);
        
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

