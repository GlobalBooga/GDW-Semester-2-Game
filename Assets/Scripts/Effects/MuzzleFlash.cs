using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public int maxframes = 3;
    int showFor;
    int frames;

    private void Awake()
    {
        showFor = Random.Range(1, maxframes);
    }

    private void OnDisable()
    {
        frames = 0;
        showFor = Random.Range(1, maxframes);
    }

    void Update()
    {
        if (frames++ >= showFor)
        {
            gameObject.SetActive(false);
        }
    }
}
