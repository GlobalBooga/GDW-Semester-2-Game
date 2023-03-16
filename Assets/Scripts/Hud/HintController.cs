using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    public float hintTime = 5f;
    public GameObject objectivePrefab;

    private Animator animator;
    private RectTransform content;
    private List<Objective> objectives;

    private void Awake()
    {
        content = transform.GetChild(2).GetChild(0) as RectTransform;
    }

    public void AddObjective(string text)
    {
        Objective obj = Instantiate(objectivePrefab, content).GetComponent<Objective>();
        obj.SetObjective(text);
        objectives.Add(obj);
    }

    public void ShowHint()
    {
        if (content.childCount == 0) return;

        animator.Play("HintEnter");
        Invoke(nameof(HideHint), hintTime);
    }

    public void HideHint()
    {
        animator.Play("HintExit");
    }
}
