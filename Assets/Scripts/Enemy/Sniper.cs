using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Enemy
{
    internal override void Awake()
    {
        base.Awake();
    }

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
    }

    internal override void OnValidate()
    {
        base.OnValidate();
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    internal override void Attack()
    {
        base.Attack();
        Debug.Log("attacck");
        //StartCoroutine(nameof(Aim));
    }

    private void Aim()
    {

    }
}

/*//Invoke(nameof(ResetAttack), attackCooldown);

    Realistic shooting approach - FOR SNIPER


RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 100f, whatTakesDamage);
if (hit)
{
    // calculate the time it will take to reach 'hit'


    HPComponent hp;
    if (hit.transform.gameObject.TryGetComponent(out hp))
    {
        hp.Reduce(damage);
    }
}*/