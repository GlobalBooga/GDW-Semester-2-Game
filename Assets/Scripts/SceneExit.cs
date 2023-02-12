using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneExit : MonoBehaviour
{
    public Animator screenOverlayAnimator;
    public float transitionTime = 0.2f;
    BoxCollider2D bc;
    private const string TRANSITION_ANIM = "SceneTransition";
    private bool willFailToDetectTriggerEnter = false;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false; 
        bc = sr.GetComponent<BoxCollider2D>();

        Block();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            //Debug.Log("trigger enter");
            StartSceneTransition();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer) && 
            willFailToDetectTriggerEnter)
        {
            willFailToDetectTriggerEnter=false;
            //Debug.Log("trigger enter");
            StartSceneTransition();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            //Debug.Log("trigger exit");
            Block();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer) &&
            !willFailToDetectTriggerEnter)
        {
            //Debug.Log("must kill remaining enemies");
            willFailToDetectTriggerEnter = true;
            LevelManager.ShowHint();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer) &&
            willFailToDetectTriggerEnter)
        {
            willFailToDetectTriggerEnter = false;
        }
    }

    public void Unblock()
    {
        if (bc) bc.isTrigger = true;
    }

    public void Block()
    {
        if (bc) bc.isTrigger = false;
    }

    private void NextScene()
    {
        LevelManager.NextScene();
    }

    private void StartSceneTransition()
    {
        if (screenOverlayAnimator) screenOverlayAnimator.Play(TRANSITION_ANIM);
        if (transitionTime > 0) Invoke(nameof(NextScene), transitionTime);
        else NextScene();
    }
}
