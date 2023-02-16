using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class Laser : MonoBehaviour
{
    public Transform laserStart;
    private LineRenderer lineRenderer;
    private bool isLaserOn;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void DrawLaser(Vector2 startPos, Vector2 endPos)
    {
        if (!lineRenderer) return;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void TurnOn()
    {
        if (!lineRenderer) return;

        if (!isLaserOn)
        {
            isLaserOn = true;
            lineRenderer.enabled = true;
        }
    }

    public void TurnOff()
    {
        if (!lineRenderer) return;

        if (isLaserOn)
        {
            isLaserOn = false;
            lineRenderer.enabled = false;
        }
    }
}
