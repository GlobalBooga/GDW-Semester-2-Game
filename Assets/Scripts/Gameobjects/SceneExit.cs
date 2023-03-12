using UnityEngine;

public class SceneExit : MonoBehaviour
{
    
    BoxCollider2D bc;
    private bool willFailToDetectTriggerEnter = false;

    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false; 
        bc = sr.GetComponent<BoxCollider2D>();

        //Block();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == StaticHelpers.PlayerLayer ||
            collision.gameObject.layer == StaticHelpers.PlayerInvincibleLayer)
        {
            //Debug.Log("trigger enter");
            LevelManager.instance.StartSceneTransition();
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
            LevelManager.instance.StartSceneTransition();
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
            LevelManager.instance.ShowHint();
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
}
