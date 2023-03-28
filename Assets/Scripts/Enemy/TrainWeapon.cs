using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWeapon : MonoBehaviour
{
    HPComponent hp;
    public Animator anim;
    public int hitIndex;
    public GameObject onDeadObj;

    private void Start()
    {
        hp = GetComponent<HPComponent>();

        hp.OnHit.Add(Hit);
        hp.OnHPZero = Dead;

        Hide();
    }

    internal virtual void Hit()
    {

    }

    public void Dead()
    {
        anim.gameObject.GetComponent<HPComponent>().Reduce(1);
        gameObject.SetActive(false);
        if (hp.hpBars[0].isActiveAndEnabled) hp.hpBars[0].transform.parent.gameObject.SetActive(false);

        CameraShake.instance.ShakeCamera(10f, 0.5f);

        GameObject g = Instantiate(onDeadObj, transform.position, Quaternion.identity);
        g.transform.parent = null;
        Destroy(g, 1f);

        Invoke(nameof(Dead2), 0.3f);
    }

    private void Dead2()
    {
        CameraShake.instance.ShakeCamera(10f, 0.5f);

        GameObject g = Instantiate(onDeadObj, transform.position + Vector3.down * 0.4f, Quaternion.identity);
        g.transform.parent = null;
        Destroy(g, 1f);
    }


    public void Expose()
    {
        hp.hpBars[0].transform.parent.gameObject.SetActive(true);
        hp.isInvincible = false;
    }

    public void Hide()
    {
        hp.hpBars[0].transform.parent.gameObject.SetActive(false);
        hp.isInvincible = true;
    }
}
