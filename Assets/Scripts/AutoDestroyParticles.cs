using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticles : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
