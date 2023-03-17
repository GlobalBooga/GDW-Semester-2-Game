using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    Animator animator;
    Text objectiveText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        objectiveText = transform.GetChild(0).GetComponent<Text>();
    }

    public void SetObjective(string text)
    {
        objectiveText.text = text;
    }

    public void Completed()
    {
        animator.Play("ObjectiveComplete");
        Destroy(gameObject, 2.5f);
    }

    public string GetObjective()
    {
        return objectiveText.text.ToLower();
    }
}
