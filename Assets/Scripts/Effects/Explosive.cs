using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    CircleCollider2D blastRadius;
    public float damage = 50;
    [SerializeField] float cameraShakeIntensity;
    [SerializeField] float cameraShakeTime;
    [SerializeField] GameObject explosionObj;
    [SerializeField] LayerMask whatTakesDamage;
    [SerializeField] float playerDamageMultiplier;
    [SerializeField] float specialObjectDamageMultiplier;
    public AudioSource audioSource;
    private bool cancelExplosion;

    void Start()
    {       
        blastRadius = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        blastRadius.enabled = false;
    }

    private void OnDestroy()
    {
        if (!LevelManager.instance.IsQuitting() && !cancelExplosion) Explode();
    }

    public void Explode()
    {
        CameraShake.instance.ShakeCamera(cameraShakeIntensity, cameraShakeTime);
        GameObject g = Instantiate(explosionObj, transform);
        g.transform.parent = null;
        Destroy(g, 1f);
        if (audioSource) audioSource.Play();
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, blastRadius.radius * transform.parent.localScale.x, whatTakesDamage);
        if (cols.Length > 0)
        {
            foreach (var obj in cols)
            {
                if (obj.gameObject.layer == StaticHelpers.SpecialBreakableObjectLayer)
                    StaticHelpers.ApplyDamage(obj.gameObject, damage * specialObjectDamageMultiplier);
                else if (obj.gameObject.layer == StaticHelpers.PlayerLayer)
                    StaticHelpers.ApplyDamage(obj.gameObject, damage * playerDamageMultiplier);
                else
                {
                    GameObject testObj = obj.gameObject;
                    while (!StaticHelpers.ApplyDamage(testObj, damage))
                    {
                        if (!testObj.gameObject.transform.parent) break;
                        testObj = testObj.transform.parent.gameObject;
                    }
                }
            }
        }
    }

    public void Disarm()
    {
        cancelExplosion = true;
    }

    public void SetWhatTakesDamage(LayerMask newWhatTakesDamage)
    {
        whatTakesDamage = newWhatTakesDamage;
    }
}

