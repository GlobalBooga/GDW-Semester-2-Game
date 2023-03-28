using System.Collections;
using UnityEngine;
using Cinemachine;

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
    private float lerpSpeed = 1;

    private float damping;
    [HideInInspector] public bool restoreCamPosAfterShake = true;

    public bool isLargePanningRoom { get; set; }

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
        if (!isLargePanningRoom && !(LevelManager.instance.IsQuitting()))
        {
            Camera.main.transform.position = new Vector3(
                Mathf.Clamp(Camera.main.transform.position.x, LevelManager.instance.CurrentScene.manager.transform.position.x - 1.5f, LevelManager.instance.CurrentScene.manager.transform.position.x + 1.5f),
                Mathf.Clamp(Camera.main.transform.position.y, LevelManager.instance.CurrentScene.manager.transform.position.y - 1.5f, LevelManager.instance.CurrentScene.manager.transform.position.y + 1.5f),
                Camera.main.transform.position.z);
        }
        
        if (restoreCameraPos)
        {
            float dist = (Time.time - startTime) * 8f;
            if (totalDist > 0)
            {
                float alpha = dist / totalDist * dist;
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

    public void RestoreCamPos(float speed = 1)
    {
        RestoreCamPos(Vector2.zero, speed);
    }

    public void RestoreCamPos(Vector2 offset, float speed)
    {
        lerpStart = Camera.main.transform.position;
        lerpEnd = LevelManager.instance.CurrentScene.playerEntrance.parent.position + new Vector3(offset.x, offset.y, -10f);
        totalDist = Vector3.Distance(lerpStart, lerpEnd);
        startTime = Time.time;
        restoreCameraPos = true;
        lerpSpeed = speed;
    }

    public void SetCameraSize(float size)
    {
        virtualCamera.m_Lens.OrthographicSize = size;   
    }

    public void LerpCameraSize(float size, float speed = 1f)
    {
        lerpSpeed = speed;
        StartCoroutine(nameof(LerpCameraSizeRoutine), size);      
    }

    private IEnumerator LerpCameraSizeRoutine(float size)
    {
        yield return null;
        float currentSize = virtualCamera.m_Lens.OrthographicSize;

        float startTime = Time.time;
        float difference = Mathf.Abs(currentSize - size);

        float alpha = 0;

        while (alpha < 1)
        {
            float time = (Time.time - startTime) * 4f;
            alpha = time / difference * lerpSpeed;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, size, alpha );
            yield return null;
        }
    }

    public void SetCameraPosition(Vector2 position)
    {
        virtualCamera.transform.position = new Vector3(position.x, position.y, -10f);
    }

    public void SetCameraFollow(Transform trans)
    {
        virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>().m_Damping = 2f;
        virtualCamera.Follow = trans;
        if (trans.position.z == 0)
        {
            GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.back * 10f;
        }
        else
        {
            GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.zero;
        }
        Invoke(nameof(ResetDamping), 1f);
    }

    private void ResetDamping()
    {
        virtualCamera.GetCinemachineComponent<CinemachineHardLockToTarget>().m_Damping = 0f;
    }

    public float GetIntensity()
    {
        return intensity;
    }
}
