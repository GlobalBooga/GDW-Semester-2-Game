using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] Object destructableObj;
    [SerializeField] bool hasPieces;
    [SerializeField] Explosive explosiveCharge;

    public HPComponent hp;

    void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = BreakObject;
    }

    private void BreakObject()
    {
        if (hasPieces == true)
        {
            GameObject destructable = (GameObject)Instantiate(destructableObj);
            destructable.transform.position = transform.position;
        }
        if (explosiveCharge) { explosiveCharge.explode(); }
        Destroy(gameObject);

    }
}
