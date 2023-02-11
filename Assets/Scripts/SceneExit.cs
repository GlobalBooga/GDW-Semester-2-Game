using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneExit : MonoBehaviour
{
    public Animator screenOverlayAnimator;
    public float transitionTime = 0.2f;
    BoxCollider2D bc;
    private const string TRANSITION_ANIM = "SceneTransition";

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
            if (screenOverlayAnimator) screenOverlayAnimator.Play(TRANSITION_ANIM);
            if (transitionTime > 0) Invoke(nameof(NextScene), transitionTime);
            else NextScene();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            Block();
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
}
