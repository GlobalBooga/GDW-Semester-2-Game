using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public float hintTime = 5f;
    public GameObject objectivePrefab;

    public bool IsShowing { get; private set; }

    private Animator animator;
    private RectTransform content;
    private List<Objective> objectives = new();

    private void Start()
    {
        content = transform.GetChild(2).GetChild(0) as RectTransform;
        animator = GetComponent<Animator>();
    }

    public void AddObjective(string text)
    {
        Objective obj = Instantiate(objectivePrefab, content).GetComponent<Objective>();
        obj.SetObjective(text);
        objectives.Add(obj);
    }

    public void ShowHint()
    {
        IsShowing = true;
        if (content.childCount == 0) return;

        animator.StopPlayback();

        animator.Play("HintEnter");
        if (hintTime > 0) Invoke(nameof(HideHint), hintTime);
    }

    public void HideHint()
    {
        IsShowing = false;
        animator.Play("HintExit");
    }

    public void ObjectiveComplete(string objective)
    {
        Objective o = null;
        foreach (var item in objectives)
        {
            if (item.GetObjective() == objective.ToLower())
            {
                o = item;
                item.Completed();
            }
        }

        objectives.Remove(o);


        if (objectives.Count == 0)
        {
            Invoke(nameof(HideHint), 2.75f);
        }
    }

    public void ClearObjectives()
    {
        for (int i = content.childCount-1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        objectives.Clear();
    }
}
