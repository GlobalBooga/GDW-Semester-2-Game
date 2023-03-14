using UnityEngine;

public class LargeRoomCameraController : MonoBehaviour
{
    [Header("Pan Mode")]
    public float upperBound;
    public float lowerBound;
    public float leftBound;
    public float rightBound;

    [Header("Zoom Mode")]
    public float minCamSize;
    public float maxCamSize;
    public float zoomSpeed = 3f;
    public bool linearZoom = true;
    //public Vector2 zoomTriggerDir;

    Transform player;
    Rigidbody2D playerRb;

    bool isZoomType;
    float speed;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        isZoomType = maxCamSize > 0f;
        playerRb= player.GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (isZoomType)
        {
            if (player.position.y - Camera.main.ScreenToWorldPoint(Vector3.zero).y < 1f)
            {
                if (!linearZoom) speed = 0.5f + Mathf.Abs(player.position.y - Camera.main.ScreenToWorldPoint(Vector3.zero).y) * zoomSpeed;
                else speed = zoomSpeed;
                CameraShake.instance.SetCameraSize(Mathf.Clamp(Camera.main.orthographicSize + Time.deltaTime * speed, minCamSize, maxCamSize));
            }
            else if (player.position.y - Camera.main.ScreenToWorldPoint(Vector3.zero).y > 5f)
            {
                if (!linearZoom) speed = 0.5f + Mathf.Abs(player.position.y - Camera.main.ScreenToWorldPoint(Vector3.zero).y - 4) * zoomSpeed;
                else speed = zoomSpeed;
                CameraShake.instance.SetCameraSize(Mathf.Clamp(Camera.main.orthographicSize - Time.deltaTime * speed, minCamSize, maxCamSize));
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isZoomType)
        {
            Vector3 pos = new Vector3(player.position.y, player.position.y, Camera.main.transform.position.z);

            pos = new Vector3(
                Mathf.Clamp(pos.x, gameObject.transform.position.x - leftBound, gameObject.transform.position.x + rightBound),
                Mathf.Clamp(pos.y, gameObject.transform.position.y - lowerBound, gameObject.transform.position.y + upperBound),
                Camera.main.transform.position.z);

            Camera.main.transform.position = pos;
        }
    }
}
