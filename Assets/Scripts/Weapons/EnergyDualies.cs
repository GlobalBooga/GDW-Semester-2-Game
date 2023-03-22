using System.Collections;
using UnityEngine;

public class EnergyDualies : DualPistols
{
    [Header("Weapon Ability"), Space(5f)]
    public float chargeSpeed = 3f;
    public float maxOrbSize = 3f;
    public float orbSpeed = 5f;
    public GameObject orbPrefab;
    public Transform orbSpawn;
    public AnimationCurve orbgrowcurve;
    //public ParticleSystem orbCharge;

    private GameObject orb;

    public override int GetID()
    {
        return 2;
    }

    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;
        usingAbility = true;

        base.UseAbility();

        StartCoroutine(nameof(FireAbility));
    }

    public override void Use()
    {
        if (!usingAbility)
        {
            base.Use();
        }
    }

    private IEnumerator FireAbility()
    {
        orb = Instantiate(orbPrefab);
        
        float startTime = Time.time;
        float scale = 0;

        while (scale < maxOrbSize)
        {
            if (!orb) break;

            float progress = (Time.time - startTime) * chargeSpeed;
            scale = Mathf.Lerp(0, maxOrbSize, orbgrowcurve.Evaluate(progress / maxOrbSize));
            orb.transform.localScale = new Vector3(scale, scale, 1f);
            orb.transform.position = orbSpawn.position;

            yield return null;
        }

        if (orb)
        {
            Rigidbody2D rb = orb.GetComponent<Rigidbody2D>();
            rb.AddForce(transform.up * orbSpeed * rb.mass, ForceMode2D.Impulse);
            orb.transform.parent = null;
        }

        StartCoroutine(nameof(CooldownAbility));
    }

    public override void EndAbility()
    {
        base.EndAbility();
        readyToUse = true;
        usingAbility = false;
    }


    public override void UpdateGunProperties(DualPistols_Single gun)
    {
        base.UpdateGunProperties(gun);
    }
}
