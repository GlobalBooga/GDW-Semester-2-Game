using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableBody : MonoBehaviour
{
    [SerializeField] Vector2 forceDirection;
    [SerializeField] float torque;
    Rigidbody2D rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(forceDirection);
        rb.AddTorque(torque);

    }
 
}
