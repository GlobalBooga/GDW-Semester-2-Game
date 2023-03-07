using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin perlinThing;

    private float intensity;
    private float time;

    private bool isShaking;
    private bool restoreCameraPos;

    private float startTime;
    private float totalDist;
    Vector3 lerpStart;
    Vector3 lerpEnd;
    AnimationCurve curve;


    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();   
        perlinThing = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        instance = this;
        curve = AnimationCurve.EaseInOut(0,0,1,1);
    }

    private void Update()
    {
        if (restoreCameraPos)
        {
            float dist = (Time.time - startTime) * 8f;
            float alpha = dist / totalDist;
            Camera.main.transform.position = Vector3.Lerp(lerpStart, lerpEnd, curve.Evaluate(alpha));
            if (alpha > 1)
            {
                restoreCameraPos = false;
            }
        }
    }

    public void ShakeCamera(float intensity, float time)
    {

        if (perlinThing.m_AmplitudeGain > intensity) return;

        this.intensity = intensity;
        this.time = time;
        
        if (isShaking) StopCoroutine(nameof(ShakeCameraRoutine));
        StartCoroutine(nameof(ShakeCameraRoutine));
    }

    public IEnumerator ShakeCameraRoutine()
    {
        restoreCameraPos = false;
        isShaking = true;
        Debug.Log("sheke start");
        perlinThing.m_AmplitudeGain = intensity;
        for (float i = 0; i < time; i+=Time.deltaTime)
        {
            if (i > 0) perlinThing.m_AmplitudeGain = Mathf.Lerp(intensity, 0f, i / time);
            yield return null;
        }
        perlinThing.m_AmplitudeGain = 0f;
        Debug.Log("sheke ed");
        isShaking = false;

        RestoreCamPos();
    }

    private void RestoreCamPos()
    {
        lerpStart = Camera.main.transform.position;
        lerpEnd = LevelManager.instance.CurrentScene.playerEntrance.parent.position + Vector3.back * 10;
        totalDist = Vector3.Distance(lerpStart, lerpEnd);
        startTime = Time.time;
        restoreCameraPos = true;
    }

    public void SetCameraSize(float size)
    {
        virtualCamera.m_Lens.OrthographicSize = size;   
    }

    public void SetCameraPosition(Vector2 position)
    {
        virtualCamera.transform.position = new Vector3(position.x, position.y, -10f);
    }
}
