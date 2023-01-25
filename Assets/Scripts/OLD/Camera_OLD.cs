using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector3 offset;
    private Transform player;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        offset = new Vector3(0f, 2f, -10f);
    }

    private void Update()
    {
        transform.position = player.position + offset;
    }
}
