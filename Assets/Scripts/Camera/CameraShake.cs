using System.Collections;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin perlinThing;
    CinemachineHardLockToTarget bodysettings;

    private float intensity;
    private float time;

    private bool isShaking;
    private bool restoreCameraPos;

    private float startTime;
    private float totalDist;
    Vector3 lerpStart;
    Vector3 lerpEnd;
    AnimationCurve curve;

    private float damping;
    [HideInInspector] public bool restoreCamPosAfterShake = true;


    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();   
        perlinThing = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        bodysettings = virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>();
        instance = this;
        curve = AnimationCurve.EaseInOut(0,0,1,1);
    }

    private void Update()
    {
        if (restoreCameraPos)
        {
            float dist = (Time.time - startTime) * 8f;
            if (totalDist > 0)
            {
                float alpha = dist / totalDist;
                Camera.main.transform.position = Vector3.Lerp(lerpStart, lerpEnd, curve.Evaluate(alpha));
                if (alpha > 1)
                {
                    restoreCameraPos = false;
                }
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
        perlinThing.m_AmplitudeGain = intensity;
        for (float i = 0; i < time; i+=Time.deltaTime)
        {
            if (i > 0) perlinThing.m_AmplitudeGain = Mathf.Lerp(intensity, 0f, i / time);
            yield return null;
        }
        perlinThing.m_AmplitudeGain = 0f;
        isShaking = false;

        if (restoreCamPosAfterShake) RestoreCamPos();
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
