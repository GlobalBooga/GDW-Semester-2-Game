using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBoss : MonoBehaviour
{
    private List<string> allAttacks = new() { nameof(Attack1), nameof(Attack2), nameof(Attack3), nameof(Attack4) };
    private Queue<string> attackPattern = new();

    [SerializeField] private bool enableAttack1;
    [SerializeField] private bool enableAttack2;
    [SerializeField] private bool enableAttack3;
    [SerializeField] private bool enableAttack4;

    HPComponent hp;

    
    private void Start()
    {
        hp = GetComponent<HPComponent>();
        if (hp) hp.OnHPZero = OnDied;
    }

    private void Update()
    {
            
    }

    /// <summary>
    /// Attack coroutine, you can have as many of these as you like
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack1()
    {
        // delay of 1 frame
        yield return null;

        // delay of 1 second
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator Attack2()
    {
        // this line is only here because otherwise it will give an error
        // when you start coding, move it to where you need it
        yield return null;
    }

    private IEnumerator Attack3()
    {
        yield return null;
    }

    
    private IEnumerator Attack4()
    {
        yield return null;
    }

    /// <summary>
    /// Called when hp is 0
    /// </summary>
    public void OnDied()
    {

    }

    /// <summary>
    /// I copied this from the Andaroz script.
    /// It will make a random list of attacks and store it in the 'attackPattern' queue.
    /// </summary>
    private void NewAttackOrder()
    {
        for (int i = 0; i < allAttacks.Count; i++)
        {
            string newAttack = allAttacks[Random.Range(0, allAttacks.Count)];

            while (attackPattern.Contains(newAttack))
            {
                newAttack = allAttacks[Random.Range(0, allAttacks.Count)];
            }

            attackPattern.Enqueue(newAttack);
        }
    }

    /// <summary>
    /// I copied this from the Andaroz script.
    /// It will check if the next attack is usable 
    /// reason = is the boss's hp low enough to use this attack? 
    /// Is this attack enabled (for debugging))
    /// </summary>
    private void NextAttack()
    {
        if (!enableAttack1 && !enableAttack2 && !enableAttack3 && !enableAttack4)
        {
            Debug.LogWarning("No attacks enabled! Enabling attack 1");
            enableAttack1 = true;
        }

        if (attackPattern.Count == 0) NewAttackOrder();

        // skip disabled attacks
        if (attackPattern.Peek() == nameof(Attack1) && enableAttack1) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Attack2) && enableAttack2) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Attack3) && enableAttack3) { attackPattern.Dequeue(); NextAttack(); return; }
        if (attackPattern.Peek() == nameof(Attack4) && enableAttack4) { attackPattern.Dequeue(); NextAttack(); return; }

        // Start the attack coroutine
        StartCoroutine(attackPattern.Dequeue());
    }
}
