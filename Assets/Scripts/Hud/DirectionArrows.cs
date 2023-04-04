using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrows : MonoBehaviour
{
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject rightArrow;
    public GameObject leftArrow;

    Vector2 currentShowingDirection;

    public void ShowNextSceneDirection(Vector2 currentSceneDireciton)
    {
        if (currentSceneDireciton == Vector2.up) upArrow.SetActive(true);
        else if (currentSceneDireciton == Vector2.down) downArrow.SetActive(true);
        else if (currentSceneDireciton == Vector2.right) rightArrow.SetActive(true);
        else if (currentSceneDireciton == Vector2.left) leftArrow.SetActive(true);

        currentShowingDirection = currentSceneDireciton;
    }

    public void HideArrows()
    {
        if (currentShowingDirection == Vector2.up) upArrow.SetActive(false);
        else if (currentShowingDirection == Vector2.down) downArrow.SetActive(false);
        else if (currentShowingDirection == Vector2.right) rightArrow.SetActive(false);
        else if (currentShowingDirection == Vector2.left) leftArrow.SetActive(false);
    }
}
