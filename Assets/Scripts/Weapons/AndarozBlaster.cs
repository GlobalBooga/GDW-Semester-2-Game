using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AndarozBlaster : Gun
{
    [Header("Weapon Ability"), Space(5f)]
    public float newCooldown = 0.01f;
    public float abilityShots;
    public float delayBeforeAbility = 1f;
    private float temp;
    public ParticleSystem chargeParticles;

    [Header("Lighting"), Space(5f)]
    public Sprite holdingEmissionMapPinks;
    public Sprite droppedEmissionMapPinks;
    public Sprite holdingEmissionMapGreens;
    public Sprite droppedEmissionMapGreens;

    public override int GetID()
    {
        return 4;
    }
    public override void UseAbility()
    {
        if (!canUseAbility) return;
        canUseAbility = false;
        readyToUse = false;
        usingAbility = true;

        base.UseAbility();

        temp = cooldown;   
        cooldown = newCooldown;

        if (chargeParticles) StartCoroutine(nameof(ChargeAndFireAbility));
    }

    private IEnumerator ChargeAndFireAbility()
    {
        if (chargeParticles)
        {
            chargeParticles.Play();
            while (chargeParticles.isPlaying) yield return null;
        }

        if (delayBeforeAbility > 0) yield return new WaitForSeconds(delayBeforeAbility);
        
        for (int i = 0; i < abilityShots; i++)
        {
            if (i == abilityShots - 1) usingAbility = false;
            Fire();
            yield return new WaitForSeconds(newCooldown);
        }

        StartCoroutine(nameof(CooldownAbility));
    }

    public override void EndAbility()
    {
        base.EndAbility();

        cooldown = temp;
        readyToUse = true;
    }

    public override void Pickup(Transform parentTo, Weapon weapon)
    {
        base.Pickup(parentTo, weapon);

        transform.GetChild(0).GetChild(0).GetComponent<Light2D>().lightCookieSprite = holdingEmissionMapPinks;
        transform.GetChild(0).GetChild(1).GetComponent<Light2D>().lightCookieSprite = holdingEmissionMapGreens;

        Vector2 newpos = new Vector3(-0.139f, 0.047f, 0f);
        transform.GetChild(0).GetChild(0).localPosition = newpos;
        transform.GetChild(0).GetChild(1).localPosition = newpos;
        transform.GetChild(0).GetChild(0).localScale = Vector3.one * 0.5f;
        transform.GetChild(0).GetChild(1).localScale = Vector3.one * 0.5f;
    }

    public override void Drop(Vector2 forwards)
    {
        base.Drop(forwards);

        transform.GetChild(0).GetChild(0).GetComponent<Light2D>().lightCookieSprite = droppedEmissionMapPinks;
        transform.GetChild(0).GetChild(1).GetComponent<Light2D>().lightCookieSprite = droppedEmissionMapGreens;

        transform.GetChild(0).GetChild(0).localPosition = Vector3.zero;
        transform.GetChild(0).GetChild(1).localPosition = Vector3.zero;
        transform.GetChild(0).GetChild(0).localScale = Vector3.one * 0.64f;
        transform.GetChild(0).GetChild(1).localScale = Vector3.one * 0.64f;
    }
}
