using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour
{

    public GameObject Target;
    public NavMeshAgent Agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Agent.SetDestination(Target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
