using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnergyOrb : MonoBehaviour
{
    public float lifeTime = 5f;
    public float collapseSpeed = 5f;
    public Explosive explosive;
    private float startTime;
    private bool collapsing;
    private float startSize;
    private HPComponent hp;

    private void Start()
    {
        startTime = Time.time;
        hp = GetComponent<HPComponent>();
        hp.OnHPZero = Explode;
    }

    private void Explode()
    {
        Destroy(gameObject);
    }


    void Update()
    {
        if (Time.time - startTime > lifeTime)
        {
            float scale = transform.localScale.x;

            if (!collapsing)
            {
                collapsing = true;
                startTime = Time.time;
                startSize = scale;
            }


            // collapse
            if (scale > 0)
            {
                float progress = (Time.time - startTime) * collapseSpeed;

                scale = Mathf.Lerp(startSize, 0, progress / startSize);
                transform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                explosive.Disarm();
                Destroy(gameObject);
            }
        }
    }
}
