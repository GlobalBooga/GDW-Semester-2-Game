using UnityEngine;

public class LargeRoomCameraController : MonoBehaviour
{
    public float upperBound;
    public float lowerBound;
    public float leftBound;
    public float rightBound;
    Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }


    private void Update()
    {
        if (upperBound > 0 || lowerBound > 0)
            if (player.position.y < upperBound && player.position.y > lowerBound) Camera.main.transform.position = new Vector3( Camera.main.transform.position.x, player.position.y, -10f);
        if (leftBound > 0 || rightBound > 0)
            if (player.position.x < rightBound && player.position.x > leftBound) Camera.main.transform.position = new Vector3(player.position.x, Camera.main.transform.position.y,  -10f);

    }
}
