using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static void SwitchLevel(GameObject nextLevel, Transform start)
    {
        GameObject.Find("Main Camera").transform.position = nextLevel.transform.position - Vector3.forward * 10f;
        GameObject.Find("Player").transform.position = start.position;
    }
}
