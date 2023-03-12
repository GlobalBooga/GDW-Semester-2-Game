using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryStrike : MonoBehaviour
{
    public float impactDelay;
    public Explosive explosive;

    void Start()
    {
        Invoke(nameof(Strike), impactDelay);
    }

    private void Strike()
    {
        Destroy(gameObject);
    }
}
